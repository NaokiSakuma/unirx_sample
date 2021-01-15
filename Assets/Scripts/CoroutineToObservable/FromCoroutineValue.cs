using UnityEngine;
using UniRx;
using System.Collections.Generic;

public class FromCoroutineValue : MonoBehaviour {
    private List<int> _list = new List<int>();

    void Start() {
        AddList();
        ExcuteFromCoroutineValue();
    }

    /// <summary>
    /// Listに値を追加
    /// </summary>
    private void AddList() {
        for (int i = 0; i < 5; i++) {
            _list.Add(i);
        }
    }

    /// <summary>
    /// FromCoroutineValueの実行
    /// </summary>
    private void ExcuteFromCoroutineValue() {
        Observable
            // コルーチンから値を取り出す
            .FromCoroutineValue<int>(TakeList, false)
            .Subscribe(x => {
                Debug.Log("OnNext. Value : " + x);
            }, () => {
                Debug.Log("OnCompleted");
            });
    }

    /// <summary>
    /// Listの値を取り出す
    /// </summary>
    /// <returns></returns>
    private IEnumerator<int> TakeList() {
        return _list.GetEnumerator();
    }
}

