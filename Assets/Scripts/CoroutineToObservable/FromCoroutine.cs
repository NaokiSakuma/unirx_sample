using UnityEngine;
using UniRx;
using System;
using System.Collections;
using System.Threading;

public class FromCoroutine : MonoBehaviour {
    [SerializeField]
    private bool _publishEveryYield;

    void Start() {
        // ExcuteFromCoroutine();
        // ExcuteMultiCoroutine();
        ExcuteDisposeCoroutine();
    }

    /// <summary>
    /// FromCoroutineの実行
    /// </summary>
    private void ExcuteFromCoroutine() {
        // コルーチンの終了タイミングを待つ
        Observable
            .FromCoroutine(LogCoroutine, _publishEveryYield)
            .Subscribe(x => {
                Debug.Log("OnNext");
            }, () => {
                Debug.Log("OnCompleted");
            }).AddTo(gameObject);
    }

    /// <summary>
    /// ログを出すコルーチン
    /// </summary>
    /// <returns></returns>
    private IEnumerator LogCoroutine() {
        Debug.Log("publishEveryYield : " + _publishEveryYield);
        Debug.Log("Coroutine Start");
        // publishEveryYieldがtrueの場合、OnNextを発行
        yield return null;
        yield return null;
        yield return null;
        Debug.Log("Coroutine Finish");
    }

    /// <summary>
    /// 複数回のSubscribe
    /// </summary>
    private void ExcuteMultiCoroutine() {
        IObservable<Unit> multi = Observable.FromCoroutine(LogCoroutine, _publishEveryYield);
        multi.Subscribe(x => {
            Debug.Log("1回目のSubscribe");
        });
        multi.Subscribe(x => {
            Debug.Log("2回目のSubscribe");
        });
        multi.Subscribe(x => {
            Debug.Log("3回目のSubscribe");
        });
    }

    /// <summary>
    /// FromCoroutineのDisposeの実行
    /// </summary>
    private void ExcuteDisposeCoroutine() {
        IDisposable dispose = Observable
            .FromCoroutine(x => DisposeCoroutine(x), _publishEveryYield)
            .Subscribe(x => {
                Debug.Log("OnNext");
            }, () => {
                Debug.Log("OnCompleted");
            }).AddTo(gameObject);
        dispose.Dispose();
    }

    /// <summary>
    /// Disposeするコルーチン
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    private IEnumerator DisposeCoroutine(CancellationToken token) {
        Debug.Log("Coroutine Start");
        // 例外エラーを投げる
        token.ThrowIfCancellationRequested();
        yield return null;
        Debug.Log("Coroutine Finish");
    }
}