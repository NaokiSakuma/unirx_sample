using UnityEngine;
using UniRx;
using System;

public class OnNext : MonoBehaviour {
    void Start() {
        OnNextInt();
        OnNextUint();
    }

    /// <summary>
    /// OnNextのintの呼び出し
    /// </summary>
    private void OnNextInt() {
        // イベント発行
        Subject<int> onNextSubject = new Subject<int>();
        // イベント登録
        onNextSubject.Subscribe(x => {
            Debug.Log("渡された値 : " + x);
        });

        // イベント実行
        onNextSubject.OnNext(0);
        onNextSubject.OnNext(10);
        onNextSubject.OnNext(100);
        onNextSubject.OnNext(1000);
    }

    /// <summary>
    /// Unit型の呼び出し
    /// </summary>
    private void OnNextUint() {
        // イベント発行
        Subject<Unit> unitSubject = new Subject<Unit>();
        // イベント登録
        unitSubject.Subscribe(x => {
            Debug.Log("渡された値 : " + x);
        });

        // イベント実行
        unitSubject.OnNext(Unit.Default);
    }
}

