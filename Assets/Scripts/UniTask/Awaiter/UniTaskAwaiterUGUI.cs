using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;

public class UniTaskAwaiterUGUI : MonoBehaviour {

    [SerializeField]
    private Button _button;

    [SerializeField]
    private Toggle _toggle;

    [SerializeField]
    private InputField _inputField;

    [SerializeField]
    private Slider _slider;

    void Start() {
        UGUIAwaiterMethods();
    }

    /// <summary>
    /// uGUIのAwaiter
    /// </summary>
    private void UGUIAwaiterMethods() {
        // CancellationTokenの取得
        CancellationToken token = this.GetCancellationTokenOnDestroy();
        AwaitButton(token);
        AwaitToggle(token);
        AwaitInputField(token);
        AwaitSlider(token);
    }

    /// <summary>
    /// ボタンが押されるのを待つ
    /// </summary>
    /// <returns></returns>
    private async void AwaitButton(CancellationToken token) {
        IAsyncClickEventHandler handler = _button.GetAsyncClickEventHandler(token);
        await handler.OnClickAsync();
        Debug.Log("onClickButton");
    }

    /// <summary>
    /// トグルの値が変更されるのを待つ
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    private async void AwaitToggle(CancellationToken token) {
        IAsyncValueChangedEventHandler<bool> handler = _toggle.GetAsyncValueChangedEventHandler(token);
        bool isOn = await handler.OnValueChangedAsync();
        Debug.Log("Toggle State : " + isOn);
    }

    /// <summary>
    /// InputFieldが入力されるのを待つ
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    private async void AwaitInputField(CancellationToken token) {
        IAsyncEndEditEventHandler<string> handler = _inputField.GetAsyncEndEditEventHandler(token);
        string input = await handler.OnEndEditAsync();
        Debug.Log("InputField入力文字 : " + input);
    }

    /// <summary>
    /// Sliderの値が変更されるのを待つ
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    private async void AwaitSlider(CancellationToken token) {
        IAsyncValueChangedEventHandler<float> handler = _slider.GetAsyncValueChangedEventHandler(token);
        float value = await handler.OnValueChangedAsync();
        Debug.Log("Slider Value : " + value);
    }
}
