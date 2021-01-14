using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class UpdateStream : MonoBehaviour {
    void Start() {
        // ExcuteUpdateAsObservable();
        ExcuteEveryUpdate();
    }

    /// <summary>
    /// UpdateAsObservableの実行
    /// </summary>
    private void ExcuteUpdateAsObservable() {
        this.UpdateAsObservable()
            .Subscribe(x => {
                Debug.Log("OnNext");
            }, () => {
                Debug.Log("OnCompleted");
            });

        // Destroy時に呼ばれる
        this.OnDestroyAsObservable()
            .Subscribe(x => {
                Debug.Log("Destroy");
            });

        // 1秒後に破棄
        Destroy(gameObject, 1.0f);
    }

    /// <summary>
    /// EveryUpdateの実行
    /// </summary>
    private void ExcuteEveryUpdate() {
        Observable
            .EveryUpdate()
            .Subscribe(x => {
                Debug.Log("Update Frame : " + x);
            });
    }

}
