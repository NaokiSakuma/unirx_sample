using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class UseOperator : MonoBehaviour {

#region UseOperator
    void Start() {
        FirstUpdate();
    }

    /// <summary>
    /// 初回のみUpdate処理
    /// </summary>
    private void FirstUpdate() {
        this.UpdateAsObservable()
            .FirstOrDefault()
            .Subscribe(x => {
                Debug.Log("初回処理終了");
            });
    }

#endregion

#region UnUseOperator

    private bool _isFirstUpdate;
    void Update() {
        if (_isFirstUpdate == false) {
            _isFirstUpdate = true;
            Debug.Log("初回処理終了");
        }
    }

#endregion

}
