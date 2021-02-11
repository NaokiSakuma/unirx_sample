using UnityEngine;
using UniRx;
using System;

public class HotColdBranch : MonoBehaviour {
    void Start() {
        // ColdBranch();
        HotBranch();
    }

    /// <summary>
    /// Coldは分岐できない
    /// </summary>
    private void ColdBranch() {
        // 1~3を発行するストリーム
        IObservable<int> stream = Observable
                                    .Range(1,3)
                                    .Select(x => {
                                        Debug.Log("Cold : " + x);
                                        return x;
                                    });

        stream.Subscribe();
        stream.Subscribe();
    }

    /// <summary>
    /// Hotは分岐できる
    /// </summary>
    private void HotBranch() {
        // 1~3を発行するストリーム
        IConnectableObservable<int> stream = Observable
                                                .Range(1,3)
                                                .Select(x => {
                                                    Debug.Log("Hot : " + x);
                                                    return x;
                                                })
                                                // Hot変換オペレータ
                                                .Publish();
        // ストリームの稼働開始
        stream.Connect();
        stream.Subscribe();
        stream.Subscribe();
    }

}
