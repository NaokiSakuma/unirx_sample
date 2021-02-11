using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System.Collections;

public class ToYieldInstruction : MonoBehaviour {
    [SerializeField]
    private Button _buttonA;
    [SerializeField]
    private Button _buttonB;

    void Start() {
        ExcuteCoroutine();
    }

    private void ExcuteCoroutine() {
        StartCoroutine(BothButtonClick());
    }

    private IEnumerator BothButtonClick() {
        Debug.Log("ボタンAが押されるのを待っています");
        yield return _buttonA
            .OnClickAsObservable()
            .FirstOrDefault()
            .ToYieldInstruction();

        Debug.Log("ボタンBが押されるのを待っています");
        yield return _buttonB
            .OnClickAsObservable()
            .FirstOrDefault()
            .ToYieldInstruction();

        Debug.Log("両方のボタンが押されました");
    }
}