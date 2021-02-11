using UnityEngine;
using UniRx;
using System;
using System.Collections;

public class ObservableFromCoroutine : MonoBehaviour {
    void Start() {
        ExcuteFromCoroutine();
    }

    /// <summary>
    /// ObservableFromCoroutineの実行
    /// </summary>
    private void ExcuteFromCoroutine() {
        Observable
            // コルーチンの実行
            .FromCoroutine<int>(x => TimerCoroutine(x, 3))
            .Subscribe(x => {
                Debug.Log("Timer : " + x);
            }, () => {
                Debug.Log("OnCompleted");
            });
    }

    /// <summary>
    /// タイマー
    /// </summary>
    /// <param name="observer"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    private IEnumerator TimerCoroutine(IObserver<int> observer, int count) {
        int currentCount = count;
        while (currentCount > 0) {
            observer.OnNext(currentCount--);
            yield return new WaitForSeconds(1);
        }
        observer.OnNext(0);
        observer.OnCompleted();
    }
}
