using UnityEngine;
using UniRx;
using System;

public class HotColdBranchCounter : MonoBehaviour {
    void Start() {
        // ColdCounter();
        HotCounter();
    }

    /// <summary>
    /// Coldなカウンター
    /// </summary>
    private void ColdCounter() {
        // 何かのキーを押された回数を通知する
        IObservable<int> counterObservable = Observable
                                        .EveryUpdate()
                                        .Where(_ => Input.anyKeyDown)
                                        .Select(_ => 1)
                                        .Scan((a, b) => a + b);

        // counterObservableの値を発行する
        IDisposable stream = Observable
                                .EveryUpdate()
                                .Where(_ => Input.anyKeyDown)
                                .Select(_ => 1)
                                .Scan((a, b) => a + b)
                                .Subscribe(subscribeNum => {
                                    counterObservable.Subscribe(counter => {
                                        Debug.Log(string.Format("【Cold】{0}回目のSubscribe, Counter Value : {1}", subscribeNum, counter));
                                    });
                        });
    }

    /// <summary>
    /// Hotなカウンター
    /// </summary>
    private void HotCounter() {
        // 何かのキーを押された回数を通知する
        IConnectableObservable<int> counterObservable = Observable
                                        .EveryUpdate()
                                        .Where(_ => Input.anyKeyDown)
                                        .Select(_ => 1)
                                        .Scan((a, b) => a + b)
                                        // Hot変換
                                        .Publish();

        // counterObservableの値を発行する
        IDisposable stream = Observable
                                .EveryUpdate()
                                .Where(_ => Input.anyKeyDown)
                                .Select(_ => 1)
                                .Scan((a, b) => a + b)
                                .Subscribe(subscribeNum => {
                                    counterObservable.Subscribe(counter => {
                                        Debug.Log(string.Format("【Hot】{0}回目のSubscribe, Counter Value : {1}", subscribeNum, counter));
                                    });
                        });

        // ストリーム稼働
        counterObservable.Connect();
    }
}
