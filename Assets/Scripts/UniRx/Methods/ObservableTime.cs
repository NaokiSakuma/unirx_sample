using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using System.Reflection;
using UnityEditor;

public class ObservableTime : MonoBehaviour {
    private enum Methods {
        TIMER,
        TIMER_ARGUMENT,
        INTERVAL,
        DELAY,
        TIMEOUT,
        THROTTLE,
        THROTTLE_FIRST,
        SAMPLE,
        NEXT_FRAME,
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

    // Frameもある

    /// <summary>
    /// 一定時間後にメッセージを発行
    /// </summary>
    private void ExcuteTimer() {
        Observable
            .Timer(TimeSpan.FromSeconds(1))
            .Subscribe(x => {
                Debug.Log("Timer OnNext : " + x);
            }, () => {
                Debug.Log("Timer OnCompleted");
            });
    }

    // Frameもある

    /// <summary>
    /// 一定時間後に、一定間隔でメッセージを発行
    /// 発行開始と発行間隔は異なる値で指定できる
    /// </summary>
    private void ExcuteTimerArgument() {
        TimeSpan timeSpan = TimeSpan.FromSeconds(1);
        Observable
            .Timer(timeSpan, timeSpan)
            .Subscribe(x => {
                // xは0からカウントされるlong型が入る
                Debug.Log("TimerArgument OnNext : " + x);
            }, () => {
                Debug.Log("TimerArgument OnCompleted");
            });
    }

    // Intervalの定義を添付したほうが良さそう
    // 中でTimer呼んでいて、両方ともperiod
    // Frameもある

    /// <summary>
    /// 一定時間後に、一定間隔でメッセージを発行
    /// 発行開始と発行間隔はイコール
    /// </summary>
    private void ExcuteInterval() {
        Observable
            .Interval(TimeSpan.FromSeconds(1))
            .Subscribe(x => {
                Debug.Log("Interval OnNext : " + x);
            }, () => {
                Debug.Log("Interval OnCompleted");
            });
    }

    // Frameもある

    /// <summary>
    /// メッセージの発行を遅延させる
    /// </summary>
    private void ExcuteDelay() {
        Observable
            .Range(0, 1)
            .Do(x => {
                Debug.Log("Range Call");
            })
            .Delay(TimeSpan.FromSeconds(1))
            .Subscribe(x => {
                Debug.Log("Delay OnNext");
            }, () => {
                Debug.Log("Delay OnCompleted");
            });
    }

    // Frameもある

    /// <summary>
    /// 一定時間、値が発行されなかったらonErrorを発行する
    /// </summary>
    private void ExcuteTimeout() {
        // キーが押されたらメッセージ発行
        IObservable<Unit> input = this.UpdateAsObservable()
                                      .Where(_ => Input.anyKeyDown);

        Observable
            .Timeout(input, TimeSpan.FromSeconds(3))
            .Subscribe(x => {
                Debug.Log("Timeout OnNext");
            }, err => {
                Debug.Log("Timeout OnError");
            }, () => {
                Debug.Log("Timeout OnCompleted");
            });
    }

    /// <summary>
    /// 一定時間、値が発行されなかったら最後の値を流す
    /// </summary>
    private void ExcuteThrottle() {
        // キーが押されたらメッセージ発行
        IObservable<Unit> input = this.UpdateAsObservable()
                                      .Where(_ => Input.anyKeyDown);

        Observable
            .Throttle(input, TimeSpan.FromSeconds(3))
            .Subscribe(x => {
                Debug.Log("Throttle OnNext");
            }, () => {
                Debug.Log("Throttle OnCompleted");
            });
    }

    // frameもある

    /// <summary>
    /// 最初は値を発行し、一定時間発行をしない
    /// </summary>
    private void ExcuteThrottleFirst() {
        // キーが押されたらメッセージ発行
        IObservable<Unit> input = this.UpdateAsObservable()
                                      .Where(_ => Input.anyKeyDown);

        Observable
            .ThrottleFirst(input, TimeSpan.FromSeconds(3))
            .Subscribe(x => {
                Debug.Log("ThrottleFirst OnNext");
            }, () => {
                Debug.Log("ThrottleFirst OnCompleted");
            });
    }

    /// <summary>
    /// 一定間隔でメッセージを発行
    /// </summary>
    private void ExcuteSample() {
        // 1フレーム毎にメッセージを発行
        IObservable<long> inteval = Observable.IntervalFrame(1);

        Observable
            .Sample(inteval, TimeSpan.FromSeconds(1))
            .Subscribe(x => {
                Debug.Log("Sample OnNext");
            }, () => {
                Debug.Log("Sample OnCompleted");
            });
    }

    /// <summary>
    /// 次のフレームで処理をする
    /// </summary>
    private void ExcuteNextFrame() {
        Observable
            .NextFrame()
            .Subscribe(x => {
                Debug.Log("NextFrame OnNext");
            }, () => {
                Debug.Log("NextFrame OnCompleted");
            });
    }

    private void ExcuteMethods() {
        if (!_isInitialize) {
            return;
        }
        Clear();
        switch (_methodStatus) {
            case Methods.TIMER :
                ExcuteTimer();
            break;
            case Methods.TIMER_ARGUMENT :
                ExcuteTimerArgument();
            break;
            case Methods.INTERVAL :
                ExcuteInterval();
            break;
            case Methods.DELAY :
                ExcuteDelay();
            break;
            case Methods.TIMEOUT :
                ExcuteTimeout();
            break;
            case Methods.THROTTLE :
                ExcuteThrottle();
            break;
            case Methods.THROTTLE_FIRST :
                ExcuteThrottleFirst();
            break;
            case Methods.SAMPLE :
                ExcuteSample();
            break;
            case Methods.NEXT_FRAME :
                ExcuteNextFrame();
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
