using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using UniRx.Triggers;
using System;
using System.Reflection;
using UnityEditor;


public class ObservableCompleted : MonoBehaviour {
    private enum Methods {
        REPEAT,
        REPEAT_COUNT,
        REPEAT_SAFE,
        REPEAT_UNTIL_DISABLE,
        REPEAT_UNTIL_DESTORY,
        DIFF_TERMINATE_FINALLY_COMPLETED,
        DIFF_TERMINATE_FINALLY_ERROR,
        DIFF_TERMINATE_FINALLY_DISPOSE,
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
    /// ストリームのOnCompletedが呼ばれたら、同じストリームを生成する
    /// </summary>
    private void ExcuteRepeat() {
        Observable
            .Timer(TimeSpan.FromSeconds(1))
            .Do(_ => Debug.Log("Do"))
            .Repeat()
            .Subscribe();
    }

    /// <summary>
    /// ストリームのOnCompletedが呼ばれたら、同じストリームを指定回数生成する
    /// </summary>
    private void ExcuteRepeatCount() {
        Observable
            .Repeat("Repeat", 3)
            .Subscribe(x => {
                Debug.Log("OnNext : " + x);
            }, () => {
                Debug.Log("OnCompleted");
            });
    }

    /// <summary>
    /// 短時間にOnCompletedが呼ばれた場合、Repeatを止める
    /// 意図的に制御できない
    /// </summary>
    private void ExcuteRepeatSafe() {
        Subject<Unit> subject = new Subject<Unit>();
        subject
            .Do(_ => {
                Debug.Log("Do");
            })
            .DoOnCompleted(() => {
                Debug.Log("DoOnCompleted");
            })
            // Repeatだと無限ループしてしまう
            //.Repeat()
            .RepeatSafe()
            .Subscribe(x => {
                Debug.Log("OnNext : " + x);
            }, () => {
                Debug.Log("OnCompleted");
            });
        subject.OnCompleted();
    }

    /// <summary>
    /// ストリームのOnCompletedが呼ばれたら、同じストリームを生成する
    /// 指定したGameObjectが非表示になったら、Repeatを中止する
    /// </summary>
    private void ExcuteRepeatUntilDisable() {
        this.UpdateAsObservable()
            .Where(_ => Input.anyKeyDown)
            .Subscribe(_ => {
                Debug.Log("anyKeyDown");
                gameObject.SetActive(false);
            });

        Observable
            .Timer(TimeSpan.FromSeconds(1))
            .Do(_ => Debug.Log("Do"))
            .RepeatUntilDisable(gameObject)
            .Subscribe(x => {
                Debug.Log("OnNext : " + x);
            }, () => {
                Debug.Log("OnCompleted");
            });
    }

    /// <summary>
    /// ストリームのOnCompletedが呼ばれたら、同じストリームを生成する
    /// 指定したGameObjectが破棄されたら、Repeatを中止する
    /// </summary>
    private void ExcuteRepeatUntilDestory() {
        this.UpdateAsObservable()
            .Where(_ => Input.anyKeyDown)
            .Subscribe(_ => {
                Debug.Log("anyKeyDown");
                Destroy(gameObject);
            });

        Observable
            .Timer(TimeSpan.FromSeconds(1))
            .Do(_ => Debug.Log("Do"))
            .RepeatUntilDestroy(gameObject)
            .Subscribe(x => {
                Debug.Log("OnNext : " + x);
            }, () => {
                Debug.Log("OnCompleted");
            });
    }

    /// <summary>
    /// ストリーム完了時のDoOnTerminateとFinallyの違い
    /// </summary>
    private void ExcuteDiffCompleted() {
        // DoOnTerminateとFinallyの両方が呼ばれる
        CreateSubject().OnCompleted();
    }

    /// <summary>
    /// 例外発生時のDoOnTerminateとFinallyの違い
    /// </summary>
    private void ExcuteDiffError() {
        // DoOnTerminateとFinallyの両方が呼ばれる
        CreateSubject().OnError(new Exception());
    }

    /// <summary>
    /// ストリーム破棄時のDoOnTerminateとFinallyの違い
    /// </summary>
    private void ExcuteDiffDispose() {
        // Finallyのみ呼ばれる
        CreateDisposable().Dispose();
    }

    /// <summary>
    /// DoOnTerminateとFinallyの違いのSubject生成
    /// </summary>
    private Subject<Unit> CreateSubject() {
        Subject<Unit> subject = new Subject<Unit>();
        subject
            // 例外発生時
            .DoOnError    (e  => Debug.Log("DoOnError"))
            // ストリーム完了時
            .DoOnCompleted(() => Debug.Log("DoOnCompleted"))
            // ストリーム破棄時
            .DoOnCancel   (() => Debug.Log("DoOnCancel"))
            // ストリーム完了、例外発生時
            .DoOnTerminate(() => Debug.Log("DoOnTerminate"))
            // ストリーム完了、ストリーム破棄、例外発生時
            .Finally      (() => Debug.Log("Finally")).Subscribe();
        return subject;
    }

    /// <summary>
    /// DoOnTerminateとFinallyの違いのDisposable生成
    /// </summary>
    private IDisposable CreateDisposable() {
        Subject<Unit> subject = new Subject<Unit>();
        return subject
                    // 例外発生時
                    .DoOnError    (e  => Debug.Log("DoOnError"))
                    // ストリーム完了時
                    .DoOnCompleted(() => Debug.Log("DoOnCompleted"))
                    // ストリーム破棄時
                    .DoOnCancel   (() => Debug.Log("DoOnCancel"))
                    // ストリーム完了、例外発生時
                    .DoOnTerminate(() => Debug.Log("DoOnTerminate"))
                    // ストリーム完了、ストリーム破棄、例外発生時
                    .Finally      (() => Debug.Log("Finally")).Subscribe();
    }



    private void ExcuteMethods() {
        if (!_isInitialize) {
            return;
        }
        Clear();
        switch (_methodStatus) {
            case Methods.REPEAT :
                ExcuteRepeat();
            break;
            case Methods.REPEAT_COUNT :
                ExcuteRepeatCount();
            break;
            case Methods.REPEAT_SAFE :
                ExcuteRepeatSafe();
            break;
            case Methods.REPEAT_UNTIL_DISABLE :
                ExcuteRepeatUntilDisable();
            break;
            case Methods.REPEAT_UNTIL_DESTORY :
                ExcuteRepeatUntilDestory();
            break;
            case Methods.DIFF_TERMINATE_FINALLY_COMPLETED :
                ExcuteDiffCompleted();
            break;
            case Methods.DIFF_TERMINATE_FINALLY_ERROR :
                ExcuteDiffError();
            break;
            case Methods.DIFF_TERMINATE_FINALLY_DISPOSE :
                ExcuteDiffDispose();
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