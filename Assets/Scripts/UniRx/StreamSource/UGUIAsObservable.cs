using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class UGUIAsObservable : MonoBehaviour {
    [SerializeField]
    private Button _button;
    [SerializeField]
    private Slider _sliderA;
    [SerializeField]
    private Slider _sliderB;

    void Start() {
        ExcuteUGUIAsObservable();
    }

    /// <summary>
    /// uGUIイベントの実行
    /// </summary>
    private void ExcuteUGUIAsObservable() {
        // ボタン押下時
        _button
            .OnClickAsObservable()
            .Subscribe(x => {
                Debug.Log("ボタンが押されました");
            });

        // Sliderの値変更時、初期値あり
        _sliderA
            .OnValueChangedAsObservable()
            .Subscribe(x => {
                Debug.Log("SliderA Value : " + x);
            });

        // Sliderの値変更時、初期値なし
        _sliderB
            .onValueChanged
            .AsObservable()
            .Subscribe(x => {
                Debug.Log("SliderB Value : " + x);
            });
    }
}
