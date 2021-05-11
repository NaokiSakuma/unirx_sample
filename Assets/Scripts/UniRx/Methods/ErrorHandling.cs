using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using System.Reflection;
using UnityEditor;

public class ErrorHandling : MonoBehaviour {
    private enum Methods {
        DO_ON_ERROR,
        RETRY,
        ON_ERROR_RETRY,
        ON_ERROR_RETRY_TIME,
        CATCH,
        CATCH_TARGET,
        CATCH_IGNORE,
    }

    [SerializeField]
    private Methods _methodStatus;
    [SerializeField]
    private bool _isDo = false;

    private bool _isInitialize = false;

    void Start() {
        _isInitialize = true;
        this.UpdateAsObservable()
            .Where(_ => _isDo)
            .Subscribe(_ => {
                _isDo = false;
            });
    }

    void OnValidate() {
        ExcuteMethods();
    }

    /// <summary>
    /// 例外発生時に、例外を補足せずに処理を行う
    /// </summary>
    private void ExtionDoOnError() {
        Observable
            .Range(0, 1)
            .Do(_ => throw new Exception())
            .DoOnError(e => {
                Debug.Log("DoOnError : " + e);
            })
            .Subscribe(x => {
                Debug.Log("OnNext : " + x);
            }, e => {
                Debug.Log("OnError : " + e);
            }, () => {
                Debug.Log("OnCompleted");
            });
    }

    /// <summary>
    /// 例外発生時に、Subscribeし直す
    /// </summary>
    private void ExcuteRetry() {
        Observable
            .Range(0, 1)
            .Do(_ => throw new Exception())
            .DoOnError(e => Debug.Log("DoOnError : " + e))
            // 3回繰り返す
            .Retry(3)
            .Subscribe(x => {
                Debug.Log("OnNext : " + x);
            }, e => {
                Debug.Log("OnError : " + e);
            }, () => {
                Debug.Log("OnCompleted");
            });
    }

    /// <summary>
    /// OnErrorRetryの実行
    /// </summary>
    private void ExcuteOnErrorRetry() {
        Debug.Log("------throw A------");
        OnErrorRetryThrowError(new ExceptionA());
        Debug.Log("------throw B------");
        OnErrorRetryThrowError(new ExceptionB());
    }

    /// <summary>
    /// 指定した型の例外の場合、Subscribeし直す
    /// </summary>
    private void OnErrorRetryThrowError(Exception error) {
        Observable
            .Range(0, 1)
            .Do(_ => throw error)
            .DoOnError(e => Debug.Log("DoOnError : " + e))
            // ExceptionAが来た場合
            .OnErrorRetry((ExceptionA e) => {
                Debug.Log("OnErrorRetry A");
            }, 3)
            // ExceptionBが来た場合
            .OnErrorRetry((ExceptionB e) => {
                Debug.Log("OnErrorRetry B");
            }, 3)
            .Subscribe(x => {
                Debug.Log("OnNext : " + x);
            }, e => {
                Debug.Log("OnError : " + e);
            }, () => {
                Debug.Log("OnCompleted");
            });
    }

    /// <summary>
    /// 指定した回数、指定した間隔でSubscribeし直す
    /// </summary>
    private void ExcuteOnErrorRetryTime() {
        Observable
            .Range(0, 1)
            .Do(_ => throw new Exception())
            .DoOnError(e => Debug.Log("DoOnError : " + e))
            // 指定した回数、指定間隔で実行
            .OnErrorRetry((Exception e) => {
                Debug.Log("OnErrorRetry");
            }, 3, TimeSpan.FromSeconds(1))
            .Subscribe(x => {
                Debug.Log("OnNext : " + x);
            }, e => {
                Debug.Log("OnError : " + e);
            }, () => {
                Debug.Log("OnCompleted");
            });
    }

    /// <summary>
    /// 例外発生時に、代わりのObservableを渡す
    /// </summary>
    private void ExcuteCatch() {
        Observable
            .Range(0, 1)
            .Select(x => x.ToString())
            .Do(_ => throw new Exception())
            .Catch((Exception e) => {
                // エラー処理の実行
                return Observable.Return("Catch");
            })
            .Subscribe(x => {
                Debug.Log("OnNext : " + x);
            }, e => {
                Debug.Log("OnError : " + e);
            }, () => {
                Debug.Log("OnCompleted");
            });
    }

    /// <summary>
    /// 指定した型をCatchの実行
    /// </summary>
    private void ExcuteCatchTarget() {
        Debug.Log("------throw A------");
        CatchThrowError(new ExceptionA());
        Debug.Log("------throw B------");
        CatchThrowError(new ExceptionB());
    }

    /// <summary>
    /// 指定した型の例外発生時に、代わりのObservableを渡す
    /// </summary>
    private void CatchThrowError(Exception error) {
        Observable
            .Range(0, 1)
            .Select(x => x.ToString())
            .Do(_ => throw error)
            // ExceptionAが来た場合
            .Catch((ExceptionA e) => {
                return Observable.Return("Catch A");
            })
            // ExceptionBが来た場合
            .Catch((ExceptionB e) => {
                return Observable.Return("Catch B");
            })
            .Subscribe(x => {
                Debug.Log("OnNext : " + x);
            }, e => {
                Debug.Log("OnError : " + e);
            }, () => {
                Debug.Log("OnCompleted");
            });
    }

    /// <summary>
    /// Catch時にObservableを返さずにOnCompletedを発行
    /// </summary>
    private void ExcuteCatchIgnore() {
        Observable
            .Range(0, 1)
            .Select(x => x.ToString())
            .Do(_ => throw new Exception())
            .CatchIgnore((Exception e) => {
                Debug.Log("Catch Ignore : " + e);
            })
            .Subscribe(x => {
                Debug.Log("OnNext : " + x);
            }, e => {
                Debug.Log("OnError : " + e);
            }, () => {
                Debug.Log("OnCompleted");
            });
    }

    private void ExcuteMethods() {
        if (!_isInitialize) {
            return;
        }
        Clear();
        switch (_methodStatus) {
            case Methods.DO_ON_ERROR :
                ExtionDoOnError();
            break;
            case Methods.RETRY :
                ExcuteRetry();
            break;
            case Methods.ON_ERROR_RETRY :
                ExcuteOnErrorRetry();
            break;
            case Methods.ON_ERROR_RETRY_TIME :
                ExcuteOnErrorRetryTime();
            break;
            case Methods.CATCH :
                ExcuteCatch();
            break;
            case Methods.CATCH_TARGET :
                ExcuteCatchTarget();
            break;
            case Methods.CATCH_IGNORE :
                ExcuteCatchIgnore();
            break;
        }
    }

    /// <summary>
    /// Unityのコンソールログを消す
    /// </summary>
    private void Clear()
    {
        var type = Assembly
            .GetAssembly( typeof( SceneView ) )
#if UNITY_2017_1_OR_NEWER
            .GetType( "UnityEditor.LogEntries" )
#else
            .GetType( "UnityEditorInternal.LogEntries" )
#endif
        ;
        var method = type.GetMethod( "Clear", BindingFlags.Static | BindingFlags.Public );
        method.Invoke( null, null );
    }
}

public class ExceptionA : Exception { }
public class ExceptionB : Exception { }