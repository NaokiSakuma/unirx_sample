using UnityEngine;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;

public class UniTaskAwaiterUnityCallback : MonoBehaviour {

    [SerializeField]
    private UniTaskLogObject _logObject;

    private UniTaskLogObject _createdLogObject;

    void Start() {
        CreateGameObject();
        UnityCallbackAwaiterMethods();
    }

    /// <summary>
    /// ログ出し用オブジェクトの生成
    /// </summary>
    private void CreateGameObject() {
        _createdLogObject = null;
        _createdLogObject = Instantiate(_logObject, parent: transform);
    }

    /// <summary>
    /// AwaiterのUnityCallbackのメソッド群
    /// </summary>
    private void UnityCallbackAwaiterMethods() {
        AwaitAwake();
        AwaitStart();
        AwaitOnDestroy();
    }

    /// <summary>
    /// Awakeを待つ
    /// </summary>
    /// <returns></returns>
    private async void AwaitAwake() {
        await _createdLogObject.AwakeAsync();
        Debug.LogError("AwaitAwake");
    }

    /// <summary>
    /// Startを待つ
    /// </summary>
    /// <returns></returns>
    private async void AwaitStart() {
        await _createdLogObject.StartAsync();
        Debug.LogError("StartAwake");

    }

    /// <summary>
    /// OnDestroyを待つ
    /// </summary>
    /// <returns></returns>
    private async void AwaitOnDestroy() {
        await _createdLogObject.OnDestroyAsync();
        Debug.LogError("OnDestroyAwake");
    }
}
