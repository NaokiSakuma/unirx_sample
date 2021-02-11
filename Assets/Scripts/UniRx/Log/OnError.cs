using UnityEngine;
using UniRx;
using System;

public class OnError : MonoBehaviour {
    void Start() {
        OnErrorParse();
        OnErrorRetry();
    }

    /// <summary>
    /// Parseを失敗させて、OnErrorを呼ぶ
    /// </summary>
    private void OnErrorParse() {
        // イベント発行
        Subject<string> OnErrorSubject = new Subject<string>();
        // イベント登録
        OnErrorSubject
            // 値を変換する
            .Select(str => int.Parse(str))
            .Subscribe(x => {
                // OnNext
                Debug.Log("処理成功 : " + x);
            }, err => {
                // OnError
                Debug.Log("例外処理 : " + err);
            });

        // イベント実行
        OnErrorSubject.OnNext("1");
        OnErrorSubject.OnNext("2");
        OnErrorSubject.OnNext("Hoge");
        // ストリームの購読が中止される
        OnErrorSubject.OnNext("4");
        OnErrorSubject.OnNext("5");
    }

    /// <summary>
    /// OnErrorが呼ばれた場合、再購読する
    /// </summary>
    private void OnErrorRetry() {
        // イベント発行
        Subject<string> OnErrorSubject = new Subject<string>();
        // イベント登録
        OnErrorSubject
            // 値を変換する
            .Select(str => int.Parse(str))
            // エラー処理後、Subscribeし直す
            .OnErrorRetry((FormatException err) => {
                Debug.Log("例外が発生したので、再購読します");
            })
            .Subscribe(x => {
                // OnNext
                Debug.Log("処理成功 : " + x);
            }, err => {
                // OnError
                Debug.Log("例外処理 : " + err);
            });

        // イベント実行
        OnErrorSubject.OnNext("1");
        OnErrorSubject.OnNext("2");
        OnErrorSubject.OnNext("Hoge");
        OnErrorSubject.OnNext("4");
        OnErrorSubject.OnNext("5");
    }
}
