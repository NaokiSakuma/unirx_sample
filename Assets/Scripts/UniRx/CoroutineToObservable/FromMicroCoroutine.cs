using UnityEngine;
using UniRx;
using System.Collections;

public class FromMicroCoroutine : MonoBehaviour {
    void Start() {
        ExcuteFromMicroCoroutine();
    }

    /// <summary>
    /// FromMicroCoroutineの実行
    /// </summary>
    private void ExcuteFromMicroCoroutine() {
        Observable
            .FromMicroCoroutine(SimpleCoroutine)
            .Subscribe(x => {
                Debug.Log("OnNext");
            }, () => {
                Debug.Log("OnCompleted");
            });
    }

    /// <summary>
    /// nullを返すだけのコルーチン
    /// </summary>
    /// <returns></returns>
    private IEnumerator SimpleCoroutine() {
        yield return null;
    }
}