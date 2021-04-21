using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using System.Collections;
using System.Reflection;
using UnityEditor;


public class ObservableBranch : MonoBehaviour {
    private enum Methods {
        PUBLISH,
        PUBLISH_ARGUMENT,
        PUBLISH_COROUTINE_BAD,
        PUBLISH_COROUTINE_GOOD,
        REF_COUNT_BAD,
        REF_COUNT_GOOD,
        SHARE,
        PUBLISH_LAST,
        REPLAY,
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
    /// Observableを枝分かれ
    /// </summary>
    private void ExcutePublishBranch() {
        Debug.Log("--------Cold--------");
        ExcutePublishCold();
        Debug.Log("--------Hot---------");
        ExcutePublishHot();
    }

    /// <summary>
    /// Coldなので、Observableが分岐できない
    /// </summary>
    private void ExcutePublishCold() {
        IObservable<int> stream = Observable
                                        .Range(1, 3)
                                        .Do(x => Debug.Log("Cold Value : " + x));
        stream.Subscribe();
        stream.Subscribe();
    }


    /// <summary>
    /// Hotなので、Observableが分岐する
    /// </summary>
    private void ExcutePublishHot() {
        IConnectableObservable<int> stream = Observable
                                        .Range(1, 3)
                                        .Do(x => Debug.Log("Hot Value : " + x))
                                        // Hot変換
                                        .Publish();
        stream.Subscribe();
        stream.Subscribe();
        // ストリームを稼働
        stream.Connect();
    }

    /// <summary>
    /// Observableを枝分かれ點せつつ、初期値を指定
    /// Multicast(new BehaviorSubject<T>(value))と同義
    /// </summary>
    private void ExcutePublishArgument() {
        var stream = Observable
                            .Range(1, 3)
                            // 良い感じの例え無いし、思いつかなかった
                            .Publish(100)
                            .Subscribe(x => {
                                Debug.Log("Publish Argument : " + x);
                            });
    }

    /// <summary>
    /// Observableを枝分かれ、コルーチン版
    /// サブスクリプション前にConnect
    /// </summary>
    private IEnumerator ExcutePublishCoroutineBad() {
        TimeSpan time = TimeSpan.FromSeconds(1);
        IConnectableObservable<long> stream = Observable
                                                .Interval(time)
                                                .Publish();
        // ここでConnectを呼ぶと、1回目のSubscribeと2回目のSubscribeがズレる可能性がある
        stream.Connect();
        stream.Subscribe(x => {
            Debug.Log("First Value :  " + x);
        });
        yield return new WaitForSeconds(2);
        stream.Subscribe(x => {
            Debug.Log(string.Format("<color=blue>Second Value : {0}</color>", x));
        });
    }

    /// <summary>
    /// Observableを枝分かれ、コルーチン版
    /// サブスクリプション後にConnect
    /// </summary>
    private IEnumerator ExcutePublishCoroutineGood() {
        TimeSpan time = TimeSpan.FromSeconds(1);
        IConnectableObservable<long> stream = Observable
                                                .Interval(time)
                                                .Publish();
        stream.Subscribe(x => {
            Debug.Log("First Value :  " + x);
        });
        yield return new WaitForSeconds(2);
        stream.Subscribe(x => {
            Debug.Log(string.Format("<color=blue>Second Value : {0}</color>", x));
        });
        // Connectは全てのサブスクリプションが終わってから呼び出すと
        // コルーチンやスレッドでズレることは無くなる
        stream.Connect();
    }

    /// <summary>
    /// ObserverがあればConnect、なければDispose
    /// Connectだと、Subscribeしたストリームを消してもDisposeが呼ばれない
    /// </summary>
    private void ExcuteRefCountBad() {
        TimeSpan time = TimeSpan.FromSeconds(1);
        // カウントダウンするストリーム
        IConnectableObservable<long> stream = Observable
                                        .Interval(time)
                                        .Do(x => {
                                            Debug.Log("Publish : " + x);
                                        })
                                        .Publish();
        stream.Connect();

        IDisposable subscription = stream
                                        .Subscribe(i => {
                                            Debug.Log("Subscription : " + i);
                                        });

        // 何かキーが押されたらDispose
        this.UpdateAsObservable()
            .Where(_ => Input.anyKeyDown)
            .Subscribe(_ => {
                Debug.Log("<color=red>Call Dispose</color>");
                subscription.Dispose();
            });
    }

    /// <summary>
    /// ObserverがあればConnect、なければDispose
    /// RefCountは自動で行ってくれる
    /// </summary>
    private void ExcuteRefCountGood() {
        TimeSpan time = TimeSpan.FromSeconds(1);
        // カウントダウンするストリーム
        IObservable<long> stream = Observable
                                        .Interval(time)
                                        .Do(x => {
                                            Debug.Log("Publish : " + x);
                                        })
                                        .Publish()
                                        // ObserverがあればConnect、なければDispose
                                        .RefCount();

        IDisposable subscription = stream
                                        .Subscribe(i => {
                                            Debug.Log("Subscription : " + i);
                                        });

        // 何かキーが押されたらDispose
        this.UpdateAsObservable()
            .Where(_ => Input.anyKeyDown)
            .Subscribe(_ => {
                Debug.Log("<color=red>Call Dispose</color>");
                subscription.Dispose();
            });
    }

    /// <summary>
    /// Publish().RefCount()を省略する
    /// </summary>
    private void ExcuteShare()
    {
        TimeSpan time = TimeSpan.FromSeconds(1);
        IObservable<long> stream = Observable
                                        .Interval(time)
                                        .Do(x => {
                                            Debug.Log("Publish : " + x);
                                        })
                                        // Publish().RefCount()を省略する
                                        .Share();

        IDisposable subscription = stream
                                        .Subscribe(i => {
                                            Debug.Log("Subscription : " + i);
                                        });

        this.UpdateAsObservable()
            .Where(_ => Input.anyKeyDown)
            .Subscribe(_ => {
                Debug.Log("<color=red>Call Dispose</color>");
                subscription.Dispose();
            });
    }

    /// <summary>
    /// Observableを分岐させ、最後の値のみキャッシュ
    /// </summary>
    private void ExcutePublishLast()
    {
        Debug.Log("--------PublishLast--------");
        IObservable<int> stream = Observable
                                    .Range(1, 5)
                                    .Do(x => {
                                        Debug.Log("Publishing : " + x);
                                    })
                                    // 最後の値のみキャッシュ
                                    .PublishLast()
                                    .RefCount();


        IDisposable subscription = stream
                                    .Subscribe(x => {
                                        Debug.Log("Subscription : " + x);
                                    });
        subscription.Dispose();

        // 比較用
        Debug.Log("--------Publish-------");
        IObservable<int> stream2 = Observable
                                        .Range(1, 5)
                                        .Do(x => {
                                            Debug.Log("Publishing : " + x);
                                        })
                                        .Share();

        IDisposable subscription2 = stream2
                                        .Subscribe(x => {
                                            Debug.Log("Subscription : " + x);
                                        });
        subscription2.Dispose();
    }

    /// <summary>
    /// Observableを分岐させ、全ての値をキャッシュ
    /// </summary>
    private IEnumerator ExcuteReplay()
    {
        TimeSpan time = TimeSpan.FromSeconds(1);
        var stream = Observable
                        .Interval(time)
                        .Take(3)
                        .Publish();
        stream.Connect();
        var replayStream = stream.Replay();
        replayStream.Connect();
        replayStream.RefCount();
        stream.Subscribe(x => {
            Debug.Log("First Value :  " + x);
        }).AddTo(this);
        yield return new WaitForSeconds(2);
        stream.Subscribe(x => {
            Debug.Log(string.Format("<color=blue>Second Value : {0}</color>", x));
        });
        // 実行が遅れて取得できなかった値もReplayだと出る
        replayStream.Subscribe(x => {
            Debug.Log(string.Format("<color=red>Replay Value : {0}</color>", x));
        });
    }

    private void ExcuteMethods() {
        if (!_isInitialize) {
            return;
        }
        Clear();
        switch (_methodStatus) {
            case Methods.PUBLISH :
                ExcutePublishBranch();
            break;
            case Methods.PUBLISH_ARGUMENT :
                ExcutePublishArgument();
            break;
            case Methods.PUBLISH_COROUTINE_BAD :
                StartCoroutine(ExcutePublishCoroutineBad());
            break;
            case Methods.PUBLISH_COROUTINE_GOOD :
                StartCoroutine(ExcutePublishCoroutineGood());
            break;
            case Methods.REF_COUNT_BAD :
                ExcuteRefCountBad();
            break;
            case Methods.REF_COUNT_GOOD :
                ExcuteRefCountGood();
            break;
            case Methods.SHARE :
                ExcuteShare();
            break;
            case Methods.PUBLISH_LAST :
                ExcutePublishLast();
            break;
            case Methods.REPLAY :
                StartCoroutine(ExcuteReplay());
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
