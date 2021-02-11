using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

public class AsynchronousSimple : MonoBehaviour {
    void Start() {
        // LogCoroutine();
        // TaskTimer();
        UniTaskTimer();
    }

    private void LogCoroutine() {
        Debug.Log("コルーチン開始");
        StartCoroutine(CoroutineTimer(1));
        Debug.Log("コルーチン終了");
    }

    private IEnumerator CoroutineTimer(int time) {
        yield return new WaitForSeconds(time);
        Debug.Log("指定時間経過");
    }

    private async Task TaskTimer() {
        Debug.Log("タスク開始");
        // 1000ミリ秒待つ
        await Task.Delay(1000);
        Debug.Log("タスク終了");
    }

    private async UniTask UniTaskTimer() {
        Debug.Log("UniTask開始");
        // 1000ミリ秒待つ
        await UniTask.Delay(1000);
        Debug.Log("UniTask終了");
    }
}
