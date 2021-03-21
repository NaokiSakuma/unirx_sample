using UnityEngine;
using UniRx;
using System;

public class ObservableConversion : MonoBehaviour {

    void Start() {
        // ToReadOnlyReactivePropertyTimer();
        // ToReadOnlyReactivePropertySelect();
        ToReadOnlyReactivePropertyCombineLatest();
    }


#region ToReadOnlyReactiveProperty

    /// <summary>
    /// 1秒に1回、現在時刻を通知する
    /// </summary>
    private void ToReadOnlyReactivePropertyTimer() {
        ReadOnlyReactiveProperty<string> timer = Observable
                                                    .Interval(TimeSpan.FromSeconds(1))
                                                    .Select(_ => DateTime.Now.ToString())
                                                    .ToReadOnlyReactiveProperty();
        timer
            .Subscribe(x => {
                Debug.Log("Now Time : " + x);
            }, () => {
                Debug.Log("Timer onCompleted");
            }).AddTo(this);
    }


    /// <summary>
    /// Selectで別のReactivePropertyに変換
    /// </summary>
    private void ToReadOnlyReactivePropertySelect() {
        const int ENERGY_HP = 50;
        ReactiveProperty<int> hp = new ReactiveProperty<int>(1);
        // SelectでintをboolのReactivePropertyへ変換
        // HPが50を超えていたら元気
        ReadOnlyReactiveProperty<bool> isEnergy = hp.Select(x => x >= ENERGY_HP)
                                                    .ToReadOnlyReactiveProperty();

        // isEnergy = false
        Debug.Log(string.Format("hp : {0}, isEnergy : {1}", hp, isEnergy));
        hp.Value = ENERGY_HP;
        // isEnergy = true
        Debug.Log(string.Format("hp : {0}, isEnergy : {1}", hp, isEnergy));
    }

    /// <summary>
    /// CombineLatestで2つのReactivePropertyを比較
    /// </summary>
    private void ToReadOnlyReactivePropertyCombineLatest() {
        ReactiveProperty<int> value1 = new ReactiveProperty<int>(0);
        ReactiveProperty<int> value2 = new ReactiveProperty<int>(1);
        // value1とvalue2を比較して同じ値だったらtrue
        ReadOnlyReactiveProperty<bool> isSameValue = Observable
                                                        .CombineLatest(value1, value2, (x, y) => x == y)
                                                        .ToReadOnlyReactiveProperty();
        Debug.Log(string.Format("value1 : {0}, value2 : {1}, isSameValue : {2}", value1, value2, isSameValue));
        value1.Value = 5;
        Debug.Log(string.Format("value1 : {0}, value2 : {1}, isSameValue : {2}", value1, value2, isSameValue));
        value2.Value = 5;
        Debug.Log(string.Format("value1 : {0}, value2 : {1}, isSameValue : {2}", value1, value2, isSameValue));
    }

#endregion

}
