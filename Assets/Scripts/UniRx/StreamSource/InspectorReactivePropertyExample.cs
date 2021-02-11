using UnityEngine;
using UniRx;

public class InspectorReactivePropertyExample : MonoBehaviour {
    [SerializeField]
    private IntReactiveProperty _intRp = new IntReactiveProperty();
    [SerializeField]
    private StringReactiveProperty _stringRp = new StringReactiveProperty();

    void Start() {
        ExcuteInspectorRP();
    }

    /// <summary>
    /// InspectorからのReactiveProperty
    /// </summary>
    private void ExcuteInspectorRP() {
        _intRp.Subscribe(x => {
            Debug.Log("int RP : " + x);
        });
        _stringRp.Subscribe(x => {
            Debug.Log("string RP : " + x);
        });
    }
}
