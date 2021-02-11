using System.Collections;
using UnityEngine;

/// <summary>
/// カウントダウンクラス
/// </summary>
public class TimeCounterEvent : MonoBehaviour {
    // イベントハンドラ
    public delegate void TimerEventHandler(int time);
    // イベント
    public event TimerEventHandler OnTimeChanged;

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
            // イベント通知
            OnTimeChanged(time);
            yield return new WaitForSeconds(1);
        }
    }
}
