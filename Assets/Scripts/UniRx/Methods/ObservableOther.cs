using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using System.Reflection;
using UnityEditor;

public class ObservableOther : MonoBehaviour {
    private enum Methods {
        START_WITH,
        TO_ARRAY,
        TO_LIST,
        DO_OPERATORS,
        FOR_EACH_ASYNC,
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
    /// 購読時に指定したメッセージを最初の値として発行する
    /// </summary>
    private void ExcuteStartWith() {
        Observable
            .Range(0, 3)
            .Select(x => x.ToString())
            // Rangeより先に呼ばれる
            .StartWith("Red", "Green", "Blue")
            .Subscribe(x => {
                Debug.Log("StartWith OnNext : " + x);
            }, () => {
                Debug.Log("StartWith OnCompleted");
            });
    }

    /// <summary>
    /// 発行されたOnNextをキャッシュし、OnCompleted時にArrayとして1つのメッセージに変換
    /// </summary>
    private void ExcuteToArray() {
        Observable
            .Range(0, 3)
            .ToArray()
            .Subscribe(x => {
                Debug.Log("ToArray OnNext : " + x);
                foreach (var value in x) {
                    Debug.Log("Value : " + value);
                }
            }, () => {
                Debug.Log("ToArray OnCompleted");
            });
    }

    /// <summary>
    /// 発行されたOnNextをキャッシュし、OnCompleted時にListとして1つのメッセージに変換
    /// </summary>
    private void ExcuteToList() {
        Observable
            .Range(0, 3)
            .ToList()
            .Subscribe(x => {
                Debug.Log("ToList OnNext : " + x);
                foreach (var value in x) {
                    Debug.Log("Value : " + value);
                }
            }, () => {
                Debug.Log("ToList OnCompleted");
            });
    }

    /// <summary>
    /// Do系のメッセージ
    /// ストリームのメッセージを用いて処理を行う
    /// メッセージ自体の加工は行わない
    /// </summary>
    private void ExcuteDoOperators() {
        Observable
            .Range(0, 1)
            // OnNext時に呼ばれる
            .Do            (x => Debug.Log("Call Do"))
            // OnCompleted時に呼ばれる
            .DoOnCompleted(() => Debug.Log("Call DoOnCompleted"))
            // 購読されたときに呼ばれる
            .DoOnSubscribe(() => Debug.Log("Call DoOnSubscribe"))
            // OnCompleted時かOnError時に呼ばれる
            .DoOnTerminate(() => Debug.Log("Call DoOnTerminate"))
            .Subscribe();

        Subject<int> cancelSubject = new Subject<int>();
        IDisposable dispose = cancelSubject
                                // Dispose時に呼ばれる
                                .DoOnCancel(() => Debug.Log("Call DoOnCancel"))
                                .Subscribe();
        dispose.Dispose();

        Subject<int> errorSubject = new Subject<int>();
        errorSubject
            // OnError時に呼ばれる
            .DoOnError(e => Debug.Log("Call DoOnError"))
            .Subscribe();
        errorSubject.OnError(new Exception());
    }

    /// <summary>
    /// メッセージの処理を行い、一番最後にUnitを流す
    /// </summary>
    private void ExcuteForEachAsync() {
        Observable
            .Range(0, 3)
            .ForEachAsync(x => Debug.Log("ForEachAsync : " + x))
            .Subscribe(x => {
                Debug.Log("Subscribe OnNext : " + x);
            }, () => {
                Debug.Log("Subscribe OnCompleted");
            });

        Debug.Log("--------Same--------");

        // ForEachAsyncを使用しない場合
        Observable
            .Range(0, 3)
            .Do(x => Debug.Log("Do : " + x))
            .Last()
            .AsUnitObservable()
            .Subscribe(x => {
                Debug.Log("Subscribe OnNext : " + x);
            }, () => {
                Debug.Log("Subscribe OnCompleted");
            });
    }


    private void ExcuteMethods() {
        if (!_isInitialize) {
            return;
        }
        Clear();
        switch (_methodStatus) {
            case Methods.START_WITH :
                ExcuteStartWith();
            break;
            case Methods.TO_ARRAY :
                ExcuteToArray();
            break;
            case Methods.TO_LIST :
                ExcuteToList();
            break;
            case Methods.DO_OPERATORS :
                ExcuteDoOperators();
            break;
            case Methods.FOR_EACH_ASYNC :
                ExcuteForEachAsync();
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