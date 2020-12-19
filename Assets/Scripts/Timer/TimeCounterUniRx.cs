using System.Collections;
using UnityEngine;
using UniRx;
using System;

/// <summary>
/// カウントダウンクラス
/// </summary>
public class TimeCounterUniRx : MonoBehaviour {
    // イベント発行のインスタンス
    public Subject<int> timerSubject = new Subject<int>();
    // イベントの購読側
    public IObservable<int> OnTimeChanged {
        get { return timerSubject; }
    }

    void Start() {
        StartCoroutine(TimerCoroutine());
    }

    /// <summary>
    /// 時間を計測する
    /// </summary>
    /// <returns></returns>
    IEnumerator TimerCoroutine() {
        int time = 100;
        while (time > 0) {
            time--;
            // イベント発行
            timerSubject.OnNext(time);
            yield return new WaitForSeconds(1);
        }
    }
}
