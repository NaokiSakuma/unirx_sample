using UnityEngine;
using UniRx;
using System.Collections;

public class SelectManyCroutine : MonoBehaviour {
    void Start() {
        ExcuteSelectManyCoroutine();
    }

    /// <summary>
    /// SelectManyの実行
    /// </summary>
    private void ExcuteSelectManyCoroutine() {
        Observable
            .FromCoroutine(CoroutineA)
            // CoroutineAの終了を待ってから、CoroutineBを起動
            .SelectMany(CoroutineB)
            // CoroutineBの終了を待ってから、CoroutineCを起動
            .SelectMany(CoroutineC)
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
