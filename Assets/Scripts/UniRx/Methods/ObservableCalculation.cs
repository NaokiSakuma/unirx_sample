using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using System.Linq;
using System.Collections.Generic;

public class ObservableCalculation : MonoBehaviour {
    private enum Methods {
        SCAN_LIST,
        SCAN_ANY_KEY_DOWN,
        BUFFER_SINGLE,
        BUFFER_DOUBLE,
        BUFFER_DOUBLE_CLICK,
        PAIRWISE,
    }

    [SerializeField]
    private Methods _methodStatus;

    private bool _isInitialize = false;

    void Start() {
        _isInitialize = true;
    }

    void OnValidate() {
        ExcuteMethods();
    }

    /// <summary>
    /// 前回のメッセージの値と今回の値を加算する
    /// </summary>
    private void ExcuteScanList() {
        Observable
            .Range(1, 5)
            .Scan((a, b) => a + b)
            .Subscribe(x => {
                Debug.Log(string.Format("ScanList onNext : " + x));
            }, () => {
                Debug.Log(string.Format("ScanList onCompleted"));
            });
    }

    /// <summary>
    /// 何かキーが押されるたびにカウントされる
    /// </summary>
    private void ExcuteScanOnAnyKeyDown()
    {
        Observable
            .EveryUpdate()
            .Where(_ => Input.anyKeyDown)
            .Select(_ => 1)
            .Scan((a, b) => a + b)
            .Subscribe(x => {
                Debug.Log(string.Format("ScanOnAnyKeyDown onNext : " + x));
            }, () => {
                Debug.Log(string.Format("ScanOnAnyKeyDown onCompleted"));
            });
    }

    /// <summary>
    /// メッセージを複数にまとめる
    /// 引数1つ
    /// </summary>
    private void ExcuteBufferSingle()
    {
        Observable
            .Range(1, 5)
            .Select(x => x.ToString())
            // Buffer(2, 2)と同じ
            .Buffer(2)
            .Subscribe(x => {
                Debug.Log("Buffer : " + x.Aggregate<string>((sum, n) => sum.ToString() + ", " + n.ToString()));
            }, () => {
                Debug.Log(string.Format("Buffer onCompleted"));
            });
    }

    /// <summary>
    /// メッセージを複数にまとめる
    /// 引数2つ
    /// </summary>
    private void ExcuteBufferDouble()
    {
        Observable
            .Range(1, 5)
            .Select(x => x.ToString())
            .Buffer(3, 2)
            .Subscribe(x => {
                Debug.Log("Buffer : " + x.Aggregate<string>((sum, n) => sum.ToString() + ", " + n.ToString()));
            }, () => {
                Debug.Log(string.Format("Buffer onCompleted"));
            });
    }

    /// <summary>
    /// 指定したObservableにメッセージが発行されるまでメッセージをまとめる
    /// ダブルクリック検知
    /// </summary>
    private void ExcuteBufferDoubleClick()
    {
        // クリック検知
        IObservable<Unit> clickObservable = this.UpdateAsObservable()
                                                .Where(_ => Input.GetMouseButtonDown(0));
        clickObservable
            // 200ms内のクリックをまとめる
            .Buffer(clickObservable.Throttle(TimeSpan.FromMilliseconds(200)))
            .Where(x=> x.Count == 2)
            .Subscribe(x => {
                Debug.Log("Buffer : Double Click");
            }, () => {
                Debug.Log(string.Format("Buffer onCompleted"));
            });
    }

    /// <summary>
    /// 2つのメッセージを1つにまとめる
    /// Buffer(2, 1)との違いは必ず2つでセットになること
    /// </summary>
    private void ExcutePairwise()
    {
        Observable
            .Range(1, 5)
            // 2つのメッセージを1つにまとめる
            .Pairwise()
            .Subscribe(x => {
                Debug.Log(string.Format("Pairwise : {0}, {1}", x.Previous, x.Current));
            }, () => {
                Debug.Log(string.Format("Pairwise : onCompleted"));
            });

        // 比較用
        Observable
            .Range(1, 5)
            .Select(x => x.ToString())
            .Buffer(2, 1)
            .Subscribe(x => {
                Debug.Log("Buffer    : " + x.Aggregate<string>((sum, n) => sum.ToString() + ", " + n.ToString()));
            }, () => {
                Debug.Log(string.Format("Buffer    : onCompleted"));
            });

    }

    private void ExcuteMethods() {
        if (!_isInitialize) {
            // return;
        }
        switch (_methodStatus) {
            case Methods.SCAN_LIST :
                ExcuteScanList();
            break;
            case Methods.SCAN_ANY_KEY_DOWN :
                ExcuteScanOnAnyKeyDown();
            break;
            case Methods.BUFFER_SINGLE :
                ExcuteBufferSingle();
            break;
            case Methods.BUFFER_DOUBLE :
                ExcuteBufferDouble();
            break;
            case Methods.BUFFER_DOUBLE_CLICK :
                ExcuteBufferDoubleClick();
            break;
            case Methods.PAIRWISE :
                ExcutePairwise();
            break;
        }
    }

}
