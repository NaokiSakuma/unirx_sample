using UnityEngine;
using UniRx;
using System;

public class ObservableMarge : MonoBehaviour {
    private enum Methods {
        AMB,
        ZIP,
        ZIP_LATEST,
        COMBINE_LATEST,
        WITH_LATEST_FROM,
        MERGE,
        CONCAT,
        SELECT_MANY,
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
    /// 複数のObservableから一番早く値が流れてきたものを流す
    /// </summary>
    private void ExcuteAmb() {
        Observable
            .Amb(
                // 流れない
                Observable.Timer(TimeSpan.FromSeconds(3)).Select(_ => "3s"),
                // 流れない
                Observable.Timer(TimeSpan.FromSeconds(2)).Select(_ => "2s"),
                // 流れる
                Observable.Timer(TimeSpan.FromSeconds(1)).Select(_ => "1s"))
            .Subscribe(x => {
                Debug.Log("Amb onNext : " + x);
            }, () => {
                Debug.Log("Amb onCompleted");
            });
        // 値が流れてきたら、onCompleted
    }

    /// <summary>
    /// 合成元のObservable全てに1回以上値が流れたら、合成後に値を流す
    /// 同じObservableに複数の値が流れていた場合、先に流れた値が優先される
    /// </summary>
    private void ExcuteZip() {
        Subject<int> subjectFirst = new Subject<int>();
        Subject<int> subjectSecond = new Subject<int>();

        Observable
            .Zip(subjectFirst, subjectSecond)
            .Subscribe(x => {
                int index = 0;
                foreach (int item in x) {
                    index++;
                    Debug.Log(string.Format("Zip onNext : {0}番目のonNext : {1}", index, item));
                }
            }, () => {
                Debug.Log("Zip onCompleted");
            });

        // 流れない
        subjectFirst.OnNext(1);
        // 流れる Firstは1が流れる
        subjectSecond.OnNext(10);
        // 流れない
        subjectSecond.OnNext(100);
        // 流れない
        subjectSecond.OnNext(1000);
        // 流れる Secondは100が流れる
        subjectFirst.OnNext(10000);

        // onCompleted呼ばれない
        subjectFirst.OnCompleted();
        // onCompleted呼ばれる
        subjectSecond.OnCompleted();
    }

    /// <summary>
    /// 合成元のObservable全てに1回以上値が流れたら、合成後に値を流す
    /// 同じObservableに複数の値が流れていた場合、後に流れた値が優先される
    /// </summary>
    private void ExcuteZipLatest()
    {
        Subject<int> subjectFirst = new Subject<int>();
        Subject<int> subjectSecond = new Subject<int>();

        Observable
            .ZipLatest(subjectFirst, subjectSecond)
            .Subscribe(x => {
                int index = 0;
                foreach (int item in x) {
                    index++;
                    Debug.Log(string.Format("ZipLatest onNext : {0}番目のonNext : {1}", index, item));
                }
            }, () => {
                Debug.Log("ZipLatest onCompleted");
            });

        // 流れない
        subjectFirst.OnNext(1);
        // 流れる Firstは1が流れる
        subjectSecond.OnNext(10);
        // 流れない
        subjectSecond.OnNext(100);
        // 流れない
        subjectSecond.OnNext(1000);
        // 流れる Secondは1000が流れる
        subjectFirst.OnNext(10000);

        // onCompleted呼ばれない
        subjectFirst.OnCompleted();
        // onCompleted呼ばれる
        subjectSecond.OnCompleted();
    }

    /// <summary>
    /// 合成元のObservableに値が流れたら、他のObservableの最後に発行した値を流す
    /// 1回も流れていないObservableがあれば流さない
    /// </summary>
    private void ExcuteCombineLatest() {
        Subject<int> subjectFirst = new Subject<int>();
        Subject<int> subjectSecond = new Subject<int>();

        Observable
            .CombineLatest(subjectFirst, subjectSecond)
            .Subscribe(x => {
                int index = 0;
                foreach (int item in x) {
                    index++;
                    Debug.Log(string.Format("CombineLatest onNext : {0}番目のonNext : {1}", index, item));
                }
            }, () => {
                Debug.Log("CombineLatest onCompleted");
            });

        // 流れない
        subjectFirst.OnNext(1);
        // 流れる Firstは1が流れる
        subjectSecond.OnNext(10);
        // 流れる Firstは1が流れる
        subjectSecond.OnNext(100);
        // 流れる Firstは1が流れる
        subjectSecond.OnNext(1000);
        // 流れる Secondは1000が流れる
        subjectFirst.OnNext(10000);

        // onCompleted呼ばれない
        subjectFirst.OnCompleted();
        // onCompleted呼ばれる
        subjectSecond.OnCompleted();
    }

    /// <summary>
    /// 1つのObservableをメインとし、メインに値が流れてきたらサブのObservableと合成する
    /// サブのObservableは一番最後に流れてきた値
    /// </summary>
    private void ExcuteWithLatestFrom() {
        Subject<int> subjectMain = new Subject<int>();
        Subject<int> subjectSub = new Subject<int>();

        Observable
            .WithLatestFrom(subjectMain, subjectSub, (mainValue, subValue) => mainValue + subValue)
            .Subscribe(x => {
                Debug.Log("WithLatestFrom onNext : " + x);
            }, () => {
                Debug.Log("WithLatestFrom onCompleted");
            });

        // 流れない
        subjectMain.OnNext(1);
        // 流れない
        subjectSub.OnNext(10);
        // 流れない
        subjectSub.OnNext(100);
        // 流れる 1100
        subjectMain.OnNext(1000);
        // 流れる 10100
        subjectMain.OnNext(10000);

        // onCompleted呼ばれる
        subjectMain.OnCompleted();
    }

    /// <summary>
    /// 複数のObservableを1つにまとめる
    /// </summary>
    private void ExcuteMerge() {
        Subject<int> subjectFirst = new Subject<int>();
        Subject<int> subjectSecond = new Subject<int>();

        Observable
            .Merge(subjectFirst, subjectSecond)
            .Subscribe(x => {
                Debug.Log("Merge onNext : " + x);
            }, () => {
                Debug.Log("Merge onCompleted");
            });

        // 流れる
        subjectFirst.OnNext(1);
        // 流れる
        subjectSecond.OnNext(10);
        // 流れる
        subjectFirst.OnNext(100);

        // onCompleted呼ばれない
        subjectFirst.OnCompleted();
        // onCompleted呼ばれる
        subjectSecond.OnCompleted();
    }


    /// <summary>
    /// 複数のObservableを直列で流す
    /// onCompleted発行時に次のObservableを購読
    /// </summary>
    private void ExcuteConcat() {
        Subject<string> subjectFirst = new Subject<string>();
        Subject<string> subjectSecond = new Subject<string>();

        Observable
            .Concat(subjectFirst, subjectSecond)
            .Subscribe(x => {
                Debug.Log("Concat onNext : " + x);
            }, () => {
                Debug.Log("Concat onCompleted");
            });

        // 流れる
        subjectFirst.OnNext("First 1回目");
        // 流れない
        subjectSecond.OnNext("Second 1回目");
        // onCompletedは呼ばれないが、subjectSecondが購読される
        subjectFirst.OnCompleted();
        // 流れない
        subjectFirst.OnNext("First 2回目");
        // 流れる
        subjectSecond.OnNext("Second 2回目");
        // onCompleted呼ばれる
        subjectSecond.OnCompleted();
    }


    /// <summary>
    /// Observableの値を使って、別のObservableを制作して合成する
    /// SelectとMarge合わせたイメージ
    /// </summary>
    private void ExcuteSelectMany()
    {
        Subject<int> subject = new Subject<int>();

        // 0,1,2を発行する
        IObservable<int> stream = Observable.Range(0,3);

        // 合成
        subject
            .SelectMany(x => stream.Select(y => x + y))
            .Subscribe(x => {
                Debug.Log("SelectMany onNext : " + x);
            }, () => {
                Debug.Log("SelectMany onCompleted");
            });

        subject.OnNext(10);
        subject.OnNext(100);
        subject.OnCompleted();
    }


    private void ExcuteMethods() {
        if (!_isInitialize) {
            return;
        }
        switch (_methodStatus) {
            case Methods.AMB :
                ExcuteAmb();
            break;
            case Methods.ZIP :
                ExcuteZip();
            break;
            case Methods.ZIP_LATEST :
                ExcuteZipLatest();
            break;
            case Methods.COMBINE_LATEST :
                ExcuteCombineLatest();
            break;
            case Methods.WITH_LATEST_FROM :
                ExcuteWithLatestFrom();
            break;
            case Methods.MERGE :
                ExcuteMerge();
            break;
            case Methods.CONCAT :
                ExcuteConcat();
            break;
            case Methods.SELECT_MANY :
                ExcuteSelectMany();
            break;

        }
    }
}
