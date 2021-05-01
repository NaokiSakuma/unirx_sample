using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using System.Threading;
using System.Collections;
using System.Reflection;
using UnityEditor;


public class ObservableAsynchronous : MonoBehaviour {
    private enum Methods {
        TO_ASYNC,
        TO_ASYNC_SUBSCRIBE,
        TO_ASYNC_INVOKE_SUBSCRIBE,
        TO_ASYNC_EXAMPLE,
        START,
        START_SUBSCRIBE,
        OBSERVE_ON_MAIN_THREAD,
        OBSERVE_ON,
        SUBSCRIBE_ON,
        TO_YIELD_INSTRUCTION,
        CONTINUE_WITH,
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
    /// 処理を別スレッドで実行
    /// </summary>
    private void ExcuteToAsync() {
        Debug.Log("Default ThreadId : " + Thread.CurrentThread.ManagedThreadId);
        Observable
            .ToAsync(() => {
                Debug.Log("ToAsync ThreadId : " + Thread.CurrentThread.ManagedThreadId);
            })
            .Invoke()
            .Subscribe(x => {
                Debug.Log("ToAsync OnNext : " + x);
            }, () => {
                Debug.Log("ToAsync OnCompleted");
            });
    }

    /// <summary>
    /// ToAsync, Subscribeのみ
    /// </summary>
    private void ExcuteToAsyncSubscribe() {
        // 同じストリームを購読するので、ToAsync内は1回のみ呼ばれる
        IObservable<Unit> stream = Observable
                                        .ToAsync(() => {
                                            Debug.Log("ToAsync action called");
                                        })
                                        .Invoke();
        stream.Subscribe(x => Debug.Log("1回目"));
        stream.Subscribe(x => Debug.Log("2回目"));
    }

    /// <summary>
    /// ToAsync, InvokeとSubscribe
    /// </summary>
    private void ExcuteToAsyncInvokeSubscribe() {
        // ストリームがそれぞれ作られるので、今回の場合ToAsync内は2回呼ばれる
        // Startと同じ
        Func<IObservable<Unit>> stream = Observable
                                            .ToAsync(() => {
                                                Debug.Log("ToAsync action called");
                                            });
        stream.Invoke().Subscribe(x => Debug.Log("1回目"));
        stream.Invoke().Subscribe(x => Debug.Log("2回目"));
    }

    /// <summary>
    /// ToAsyncの例
    /// </summary>
    private void ExcuteToAsyncExample() {
        Debug.Log("--------Don't use Subscribe-------");
        // Subscribeをしなくても呼び出される
        Observable
            .ToAsync(() => {
                Debug.Log("ToAsync");
            })
            .Invoke();

        Debug.Log("--------return T-------");
        // <T>なので、ToAsync内でreturn可能
        Observable
            .ToAsync(() => {
                return DateTime.Now;
            })
            .Invoke()
            .Subscribe(x => Debug.Log("return value : " + x));
    }

    /// <summary>
    /// 処理を別スレッドで実行
    /// </summary>
    private void ExcuteStart() {
        Debug.Log("Default ThreadId : " + Thread.CurrentThread.ManagedThreadId);
        Observable
            .Start(() => {
                Debug.Log("Start ThreadId : " + Thread.CurrentThread.ManagedThreadId);
            })
            .Subscribe(x => {
                Debug.Log("Start OnNext : " + x);
            }, () => {
                Debug.Log("Start OnCompleted");
            });
    }

    /// <summary>
    /// Start, Subscribe時
    /// </summary>
    private void ExcuteStartSubscribe() {
        // ストリームがそれぞれ作られるので、今回の場合Start内は2回呼ばれる
        // ToAsync().Invoke().Subscribe()と同じ
        IObservable<Unit> stream = Observable
                                        .Start(() => {
                                            Debug.Log("Start action called");
                                        });
        stream.Subscribe(x => Debug.Log("1回目"));
        stream.Subscribe(x => Debug.Log("2回目"));
    }

    /// <summary>
    /// メッセージのスレッドをメインスレッドに変更
    /// </summary>
    private void ExcuteObserveOnMainThread() {

        Debug.Log("Default ThreadId : " + Thread.CurrentThread.ManagedThreadId);
        Observable
            // 別スレッドで実行
            .Start(() => {
                // メインスレッドではないので、UnityAPIは呼び出すことが出来ない
                // Instantiate(GameObject);
                // Button.onClick.AddListener(Hoge);
                Debug.Log("Start ThreadId : " + Thread.CurrentThread.ManagedThreadId);
            })
            // メインスレッドで実行
            .ObserveOnMainThread()
            .Subscribe(x => {
                Debug.Log("ObserveOnMainThread ThreadId : " + Thread.CurrentThread.ManagedThreadId);
            });
    }


    /// <summary>
    /// Observableの途中で別スレッドにする
    /// </summary>
    private void ExcuteObserveOn() {
        Debug.Log("Default ThreadId : " + Thread.CurrentThread.ManagedThreadId);
        Observable
            // 別スレッドで実行
            .Start(() => {
                Debug.Log("Start ThreadId : " + Thread.CurrentThread.ManagedThreadId);
            })
            // メインスレッドで実行
            .ObserveOnMainThread()
            .Do(x => Debug.Log("ObserveOnMainThread ThreadId : " + Thread.CurrentThread.ManagedThreadId))
            // 別スレッドで実行
            .ObserveOn(Scheduler.ThreadPool)
            .Subscribe(x => {
                Debug.Log("ObserveOn ThreadId : " + Thread.CurrentThread.ManagedThreadId);
            });
    }

    /// <summary>
    /// Observableを別スレッドにする
    /// </summary>
    private void ExcuteSubscribeOn() {
        Observable
            .Create<Unit>(stream => {
                stream.OnNext(Unit.Default);
                stream.OnCompleted();
                // SubscribeOnにより、別スレッドで実行される
                Debug.Log("Create ThreadId : " + Thread.CurrentThread.ManagedThreadId);
                return Disposable.Empty;
            })
            // 別スレッドで実行
            .SubscribeOn(Scheduler.ThreadPool)
            // メインスレッドで実行
            .ObserveOnMainThread()
            .Subscribe(x => {
                Debug.Log("Subscribe ThreadId : " + Thread.CurrentThread.ManagedThreadId);
            });
    }

    /// <summary>
    /// Observableの結果をコルーチン上で待ち受ける
    /// </summary>
    private void ExcuteToYieldInstruction() {
        Observable
            // gameobjectが破棄されても裏で動くので、tokenを渡す
            .FromCoroutine(token => ToYieldInstructionCoroutine(token))
            .Subscribe(x => {
                Debug.Log("ToYieldInstruction OnNext");
            }, () => {
                Debug.Log("ToYieldInstruction OnCompleted");
            });
    }

    /// <summary>
    /// 1秒後に発行されるObservableをコルーチンに変換
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    private IEnumerator ToYieldInstructionCoroutine(CancellationToken token) {
        yield return Observable
                         .Timer(TimeSpan.FromSeconds(1))
                         // コルーチンに変換
                         .ToYieldInstruction(token);
        Debug.Log("Finished Coroutine");
    }

    /// <summary>
    /// 処理を連結させる
    /// </summary>
    private void ExcuteContinueWith() {
        Subject<string> subject = new Subject<string>();
        subject.Subscribe(x => {
                Debug.Log("Subject OnNext");
            }, () => {
                Debug.Log("Subject OnCompleted");
            });

        // subjectに連結させる
        IObservable<string> continueWith = subject.ContinueWith(Observable.Return("Return"));
        continueWith.Subscribe(x => {
                Debug.Log("ContinueWith OnNext");
            }, () => {
                Debug.Log("ContinueWith OnCompleted");
            });

        subject.OnNext("OnNext");
        subject.OnCompleted();
    }

    private void ExcuteMethods() {
        if (!_isInitialize) {
            return;
        }
        Clear();
        switch (_methodStatus) {
            case Methods.TO_ASYNC :
                ExcuteToAsync();
            break;
            case Methods.TO_ASYNC_SUBSCRIBE :
                ExcuteToAsyncSubscribe();
            break;
            case Methods.TO_ASYNC_INVOKE_SUBSCRIBE :
                ExcuteToAsyncInvokeSubscribe();
            break;
            case Methods.TO_ASYNC_EXAMPLE :
                ExcuteToAsyncExample();
            break;
            case Methods.START :
                ExcuteStart();
            break;
            case Methods.START_SUBSCRIBE :
                ExcuteStartSubscribe();
            break;
            case Methods.OBSERVE_ON_MAIN_THREAD :
                ExcuteObserveOnMainThread();
            break;
            case Methods.OBSERVE_ON :
                ExcuteObserveOn();
            break;
            case Methods.SUBSCRIBE_ON :
                ExcuteSubscribeOn();
            break;
            case Methods.TO_YIELD_INSTRUCTION :
                ExcuteToYieldInstruction();
            break;
            case Methods.CONTINUE_WITH :
                ExcuteContinueWith();
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
