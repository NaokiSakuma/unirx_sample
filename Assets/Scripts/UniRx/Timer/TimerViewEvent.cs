using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 時間表示クラス
/// </summary>
public class TimerViewEvent : MonoBehaviour {
    [SerializeField]
    private TimeCounterEvent _timeCounter;
    [SerializeField]
    private Text _counterText;

    void Start() {
        // 通知が来たら、Textをtimeの値で更新
        _timeCounter.OnTimeChanged += (time) => {
            _counterText.text = "event : " + time.ToString();
        };
    }
}
