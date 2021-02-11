using UnityEngine;
using UniRx;
using System;
using System.Collections;

public class ObservableToCoroutine : MonoBehaviour {
    void Start() {
        ExcuteTimerCoroutine();
    }

    /// <summary>
    /// タイマーコルーチンの実行
    /// </summary>
    private void ExcuteTimerCoroutine() {
        Observable
            .FromCoroutine(TimerCoroutine)
            .Subscribe(x => {
                Debug.Log("OnNext");
            }, () => {
                Debug.Log("OnCompleted");
            });
    }

    /// <summary>
    /// タイマーコルーチン
    /// </summary>
    /// <returns></returns>
    private IEnumerator TimerCoroutine() {
        Debug.Log("1秒後に終了します");
        yield return Observable
                            .Timer(TimeSpan.FromSeconds(1))
                            .ToYieldInstruction();
    }
}
