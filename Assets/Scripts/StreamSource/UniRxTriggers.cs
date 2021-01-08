using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class UniRxTriggers : MonoBehaviour {
    void Start() {
        ExcuteUniRxTriggers();
    }

    /// <summary>
    /// UniRxTriggersの実行
    /// </summary>
    private void ExcuteUniRxTriggers() {
        // Updateでメッセージを発行
        this.UpdateAsObservable()
            .Subscribe(x => {
                Debug.Log("Update");
            });

        // OnCollisionEnterが呼ばれた時にメッセージを発行
        this.OnCollisionEnterAsObservable()
            .Where(x => x.gameObject.tag == "Enemy")
            .Subscribe(x => {
                Debug.Log("敵と接触");
            });

        // OnTriggerEnterが呼ばれた時にメッセージを発行
        this.OnTriggerEnterAsObservable()
            .Where(x => x.gameObject.tag == "Water")
            .Subscribe(x => {
                Debug.Log("泳ぎ判定");
            });
    }
}
