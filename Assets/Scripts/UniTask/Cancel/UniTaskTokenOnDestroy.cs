using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System.Threading;
using Cysharp.Threading.Tasks;

public class UniTaskTokenOnDestroy : MonoBehaviour {

    [SerializeField]
    private Button _button;
    void Start() {
        TokenOnDestroy();
    }

    /// <summary>
    /// Destory時にキャンセルする
    /// </summary>
    private void TokenOnDestroy() {
        // GameObjectが破棄されるとキャンセル状態にしてくれるtoken
        CancellationToken token = this.GetCancellationTokenOnDestroy();
        LoopDelay(token).Forget();
        _button
            .OnClickAsObservable()
            .Subscribe((x) => {
                Destroy(gameObject);
        });
    }

    /// <summary>
    /// Delayをループする
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    private async UniTask LoopDelay(CancellationToken token) {
        int count = 0;
        while (!token.IsCancellationRequested) {
            count++;
            // 引数でCancellationTokenを指定
            await UniTask.Delay(1000);
            Debug.Log(count + "回目のDelay");
        }
    }
}
