using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class OtherStream : MonoBehaviour {

    [SerializeField]
    private Slider _slider;

    void Start() {
        ExcuteNextFrame();
        ExcuteEveryUpdate();
        ExcuteObserveEveryValueChanged();
    }

    /// <summary>
    /// NextFrameの実行
    /// </summary>
    private void ExcuteNextFrame() {
        // 次のフレームで実行
        Observable
            .NextFrame()
            .Subscribe(x => {
                Debug.Log("Next Frame");
            });
    }

    /// <summary>
    /// EveryUpdateの実行
    /// </summary>
    private void ExcuteEveryUpdate() {
        // Updateのタイミングを通知
        Observable
            .EveryUpdate()
            .Subscribe(x => {
                Debug.Log("Frame : " + x);
            });
    }

    /// <summary>
    /// ObserveEveryValueChangedの実行
    /// </summary>
    private void ExcuteObserveEveryValueChanged() {
        _slider
            .ObserveEveryValueChanged(x => x.value)
            .Subscribe(x => {
                Debug.Log("Slider Value : " + x);
            });
    }
}
