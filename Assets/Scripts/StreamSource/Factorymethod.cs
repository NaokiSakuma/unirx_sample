using UnityEngine;
using UniRx;
using System;
using System.Threading;

public class Factorymethod : MonoBehaviour {
    void Start() {
        // ExcuteObservableCreate();
        // ExcuteObservableStart();
        ExcuteObservableTimer();
    }

    /// <summary>
    /// ObservableCreateの実行
    /// </summary>
    private void ExcuteObservableCreate() {
        IObservable<int> create = Observable.Create<int>(x => {
            Debug.Log("Start");
            for (int i = 0; i <= 20; i+= 10) {
                x.OnNext(i);
            }
            x.OnCompleted();
            return Disposable.Create(() => {
                // 終了時処理
                Debug.Log("Dispose");
            });
        });

        // イベント登録
        create.Subscribe(x => {
            Debug.Log("OnNext : " + x);
        }, () => {
            Debug.Log("OnCompleted");
        });
    }

    /// <summary>
    /// ObservableStartの実行
    /// </summary>
    private void ExcuteObservableStart() {
        // 別スレッドで実行
        IObservable<Unit> start = Observable.Start(() => {
            Debug.Log("Start");
            // 1秒待つ
            Thread.Sleep(1000);
            Debug.Log("Finish");
        });

        start
            // メインスレッドに戻す
            .ObserveOnMainThread()
            .Subscribe(x => {
                Debug.Log("OnNext");
            }, () => {
                Debug.Log("OnComplete");
            });
    }

/// <summary>
/// ObservableTimerの実行
/// </summary>
private void ExcuteObservableTimer() {
    // 実時間で指定し、経過後発火
    Observable
        .Timer(TimeSpan.FromSeconds(1))
        .Subscribe(x => {
            Debug.Log("1秒経ちました");
        });

    // フレームで指定し、経過後発火
    Observable
        .TimerFrame(1)
        .Subscribe(x => {
            Debug.Log("1フレーム経ちました");
        });

    Observable
        // 停止させない限り、動き続ける
        .Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2))
        .Subscribe(x => {
            Debug.Log("1秒経ちました、停止されるまで実行します。");
        });
}
}
