using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;

/// <summary>
/// 時間表示クラス
/// </summary>
public class TimerViewUniRx : MonoBehaviour {
    [SerializeField]
    private TimeCounterUniRx _timeCounter;
    [SerializeField]
    private Text _counterText;

    void Awake() {
        // 通知が来たら、Textをtimeの値で更新
        _timeCounter.OnTimeChanged.Subscribe(time => {
            _counterText.text = "UniRx : " + time.ToString();
        });
    }
}
