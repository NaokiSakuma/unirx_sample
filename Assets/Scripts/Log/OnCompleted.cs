using UnityEngine;
using UniRx;
using System;

public class OnCompleted : MonoBehaviour {
    void Start() {
        OnCompletedInt();
    }

    /// <summary>
    /// OnCompletedの呼び出し
    /// </summary>
    private void OnCompletedInt() {
        // イベント発行
        Subject<int> OnCompletedSubject = new Subject<int>();
        // イベント登録
        OnCompletedSubject.Subscribe(x => {
            // OnNext
            Debug.Log("処理成功 : " + x);
        }, () => {
            // OnCompleted
            Debug.Log("Complete");
        });

        // イベント実行
        OnCompletedSubject.OnNext(1);
        OnCompletedSubject.OnNext(10);
        OnCompletedSubject.OnCompleted();
        // 呼ばれない
        OnCompletedSubject.OnNext(100);
    }
}
