using UnityEngine;
using UniRx;

public class ReactiveCollection : MonoBehaviour {
    void Start() {
        // ExcuteReactiveCollection();
        ExcuteReactiveDictionary();
    }

    /// <summary>
    /// ReactiveCollectionの実行
    /// </summary>
    private void ExcuteReactiveCollection() {
        // ReactiveCollection
        ReactiveCollection<string> rcString = new ReactiveCollection<string>();

        // Subscribe時に値を発行
        // 要素追加時
        rcString
            .ObserveAdd()
            .Subscribe(x => {
                Debug.Log(string.Format("[{0}]番目に[{1}]を追加", x.Index, x.Value));
            });

        // 要素削除時
        rcString
            .ObserveRemove()
            .Subscribe(x => {
                Debug.Log(string.Format("[{0}]番目の[{1}]を削除", x.Index, x.Value));
            });

        // 要素を追加するとOnNext
        rcString.Add("Hoge");
        rcString.Add("Fuga");
        rcString.Add("Foo");

        // 要素を削除するとOnNext
        rcString.Remove("Fuga");
        foreach (string str in rcString) {
            Debug.Log("rcStringの中身 : " + str);
        }

        // 存在しないので呼ばれない
        rcString.Remove("nothing");
    }

    /// <summary>
    /// ReactiveDictionaryの実行
    /// </summary>
    private void ExcuteReactiveDictionary() {
        // ReactiveDictionary
        ReactiveDictionary<string, int> reactiveDictionary = new ReactiveDictionary<string, int>();

        // Subscribe時に値を発行
        // 要素追加時
        reactiveDictionary
            .ObserveAdd()
            .Subscribe(x => {
                Debug.Log(string.Format("要素追加, Key : {0}, Value : {1}", x.Key, x.Value));
            });

        // 要素削除時
        reactiveDictionary
            .ObserveRemove()
            .Subscribe(x => {
                Debug.Log(string.Format("要素削除, Key : {0}, Value : {1}", x.Key, x.Value));
            });

        // 要素数変化時
        reactiveDictionary
            .ObserveCountChanged()
            .Subscribe(x => {
                Debug.Log(string.Format("要素数変化, Count : {0}", x));
            });

        // 要素を追加するとOnNext
        reactiveDictionary.Add("スライム", 10);
        reactiveDictionary.Add("ドラゴン", 100);
        reactiveDictionary.Add("魔王", 1000);

        // 要素を削除するとOnNext
        reactiveDictionary.Remove("スライム");
        foreach(var keyValue in reactiveDictionary) {
            Debug.Log(string.Format("Dictionaryの中身, Key : {0}, Value : {1}", keyValue.Key, keyValue.Value));
        }
    }
}
