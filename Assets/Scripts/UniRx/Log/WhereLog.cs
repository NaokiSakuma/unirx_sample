using UnityEngine;
using UniRx;
using System;

public class WhereLog : MonoBehaviour {
    private Subject<string> logSubject = new Subject<string>();

    void Start() {
        // イベント登録
        logSubject
            .Where(message => message == "敵")
            .Subscribe(message => {
                Debug.Log(message + "と衝突");
            });

        // イベント実行
        logSubject.OnNext("敵");
        logSubject.OnNext("味方");
        logSubject.OnNext("敵");
        logSubject.OnNext("味方");
    }
}
