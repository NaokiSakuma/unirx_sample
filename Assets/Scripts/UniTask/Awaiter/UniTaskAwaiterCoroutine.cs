using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections;

public class UniTaskAwaiterCoroutine : MonoBehaviour {
    void Start() {
        AwaitCoroutine();
        OnlyCoroutine();
    }

    /// <summary>
    /// コルーチンのAwaiter
    /// </summary>
    /// <returns></returns>
    private async void AwaitCoroutine() {
        await WaitTime(1, "Awaiterでの1回目の待機");
        await WaitTime(2, "Awaiterでの2回目の待機");
        await WaitTime(3, "Awaiterでの3回目の待機");
    }

    /// <summary>
    /// 指定時間待つコルーチン
    /// </summary>
    /// <param name="time"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    private IEnumerator WaitTime(float time, string message) {
        yield return new WaitForSeconds(time);
        Debug.Log("Finished. Message is : " + message);
    }

    /// <summary>
    /// コルーチンでの呼び出し
    /// </summary>
    private void OnlyCoroutine() {
        StartCoroutine(WaitTime(1, "コルーチンでの1回目の待機"));
        StartCoroutine(WaitTime(2, "コルーチンでの2回目の待機"));
        StartCoroutine(WaitTime(3, "コルーチンでの3回目の待機"));
    }

}
