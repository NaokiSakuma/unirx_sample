using UnityEngine;
using UniRx;
using System;

public class Dispose : MonoBehaviour {
    void Start() {
        DisposeTest();
        DisposeSpecificStream();
    }

    /// <summary>
    /// Disposeの呼び出し
    /// </summary>
    private void DisposeTest() {
        // イベント発行
        Subject<int> subject = new Subject<int>();
        // 保持
        IDisposable dispose = subject.Subscribe(x => {
            Debug.Log("OnNext : " + x);
        }, () => {
            Debug.Log("OnCompleted");
        });

        // イベント実行
        subject.OnNext(1);
        subject.OnNext(10);
        // イベント購読終了
        dispose.Dispose();
        // 呼ばれない
        subject.OnNext(100);
        subject.OnNext(1000);
        // OnCompletedも呼ばれない
        subject.OnCompleted();
    }

    /// <summary>
    /// 特定のストリームのみ購読停止させる
    /// </summary>
    private void DisposeSpecificStream() {
        // イベント発行
        Subject<int> subject = new Subject<int>();
        // 保持
        IDisposable dispose1 = subject.Subscribe(x => {
            Debug.Log("ストリーム1 : " + x);
        }, () => {
            Debug.Log("ストリーム1 OnCompleted");
        });
        IDisposable dispose2 = subject.Subscribe(x => {
            Debug.Log("ストリーム2 : " + x);
        }, () => {
            Debug.Log("ストリーム2 OnCompleted");
        });

        // イベント実行
        subject.OnNext(1);
        subject.OnNext(10);
        // ストリーム1のみイベント購読終了
        dispose1.Dispose();
        subject.OnNext(100);
        subject.OnCompleted();
    }
}
