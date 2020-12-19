using UnityEngine;
using UnityEngine.UI;
using UniRx;

/// <summary>
/// プレイヤークラス
/// </summary>
public class StreamLifePlayer : MonoBehaviour {
    [SerializeField]
    private StreamLifeTimer _lifeTimer;
    private float _moveSpeed = 10.0f;

    void Start() {
        ExcuteTimer();
    }

    /// <summary>
    /// タイマーの購読
    /// </summary>
    private void ExcuteTimer() {
        _lifeTimer.onTimeChanged
            .Where(x => x == 0)
            .Subscribe(x => {
                // タイマーが0になったら自身の座標を通知
                Debug.Log("自分の座標 : " + transform.position);
            // 破棄された場合Dispose
            }).AddTo(gameObject);
    }

    void Update() {
        DestroyCommand();
    }

    /// <summary>
    /// 自身を破棄する
    /// </summary>
    private void DestroyCommand() {
        if (Input.GetKey(KeyCode.Space)) {
            Debug.Log("破棄された");
            Destroy(gameObject);
        }
    }
}
