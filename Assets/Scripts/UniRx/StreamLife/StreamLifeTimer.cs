using UnityEngine;
using UniRx;
using System;
using System.Collections;

/// <summary>
/// カウントダウンクラス
/// </summary>
public class StreamLifeTimer : MonoBehaviour {
    [SerializeField]
    private int _timeLeft = 3;
    private Subject<int> _timerSubject = new Subject<int>();
    public IObservable<int> onTimeChanged {
        get { return _timerSubject; }
    }

    void Awake() {
        StartCoroutine(TimerCoroutine());
        TimerSubscribe();
    }

    /// <summary>
    /// カウントダウンする
    /// </summary>
    /// <returns></returns>
    private IEnumerator TimerCoroutine() {
        yield return null;
        int time = _timeLeft;
        while (time >= 0) {
            _timerSubject.OnNext(time--);
            // 1秒間コルーチンの実行を待つ
            yield return new WaitForSeconds(1);
        }
        // timerが0になったら完了通知
        _timerSubject.OnCompleted();
    }

    /// <summary>
    /// 現在のカウントダウンを表示
    /// </summary>
    private void TimerSubscribe() {
        _timerSubject.Subscribe(x => {
            Debug.Log("NowCount : " + x);
        }, () => {
            Debug.Log("Complete");
        });
    }
}
