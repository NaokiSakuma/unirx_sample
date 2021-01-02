using UnityEngine;
using UniRx;
using System;

public class SubjectsExample : MonoBehaviour {
    [SerializeField]
    private SubjectStatus _status;
    private enum SubjectStatus {
        NONE,
        SUBJECT,
        BEHAVIOR_SUBJECT,
        REPLAY_SUBJECT,
        ASYNC_SUBJECT,
    }

    void OnValidate() {
        switch(_status) {
            case SubjectStatus.SUBJECT:
                ExcuteSubject();
                break;
            case SubjectStatus.BEHAVIOR_SUBJECT:
                ExcuteBehaviorSubject();
                break;
            case SubjectStatus.REPLAY_SUBJECT:
                ExcuteReplaySubject();
                break;
            case SubjectStatus.ASYNC_SUBJECT:
                ExcuteAsyncSubject();
                break;
        }
    }

    /// <summary>
    /// Subject
    /// OnNextで発行
    /// </summary>
    private void ExcuteSubject() {
        // イベント発行
        Subject<int> subject = new Subject<int>();
        // イベント登録
        subject.Subscribe(x => {
            Debug.Log("OnNext : " + x);
        }, () => {
            Debug.Log("OnCompleted");
        });
        // イベント実行
        subject.OnNext(1);
        subject.OnNext(10);
        subject.OnCompleted();
    }

    /// <summary>
    /// BehaviorSubject
    /// OnNextで発行された最新の値をキャッシュ
    /// Subscribeでキャッシュされた値を発行
    /// </summary>
    private void ExcuteBehaviorSubject() {
        BehaviorSubject<int> subject = new BehaviorSubject<int>(0);

        IDisposable stream1 = subject.Subscribe(x => {
            Debug.Log("ストリーム1 : " + x);
        }, () => {
            Debug.Log("ストリーム1 OnCompleted");
        });
        subject.OnNext(1);
        subject.OnNext(10);

        // 最新の値のみキャッシュされているので、OnNext(10)のみ呼ばれる
        IDisposable stream2 = subject.Subscribe(x => {
            Debug.Log("ストリーム2 : " + x);
        }, () => {
            Debug.Log("ストリーム2 OnCompleted");
        });
        subject.OnCompleted();
        // OnCompleted後なので呼ばれない
        subject.OnNext(100);

        // 最新の値のみキャッシュされているので、OnCompletedのみ呼ばれる
        IDisposable stream3 = subject.Subscribe(x => {
            Debug.Log("ストリーム3 : " + x);
        }, () => {
            Debug.Log("ストリーム3 OnCompleted");
        });
    }

    /// <summary>
    /// ReplaySubject
    /// OnNextで発行された値を全てキャッシュ
    /// Subscribeでキャッシュされた値を全て発行
    /// </summary>
    private void ExcuteReplaySubject() {
        ReplaySubject<int> subject = new ReplaySubject<int>();
        subject.OnNext(1);

        IDisposable stream1 = subject.Subscribe(x => {
            Debug.Log("ストリーム1 : " + x);
        }, () => {
            Debug.Log("ストリーム1 OnCompleted");
        });
        subject.OnNext(10);

        // キャッシュされているので、OnNextも呼ばれる
        IDisposable stream2 = subject.Subscribe(x => {
            Debug.Log("ストリーム2 : " + x);
        }, () => {
            Debug.Log("ストリーム2 OnCompleted");
        });
        subject.OnCompleted();
        // OnCompleted後なので呼ばれない
        subject.OnNext(100);

        // キャッシュされているので、OnNextも呼ばれる
        IDisposable stream3 = subject.Subscribe(x => {
            Debug.Log("ストリーム3 : " + x);
        }, () => {
            Debug.Log("ストリーム3 OnCompleted");
        });
    }

    /// <summary>
    /// AsyncSubject
    /// OnNextで発行された最新の値をキャッシュ
    /// OnCompletedでキャッシュされた値を発行
    /// </summary>
    private void ExcuteAsyncSubject() {
        AsyncSubject<int> subject = new AsyncSubject<int>();
        subject.OnNext(1);

        // OnCompleted()が呼ばれていないので、OnNext(1)は呼ばれない
        IDisposable stream1 = subject.Subscribe(x => {
            Debug.Log("ストリーム1 : " + x);
        }, () => {
            Debug.Log("ストリーム1 OnCompleted");
        });
        subject.OnNext(10);

        IDisposable stream2 = subject.Subscribe(x => {
            Debug.Log("ストリーム2 : " + x);
        }, () => {
            Debug.Log("ストリーム2 OnCompleted");
        });
        // OnCompletedが呼ばれたので、最新のキャッシュも発行
        subject.OnCompleted();
        // OnCompleted後なので呼ばれない
        subject.OnNext(100);

        // OnNext(10)とOnCompletedを発行
        IDisposable stream3 = subject.Subscribe(x => {
            Debug.Log("ストリーム3 : " + x);
        }, () => {
            Debug.Log("ストリーム3 OnCompleted");
        });
    }
}


