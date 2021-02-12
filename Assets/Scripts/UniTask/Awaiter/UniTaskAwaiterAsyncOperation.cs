using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public class UniTaskAwaiterAsyncOperation : MonoBehaviour {
    void Start() {
        // WaitSceneLoad();
        WaitSceneLoadProgress();
    }

    /// <summary>
    /// AsyncOperationのAwaiter
    /// </summary>
    /// <returns></returns>
    private async void WaitSceneLoad() {
        await SceneManager.LoadSceneAsync("Scenes/CoroutineToObservable");
        Debug.Log("Sceneの読み込み成功");
    }

    /// <summary>
    /// 進捗を表示
    /// </summary>
    /// <returns></returns>
    private async void WaitSceneLoadProgress() {
        await SceneManager
                .LoadSceneAsync("Scenes/CoroutineToObservable")
                .ToUniTask(Progress.Create<float>(x => {
                    Debug.Log("進捗 : " + x);
                }));
        Debug.LogError("Sceneの読み込み成功");
    }
}
