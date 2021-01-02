using UnityEngine;
using UniRx;

public class ReactivePropertyExample : MonoBehaviour {
    void Start() {
        ExcuteReactiveProperty();
    }

    /// <summary>
    /// ReactivePropertyの実行
    /// </summary>
    private void ExcuteReactiveProperty() {
        // ReactiveProperty
        ReactiveProperty<int> intRp = new ReactiveProperty<int>();
        // 初期値を指定可能
        ReactiveProperty<string> stringRp = new ReactiveProperty<string>("Hoge");

        // Subscribe時に値を発行
        intRp.Subscribe(x => {
            Debug.Log("int RP : " + x);
        });
        stringRp.Subscribe(x => {
            Debug.Log("string RP : " + x);
        });

        // 値を書き換えるとOnNext
        intRp.Value = 10;
        stringRp.Value = "Fuga";

        // 同じ値なので何もしない
        intRp.Value = 10;
        stringRp.Value = "Fuga";

        // 値の読み取りも可能
        int value = intRp.Value;
        Debug.Log("value : " + value);
    }
}
