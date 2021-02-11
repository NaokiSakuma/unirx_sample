using UnityEngine;
using UniRx;
using System;

public class HotObservable : MonoBehaviour {
    void Start() {
        // MissHotObservable();
        SuccessHotObservable();
    }

    /// <summary>
    /// 意図していないメッセージ発行
    /// </summary>
    private void MissHotObservable() {
        Subject<int> subject = new Subject<int>();
        // subjectから生成されたObservableはHot
        IObservable<int> asObservable = subject.AsObservable();
        // 渡された値を発行
        IObservable<int> observable = asObservable.Select(x => {
            Debug.Log("Value : " + x);
            return x;
        });

        // 発行されない
        subject.OnNext(1);
        subject.OnNext(10);
        observable.Subscribe();
        // 発行される
        subject.OnNext(100);
    }

    /// <summary>
    /// 意図通りのメッセージ発行
    /// </summary>
    private void SuccessHotObservable() {
        Subject<int> subject = new Subject<int>();
        // subjectから生成されたObservableはHot
        IObservable<int> asObservable = subject.AsObservable();
        // 渡された値を発行
        IConnectableObservable<int> observable = asObservable
                                                        .Select(x => {
                                                            Debug.Log("Value : " + x);
                                                            return x;
                                                        })
                                                        // Hot変換オペレータ
                                                        .Publish();
        // ストリームの稼働開始
        observable.Connect();

        // 発行されるようになる
        subject.OnNext(1);
        subject.OnNext(10);
        observable.Subscribe();
        // 発行される
        subject.OnNext(100);
    }
}
