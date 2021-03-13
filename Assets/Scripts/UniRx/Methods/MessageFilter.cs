using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;

public class MessageFilter : MonoBehaviour {

    private enum Methods {
        WHERE,
        DISTINCT,
        DISTINCT_UNTIL_CHANGED,
        THROTTLE,
        THROTTLE_FIRST,
        FIRST,
        LAST,
        SINGLE,
        TAKE,
        TAKE_WHILE,
        TAKE_UNTIL,
        SKIP,
        SKIP_WHILE,
        SKIP_UNTIL,
        IGNORE_ELEMENTS,
    }

    [SerializeField]
    private Methods _methodStatus;

    private bool _isInitialize = false;

    void Start() {
        _isInitialize = true;
    }

    void OnValidate() {
        ExcuteMethods();
    }

    /// <summary>
    /// 条件を満たすものを流す
    /// </summary>
    private void ExcuteWhere() {
        Subject<int> subject = new Subject<int>();
        subject
            .Where(x => x > 10)
            .Subscribe(x => {
                Debug.Log("Where : " + x);
            });
        // 流れない
        subject.OnNext(1);
        // 流れない
        subject.OnNext(10);
        // 流れる
        subject.OnNext(100);
    }

    /// <summary>
    /// 1度流れたものは流さない
    /// </summary>
    private void ExcuteDistinct() {
        Subject<int> subject = new Subject<int>();
        subject
            .Distinct()
            .Subscribe(x => {
                Debug.Log("Distinct : " + x);
            });
        // 流れる
        subject.OnNext(1);
        // 流れない
        subject.OnNext(1);
        // 流れる
        subject.OnNext(0);
        // 流れない
        subject.OnNext(1);
        // 流れない
        subject.OnNext(0);
    }

    /// <summary>
    /// 値が前回と異なる場合に流す
    /// </summary>
    private void ExcuteDistinctUntilChanged() {
        Subject<int> subject = new Subject<int>();
        subject
            .DistinctUntilChanged()
            .Subscribe(x => {
                Debug.Log("DistinctUntilChanged : " + x);
            });
        // 流れる
        subject.OnNext(1);
        // 流れない
        subject.OnNext(1);
        // 流れる
        subject.OnNext(0);
        // 流れる
        subject.OnNext(1);
        // 流れる
        subject.OnNext(0);
    }

    /// <summary>
    /// 指定した時間値が流れてこなかったら最後のonNextを流す
    /// </summary>
    private void ExcuteThrottle() {
        this.UpdateAsObservable()
            .Where(_ => Input.anyKeyDown)
            .Throttle(TimeSpan.FromSeconds(2))
            .Subscribe(_ => {
                Debug.Log("Throttle");
            });
    }

    /// <summary>
    /// 最初は値を流し、指定した時間は流さないを繰り返す
    /// </summary>
    private void ExcuteThrottleFirst() {
        this.UpdateAsObservable()
            .ThrottleFirst(TimeSpan.FromSeconds(1))
            .Subscribe(_ => {
                Debug.Log("ThrottleFirst");
            });
    }

    // First,Single,LastにはDefault〇〇がある
    // 違いは値を一度も流さずにonCompletedを呼ぶとDefaultがあるとエラーが出ない

    /// <summary>
    /// 最初のonNextのみ通し、onCompletedを発行
    /// </summary>
    private void ExcuteFirst() {
        Subject<int> subject = new Subject<int>();
        subject
            .First()
            .Subscribe(x => {
                Debug.Log("First onNext : " + x);
            }, () => {
                Debug.Log("First onCompleted");
            });
        // 流れる
        subject.OnNext(1);
        // 流れない
        subject.OnNext(10);
        // 流れない
        subject.OnNext(100);
    }

    /// <summary>
    /// onCompletedを発行した時点での最後の値を発行
    /// </summary>
    private void ExcuteLast() {
        Subject<int> subject = new Subject<int>();
        subject
            .Last()
            .Subscribe(x => {
                Debug.Log("Last onNext : " + x);
            }, () => {
                Debug.Log("Last onCompleted");
            });
        // 流れない
        subject.OnNext(1);
        // 流れない
        subject.OnNext(10);
        // 流れる
        subject.OnNext(100);
        subject.OnCompleted();
    }

    /// <summary>
    /// OnNextが2回以上流れたら、onErrorを発行
    /// </summary>
    private void ExcuteSingle() {
        Subject<int> subject = new Subject<int>();
        subject
            .Single()
            .Subscribe(x => {
                Debug.Log("Single onNext : " + x);
            }, err => {
                Debug.Log("Single onError : " + err);
            });
        subject.OnNext(1);
        // onErrorを発行
        subject.OnNext(10);
    }

    /// <summary>
    /// 指定した数だけ流す
    /// </summary>
    private void ExcuteTake() {
        Subject<int> subject = new Subject<int>();
        int count = 3;
        subject
            .Take(count)
            .Subscribe(x => {
                Debug.Log("Take onNext : " + x);
            });
        // 流れる
        subject.OnNext(1);
        // 流れる
        subject.OnNext(10);
        // 流れる
        subject.OnNext(100);
        // 流れない
        subject.OnNext(1000);
        // 流れない
        subject.OnNext(10000);
    }

    /// <summary>
    /// 指定した条件を満たしている間は流し、一度でも満たしていないものが来たら以降は流さない
    /// </summary>
    private void ExcuteTakeWhile() {
        Subject<int> subject = new Subject<int>();
        subject
            .TakeWhile(x => x > 10)
            .Subscribe(x => {
                Debug.Log("TakeWhile onNext : " + x);
            });
        // 流れる
        subject.OnNext(100);
        // 流れる
        subject.OnNext(1000);
        // 流れない
        subject.OnNext(10);
        // 流れない
        subject.OnNext(10000);
        // 流れない
        subject.OnNext(100000);
    }

    /// <summary>
    /// 指定したObservableのonNextが発行されるまで流す
    /// </summary>
    private void ExcuteTakeUntil() {
        Subject<int> subject = new Subject<int>();
        Subject<int> otherSubject = new Subject<int>();
        subject
            .TakeUntil(otherSubject)
            .Subscribe(x => {
                Debug.Log("TakeUntil onNext : " + x);
            });
        // 流れる
        subject.OnNext(1);
        // 流れる
        subject.OnNext(10);
        otherSubject.OnNext(1);
        // 流れない
        subject.OnNext(100);
    }

    /// <summary>
    /// 指定した数流さず、それ以降は流す
    /// </summary>
    private void ExcuteSkip() {
        Subject<int> subject = new Subject<int>();
        int count = 3;
        subject
            .Skip(count)
            .Subscribe(x => {
                Debug.Log("Skip onNext : " + x);
            });
        // 流れない
        subject.OnNext(1);
        // 流れない
        subject.OnNext(10);
        // 流れない
        subject.OnNext(100);
        // 流れる
        subject.OnNext(1000);
        // 流れる
        subject.OnNext(10000);
    }

    /// <summary>
    /// 指定した条件を満たしている間は流さず、一度でも満たしているものが来たら以降は流す
    /// </summary>
    private void ExcuteSkipWhile() {
        Subject<int> subject = new Subject<int>();
        subject
            .SkipWhile(x => x > 10)
            .Subscribe(x => {
                Debug.Log("SkipWhile : onNext : " + x);
            });
        // 流れない
        subject.OnNext(100);
        // 流れない
        subject.OnNext(1000);
        // 流れる
        subject.OnNext(10);
        // 流れる
        subject.OnNext(10000);
        // 流れる
        subject.OnNext(100000);
    }

    /// <summary>
    /// 指定したObservableのonNextが発行されるまで流さない
    /// </summary>
    private void ExcuteSkipUntil() {
        Subject<int> subject = new Subject<int>();
        Subject<int> otherSubject = new Subject<int>();
        subject
            .SkipUntil(otherSubject)
            .Subscribe(x => {
                Debug.Log("SkipUntil onNext : " + x);
            });
        // 流れない
        subject.OnNext(1);
        // 流れない
        subject.OnNext(10);
        otherSubject.OnNext(1);
        // 流れる
        subject.OnNext(100);
    }

    /// <summary>
    /// OnErrorかonCompletedのみ流し、onNextは流さない
    /// </summary>
    private void ExcuteIgnoreElements() {
        Subject<int> subject = new Subject<int>();
        subject
            .IgnoreElements()
            .Subscribe(x => {
                Debug.Log("IgnoreElements onNext : " + x);
            }, err => {
                Debug.Log("IgnoreElements onError : " + err);
            }, () => {
                Debug.Log("IgnoreElements onCompleted ");
            });
        // 流れない
        subject.OnNext(1);
        // 流れる
        subject.OnError(new System.Exception("error"));
        // この例だと流れないが、流れる
        subject.OnCompleted();
    }


    private void ExcuteMethods() {
        if (!_isInitialize) {
            // このコメントアウト外すとデバッグしにくいからコメント
            // return;
        }
        switch (_methodStatus) {
            case Methods.WHERE :
                ExcuteWhere();
            break;
            case Methods.DISTINCT :
                ExcuteDistinct();
            break;
            case Methods.DISTINCT_UNTIL_CHANGED :
                ExcuteDistinctUntilChanged();
            break;
            case Methods.THROTTLE :
                ExcuteThrottle();
            break;
            case Methods.THROTTLE_FIRST :
                ExcuteThrottleFirst();
            break;
            case Methods.FIRST :
                ExcuteFirst();
            break;
            case Methods.LAST :
                ExcuteLast();
            break;
            case Methods.SINGLE :
                ExcuteSingle();
            break;
            case Methods.TAKE :
                ExcuteTake();
            break;
            case Methods.TAKE_WHILE :
                ExcuteTakeWhile();
            break;
            case Methods.TAKE_UNTIL :
                ExcuteTakeUntil();
            break;
            case Methods.SKIP :
                ExcuteSkip();
            break;
            case Methods.SKIP_WHILE :
                ExcuteSkipWhile();
            break;
            case Methods.SKIP_UNTIL :
                ExcuteSkipUntil();
            break;
            case Methods.IGNORE_ELEMENTS :
                ExcuteIgnoreElements();
            break;
        }
    }
}
