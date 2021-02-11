using UnityEngine;
using UniRx;
using System;
using System.Collections;
using System.Threading;

public class FromCoroutineT : MonoBehaviour {

    [SerializeField]
    private bool _isPause;
    void Start() {
        ExcuteFromCoroutineT();
    }

    /// <summary>
    /// FromCoroutine<T>の実行
    /// </summary>
    private void ExcuteFromCoroutineT() {
        Observable
            .FromCoroutine<int>(x => Counter(x))
            .Subscribe(x => {
                Debug.Log(x);
            }).AddTo(gameObject);
    }

    /// <summary>
    /// カウンター
    /// </summary>
    /// <param name="observer"></param>
    /// <returns></returns>
    private IEnumerator Counter(IObserver<int> observer) {
        int current = 0;
        float deltaTime = 0;

        while(true) {
            yield return null;
            if (_isPause) {
                continue;
            }
            deltaTime += Time.deltaTime;
            if (deltaTime < 1.0) {
                continue;
            }
            // 1秒ごとにOnNext発行
            int integerPart = (int)Mathf.Floor(deltaTime);
            current += integerPart;
            deltaTime -= integerPart;
            observer.OnNext(current);
        }
    }
}
