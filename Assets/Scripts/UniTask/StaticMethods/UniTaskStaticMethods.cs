using UnityEngine;
using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class UniTaskStaticMethods : MonoBehaviour {

    void Start() {
        // ExcuteDelay();
        // ExcuteDelayFrame();
        // ExcuteYield();
        // ExcuteRun();
        // ExcuteWhenAll();
        // ExcuteWhenAny();
        // ExcuteWaitFlag();
        ExcuteWaitUntilValueChanged();
    }

    /// <summary>
    /// 指定秒数待つ
    /// </summary>
    /// <returns></returns>
    private async UniTask ExcuteDelay() {
        // 1000ミリ秒待つ
        await UniTask.Delay(1000);
        Debug.Log("1000ミリ秒経過");
        // 1秒待つ
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        Debug.Log("1秒経過");
        // FixedUpdateタイミングで1000ミリ秒待つ、デフォルトはUpdateタイミング
        await UniTask.Delay(1000, delayTiming: PlayerLoopTiming.FixedUpdate);
        Debug.Log("FixedUpdateで1000ミリ秒経過");
    }

    /// <summary>
    /// 指定フレーム待つ
    /// </summary>
    /// <returns></returns>
    private async UniTask ExcuteDelayFrame() {
        // 60フレーム待つ
        await UniTask.DelayFrame(60);
        Debug.Log("60フレーム経過");
        // FixedUpdateタイミングで60フレーム待つ、デフォルトはUpdateタイミング
        await UniTask.DelayFrame(60, delayTiming: PlayerLoopTiming.FixedUpdate);
        Debug.Log("FixedUpdateで60フレーム経過");
    }

    /// <summary>
    ///　指定タイミングで1フレーム待機
    /// </summary>
    /// <returns></returns>
    private async UniTask ExcuteYield() {
        // Updateで1フレーム待機
        await UniTask.Yield();
        // FixedUpdateで1フレーム待機
        await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
        Debug.Log("このLogはFixedUpdateのタイミングで実行される");
        await UniTask.Yield();
        Debug.Log("このLogはUpdateのタイミングで実行される");
    }

    /// <summary>
    /// スレッドを切り替える
    /// </summary>
    /// <returns></returns>
    private async UniTask ExcuteThreadPool() {
        // スレッドプールに切り替える
        await UniTask.SwitchToThreadPool();
        Debug.Log("スレッドプール上での処理");
        // メインスレッドに切り替える
        await UniTask.SwitchToMainThread();
        Debug.Log("メインスレッド上での処理");
        await UniTask.SwitchToThreadPool();
        // Yieldでメインスレッドに切り替えることもできるが、1フレーム待ってしまう
        await UniTask.Yield();
    }

    [SerializeField]
    private GameObject _prefab;

    /// <summary>
    /// 引数のデリゲートを実行
    /// </summary>
    /// <returns></returns>
    private async UniTask ExcuteRun() {
        int hoge = 1;
        int fuga = 1;
        // スレッドプール上で実行、処理後はメインスレッドへ
        await UniTask.Run(() => {
            int hogeFuga = hoge + fuga;
            Debug.Log(hogeFuga);
        });

        // Instantiateのようなスレッドプールでは実行できないものもある
        await UniTask.Run(() => {
            GameObject obj = Instantiate(_prefab, this.transform);
        });
    }

    /// <summary>
    /// 指定した全てのUniTaskが完了するまで待機
    /// </summary>
    /// <returns></returns>
    private async UniTask ExcuteWhenAll() {
        UniTask<int> task1 = UniTask.Run(() => 100);
        UniTask<string> task2 = UniTask.Run(() => "Finish");
        // 指定した全てのUniTaskの完了を待機
        // 結果をタプルで受け取ることもできる
        var (t1, t2) = await UniTask.WhenAll(task1, task2);
        Debug.Log("t1 : " + t1);
        Debug.Log("t2 : " + t2);
    }

    /// <summary>
    /// 指定したどれかのUniTaskが完了するまで待機
    /// </summary>
    /// <returns></returns>
    private async UniTask ExcuteWhenAny() {
        UniTask task1 = UniTask.Delay(3000);
        UniTask<string> task2 = UniTask.Run(() => "Finish");
        // 指定したどれかのUnitaskの完了を待機
        await UniTask.WhenAny(task1, task2);
        Debug.Log("どちらかのUnitaskが完了");

        List<UniTask<string>> listTask = new List<UniTask<string>>();
        listTask.Add(UniTask.Run(() => "No.1 Task"));
        listTask.Add(UniTask.Run(() => "No.2 Task"));
        listTask.Add(UniTask.Run(() => "No.3 Task"));
        // タスクの方が全て同じの場合、完了したタスクを直接取得することができる
        var (_, result) = await UniTask.WhenAny(listTask.ToArray());
        Debug.Log("終了したタスク : " + result);
    }

    [SerializeField]
    private bool _isWaitFlag;

    /// <summary>
    /// 指定した条件がtrue/falseになるまで待機
    /// </summary>
    /// <returns></returns>
    private async UniTask ExcuteWaitFlag() {
        _isWaitFlag = false;
        // 指定した条件がtrueになるまで待機
        await UniTask.WaitUntil(() => _isWaitFlag);
        Debug.Log("WaitUntil終了");
        // 指定した条件がfalseになるまで待機
        await UniTask.WaitWhile(() => _isWaitFlag);
        Debug.Log("WaitWhile終了");
    }

    /// <summary>
    /// 指定した値が変化するまで待機
    /// </summary>
    /// <returns></returns>
    private async UniTask ExcuteWaitUntilValueChanged() {
        // 自身の座標が変化するまで待機
        await UniTask.WaitUntilValueChanged(transform, x => x.position);
        Debug.Log("移動した");
    }

}
