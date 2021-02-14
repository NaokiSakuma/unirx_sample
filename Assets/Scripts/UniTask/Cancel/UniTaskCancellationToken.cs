using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;

public class UniTaskCancellationToken : MonoBehaviour {

    [SerializeField]
    private Button _button;

    void Start() {
        // CreateCancellationToken();
        LoopCancelButton();
    }

    /// <summary>
    /// CancellationTokenの生成
    /// </summary>
    private void CreateCancellationToken() {
        // tokenの生成
        CancellationTokenSource source = new CancellationTokenSource();
        CancellationToken token = source.Token;
        // キャンセルにさせる
        source.Cancel();

        // キャンセル状態の時
        if (token.IsCancellationRequested) {
            Debug.Log("Cancel");
        }
        // キャンセル状態なら、OperationCanceledExceptionを投げる
        token.ThrowIfCancellationRequested();
    }

    /// <summary>
    /// ループを止めるボタン
    /// </summary>
    private void LoopCancelButton() {
        CancellationTokenSource source = new CancellationTokenSource();
        CancellationToken token = source.Token;
        // ボタンが押された初回のみOnNext
        _button
            .OnClickAsObservable()
            .First()
            .Subscribe(x => {
                // 押されたらUniTaskをキャンセル
                Debug.Log("ボタンが押されました");
                source.Cancel();
                token.ThrowIfCancellationRequested();
        });
        LoopDelay(token).Forget();
    }

    /// <summary>
    /// Delayをループする
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    private async UniTask LoopDelay(CancellationToken token) {
        int count = 0;
        while (true) {
            count++;
            // 引数でCancellationTokenを指定
            await UniTask.Delay(1000, cancellationToken:token);
            Debug.Log(count + "回目のDelay");
        }
    }
}
