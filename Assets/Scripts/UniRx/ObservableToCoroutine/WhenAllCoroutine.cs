using UnityEngine;
using UniRx;
using System.Collections;

public class WhenAllCoroutine : MonoBehaviour {
    void Start() {
        ExcuteSelectManyCoroutine();
    }

    /// <summary>
    /// SelectManyの実行
    /// </summary>
    private void ExcuteSelectManyCoroutine() {
        Observable
            // 並列でコルーチンを起動する
            .WhenAll(Observable.FromCoroutine(CoroutineA),
                     Observable.FromCoroutine(CoroutineB),
                     Observable.FromCoroutine(CoroutineC))
            .Subscribe(x => {
                Debug.Log("全てのコルーチンが終了しました");
            });
    }

    private IEnumerator CoroutineA() {
        Debug.Log("コルーチンA開始");
        yield return new WaitForSeconds(1);
        Debug.Log("コルーチンA終了");
    }

    private IEnumerator CoroutineB() {
        Debug.Log("コルーチンB開始");
        yield return new WaitForSeconds(2);
        Debug.Log("コルーチンB終了");
    }

    private IEnumerator CoroutineC() {
        Debug.Log("コルーチンC開始");
        yield return new WaitForSeconds(3);
        Debug.Log("コルーチンC終了");
    }
}
