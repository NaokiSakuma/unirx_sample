using UnityEngine;
using UniRx;
using System;

public class ColdObservable : MonoBehaviour {
    void Start() {
        SubscribeTimer();
    }

    /// <summary>
    /// Coldの例
    /// </summary>
    private void ColdTimerObservable() {
        // ColdなObservableのみなので、発行されない
        IObservable<long> timer= Observable
                    // 1秒ごとに発行
                    .Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
                    .Select(x => {
                        Debug.Log(x);
                        return x;
                    });
    }

    /// <summary>
    /// Coldの例をSubscribe
    /// </summary>
    private void SubscribeTimer() {
        IObservable<long> timer= Observable
                    // 1秒ごとに発行
                    .Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
                    .Select(x => {
                        Debug.Log(x);
                        return x;
                    });
        // Subscribeする
        timer.Subscribe();
    }
}


