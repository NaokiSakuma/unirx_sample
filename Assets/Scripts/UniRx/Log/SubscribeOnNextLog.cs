using UnityEngine;
using UniRx;
using System;

/// <summary>
/// SubscribeとOnNextをLogで確認するクラス
/// </summary>
public class SubscribeOnNextLog : MonoBehaviour {
    // イベント発行
    private Subject<string> logSubject = new Subject<string>();

    void Start() {
        // イベント実行(ログに出ない)
        logSubject.OnNext("foo");

        // イベント登録
        logSubject.Subscribe(message => {
            Debug.Log("1回目のSubscribe : " + message);
        });
        logSubject.Subscribe(message => {
            Debug.Log("2回目のSubscribe : " + message);
        });
        logSubject.Subscribe(message => {
            Debug.Log("3回目のSubscribe : " + message);
        });

        // イベント実行
        logSubject.OnNext("Hoge");
        logSubject.OnNext("Fuga");
    }
}

