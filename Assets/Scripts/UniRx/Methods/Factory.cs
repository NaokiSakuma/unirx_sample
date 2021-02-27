using UnityEngine;
using UniRx;
using System;

public class Factory : MonoBehaviour
{
    private enum Methods {
        RETURN,
        REPEAT,
        RANGE,
        DEFER,
        TIMER,
        TIMER_FRAME,
        TIMER_INTERVAL,
        TIMER_FRAME_INTERVAL,
        CREATE,
        THROW,
        EMPTY,
        NEVER,
        FROM_EVENT,
        FROM_EVENT_TUPLE,
        FROM_EVENT_EVENT_ARGS,
        EVERY_UPDATE,
        FIXED_EVERY_UPDATE,
    }

    [SerializeField]
    private Methods _methodStatus;

    void OnValidate() {
        ExcuteMethods();
    }

    /// <summary>
    /// 値を1つだけ発行
    /// </summary>
    private void ExcuteReturn() {
        // 発行する値
        int value = 1;
        Observable
            .Return(value)
            .Subscribe(x => Debug.Log("Return : " + x));
    }

    /// <summary>
    /// 値を繰り返し発行
    /// </summary>
    private void ExcuteRepeat() {
        // 発行する値
        int value = 1;
        // 繰り返す数
        int repeat = 3;
        Observable
            .Repeat(value, repeat)
            .Subscribe(x => Debug.Log("Repeat : " + x));
    }

    /// <summary>
    /// 指定した範囲で数値を発行
    /// </summary>
    private void ExcuteRange() {
        // 開始する値
        int start = 5;
        // 繰り返す数
        int count = 3;
        Observable
            .Range(start, count)
            .Subscribe(x => Debug.Log("Range : " + x));
    }

    /// <summary>
    /// ObservableをSubscribe時まで遅延
    /// </summary>
    private void ExcuteDefer() {
        // 遅延させるObservable
        IObservable<DateTime> deferObservable = Observable.Return(DateTime.Now);
        Observable.Defer(() =>
            deferObservable
        ).Subscribe(x => {
            Debug.Log("Defer : " + x);
        });
    }

    /// <summary>
    /// 一定時間後に値を発行
    /// </summary>
    private void ExcuteTimer() {
        int seconds = 1;
        Observable
            .Timer(TimeSpan.FromSeconds(seconds))
            .Subscribe(x => {
                Debug.Log("Timer : " + x);
            });
    }

    /// <summary>
    /// 指定フレーム後に値を発行
    /// </summary>
    private void ExcuteTimerFrame() {
        int frame = 30;
        Observable
            .TimerFrame(frame)
            .Subscribe(x => {
                Debug.Log("TimerFrame : " + x);
            });
    }

    /// <summary>
    /// 一定間隔で値を発行
    /// </summary>
    private void ExcuteTimerInterval() {
        // 開始までの時間
        int dueTime = 2;
        // 間隔
        int span = 1;
        // TimerとIntervalとの違いは、開始までの時間を指定できるかどうか
        Observable
            .Timer(TimeSpan.FromSeconds(dueTime), TimeSpan.FromSeconds(span))
            .Subscribe(x => {
                Debug.Log("Timer : " + x);
            });

        Observable
            .Interval(TimeSpan.FromSeconds(span))
            .Subscribe(x => {
                Debug.Log("Interval : " + x);
            });
    }

    /// <summary>
    /// 一定フレーム間隔で値を発行
    /// </summary>
    private void ExcuteFrameTimerInterval() {
        // 開始までの時間
        int dueTime = 120;
        // 間隔
        int span = 60;
        // TimerとIntervalとの違いは、開始までの時間を指定できるかどうか
        Observable
            .TimerFrame(dueTime, span)
            .Subscribe(x => {
                Debug.Log("TimerFrame : " + x);
            });

        Observable
            .IntervalFrame(span)
            .Subscribe(x => {
                Debug.Log("IntervalFrame : " + x);
            });
    }

    /// <summary>
    /// 値を発行するストリームの生成
    /// </summary>
    private void ExcuteCreate() {
        Observable
            .Create<int>(observer => {
                int value = 1;
                observer.OnNext(value);
                observer.OnCompleted();
                return Disposable
                    .Create(() => {
                        Debug.Log("Dispose");
                    });
                }).Subscribe(x => {
                Debug.Log("Create : " + x);
            });
    }

    /// <summary>
    /// onErrorを発行する
    /// </summary>
    private void ExcuteThrow() {
        string message = "error";
        Observable
            .Throw<Unit>(new System.Exception(message))
            .Subscribe(_ => {
                // 呼ばれない
                Debug.Log("Throw onNext");
            }, err => {
                // 呼ばれる
                Debug.Log("Throw onError : " + err);
            }, () => {
                // 呼ばれない
                Debug.Log("Throw OnCompleted");
            });
    }

    /// <summary>
    /// OnCompletedを発行する
    /// </summary>
    private void ExcuteEmpty() {
        Observable
            .Empty<Unit>()
            .Subscribe(_ => {
                // 呼ばれない
                Debug.Log("Empty OnNext");
            }, err => {
                // 呼ばれない
                Debug.Log("Empty onError : " + err);
            }, () => {
                // 呼ばれる
                Debug.Log("Empty OnCompleted");
            });
    }

    /// <summary>
    /// 何もしないObservableを定義
    /// </summary>
    private void ExcuteNever() {
        Observable
            .Never<Unit>()
            .Subscribe(_ => {
                // 呼ばれない
                Debug.Log("Never OnNext");
            }, err => {
                // 呼ばれない
                Debug.Log("Never OnError : " + err);
            }, () => {
                // 呼ばれない
                Debug.Log("Never OnCompleted");
            });
    }

    /// <summary>
    /// EventをObservableに変換する
    /// 引数1つまではこれで対応可能
    /// </summary>
    private void ExcuteFromEventSimple() {
        Action<int> fromEventAction = (intValue) => {
            Debug.Log("Called FromEventAction. int :  " + intValue);
        };
        Observable
            .FromEvent<int>(
                handler => fromEventAction += handler,
                handler => fromEventAction -= handler
            )
            .Subscribe(x => {
                Debug.Log("FromEventSimple onNext. int : " + x);
            });
        fromEventAction(1);
    }

    /// <summary>
    /// EventをObservableに変換する
    /// Tupleバージョン
    /// </summary>
    private void ExcuteFromEventTuple() {
        Action<int, string> fromEventAction = (intValue, strValue) => {
            Debug.Log(string.Format("Called FromEventAction. int : {0}, string : {1}", intValue, strValue));
        };
        Observable
            .FromEvent<Action<int, string>, Tuple<int, string>>(
                handler => (x, y) => handler(Tuple.Create(x, y)),
                handler => fromEventAction += handler,
                handler => fromEventAction -= handler
            )
            .Subscribe(x => {
                Debug.Log(string.Format("FromEventTuple onNext. int : {0}, string : {1}", x.Item1, x.Item2));
            });
        fromEventAction(1, "message");
    }

    /// <summary>
    /// EventをObservableに変換する
    /// EventArgsバージョン
    /// </summary>
    private void ExcuteFromEventEventArgs() {
        Action<int, string> fromEventAction = (intValue, strValue) => {
            Debug.Log(string.Format("Called FromEventAction. int : {0}, string : {1}", intValue, strValue));
        };
        Observable
            .FromEvent<Action<int, string>, FromEventActionClass>(
                handler => (x, y) => handler(new FromEventActionClass() { intValue = x, strValue = y }),
                handler => fromEventAction += handler,
                handler => fromEventAction -= handler
            )
            .Subscribe(x => {
                Debug.Log(string.Format("FromEventEventArgs onNext. int : {0}, string : {1}", x.intValue, x.strValue));
            });
        fromEventAction(1, "message");
    }

    /// <summary>
    /// UpdateをObservableに変換する
    /// </summary>
    private void ExcuteEveryUpdate() {
        Observable
            .EveryUpdate()
            .Subscribe(x => {
                Debug.Log("EveryUpdate : " + x);
            });
    }

    /// <summary>
    /// FixedUpdateをObservableに変換する
    /// </summary>
    private void ExcuteEveryFixedUpdate() {
        Observable
            .EveryFixedUpdate()
            .Subscribe(x => {
                Debug.Log("EveryFixedUpdate : " + x);
            });
    }

    /// <summary>
    /// 各メソッドの実行
    /// </summary>
    private void ExcuteMethods() {
        switch (_methodStatus) {
            case Methods.RETURN :
                ExcuteReturn();
            break;
            case Methods.REPEAT :
                ExcuteRepeat();
            break;
            case Methods.RANGE :
                ExcuteRange();
            break;
            case Methods.DEFER :
                ExcuteDefer();
            break;
            case Methods.TIMER :
                ExcuteTimer();
            break;
            case Methods.TIMER_FRAME :
                ExcuteTimerFrame();
            break;
            case Methods.TIMER_INTERVAL :
                ExcuteTimerInterval();
            break;
            case Methods.TIMER_FRAME_INTERVAL :
                ExcuteFrameTimerInterval();
            break;
            case Methods.CREATE :
                ExcuteCreate();
            break;
            case Methods.THROW :
                ExcuteThrow();
            break;
            case Methods.EMPTY :
                ExcuteEmpty();
            break;
            case Methods.NEVER :
                ExcuteNever();
            break;
            case Methods.FROM_EVENT :
                ExcuteFromEventSimple();
            break;
            case Methods.FROM_EVENT_TUPLE :
                ExcuteFromEventTuple();
            break;
            case Methods.FROM_EVENT_EVENT_ARGS :
                ExcuteFromEventEventArgs();
            break;
            case Methods.EVERY_UPDATE :
                ExcuteEveryUpdate();
            break;
            case Methods.FIXED_EVERY_UPDATE :
                ExcuteEveryFixedUpdate();
            break;
        }
    }
}

// ExcuteFromEventEventArgsのClass
public class FromEventActionClass : EventArgs
{
    public int intValue;
    public string strValue;
}