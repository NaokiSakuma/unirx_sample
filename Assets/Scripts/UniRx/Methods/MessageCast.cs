using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor;

// Cast ofType用
public class Enemy { public string Name { get; set; } };
public class Slime : Enemy { };
public class Drakee : Enemy { };

public class MessageCast : MonoBehaviour {

    private enum Methods {
        SELECT,
        CAST,
        OF_TYPE,
        SELECT_MANY_OBSERVABLE,
        SELECT_MANY_LIST,
        MATERIALIZE,
        TIME_INTERVAL,
        TIME_STAMP,
        AS_UNIT_OBSERVABLE,
    }

    [SerializeField]
    private Methods _methodStatus;
    [SerializeField]
    private bool _isDo = false;

    private bool _isInitialize = false;

    void Start() {
        _isInitialize = true;
        this.UpdateAsObservable()
            .Where(_ => _isDo)
            .Subscribe(_ => {
                _isDo = false;
            });
    }

    void OnValidate() {
        ExcuteMethods();
    }

    /// <summary>
    /// 値を変換する
    /// </summary>
    private void ExcuteSelect()
    {
        Debug.Log("--------Change Type--------");
        // 型変換
        Observable
            .Range(0, 1)
            // intからstringに変換
            .Select(x => x.ToString())
            .Subscribe(x => {
                Debug.Log("Select onNext : " + x.GetType());
            }, () => {
                Debug.Log("Select onCompleted");
            });
        Debug.Log("--------Change Value--------");
        // 値を変換
        Observable
            .Range(0, 1)
            // 値を変換
            .Select(x => 1000)
            .Subscribe(x => {
                Debug.Log("Select onNext : " + x);
            }, () => {
                Debug.Log("Select onCompleted");
            });

    }

    /// <summary>
    /// 型を変換し、失敗時はonErrorを発行する
    /// </summary>
    private void ExcuteCast() {
        Subject<Enemy> subject = new Subject<Enemy>();

        subject
            .Cast<Enemy, Slime>()
            .Subscribe(x => {
                Debug.Log("Cast onNext : " + x.Name);
            }, error => {
                Debug.LogException(error);
            }, () => {
                Debug.Log("Cast onCompleted");
            });

        subject.OnNext(new Slime { Name = "スライム" });
        // cast失敗
        subject.OnNext(new Drakee { Name = "ドラキー" });
        // 通らない
        subject.OnNext(new Slime { Name = "スライムベス" });
        // 通らない
        subject.OnCompleted();
    }

    /// <summary>
    /// 型変換をし、失敗しても無視する
    /// </summary>
    private void ExcuteOfType() {
        Subject<Enemy> subject = new Subject<Enemy>();

        subject
            .OfType<Enemy, Slime>()
            .Subscribe(x => {
                Debug.Log("OfType onNext : " + x.Name);
            }, error => {
                Debug.LogException(error);
            }, () => {
                Debug.Log("OfType onCompleted");
            });

        subject.OnNext(new Slime { Name = "スライム" });
        // cast失敗
        subject.OnNext(new Drakee { Name = "ドラキー" });
        // 通る
        subject.OnNext(new Slime { Name = "スライムベス" });
        // 通る
        subject.OnCompleted();
    }

    /// <summary>
    /// メッセージの値を元に別のObservableと合成する
    /// </summary>
    private void ExcuteSelectManyObservable()
    {
        Subject<string> subject = new Subject<string>();
        subject
            .SelectMany(x => Observable
                                .Range(0, 3)
                                .Select(y => x + y.ToString()))
            .Subscribe(x => {
                Debug.Log("SelectMany OnNext : " + x);
            } ,() => {
                Debug.Log("SelectMany OnCompleted");
            });

        subject.OnNext("A");
        subject.OnNext("B");
        subject.OnNext("C");
        subject.OnCompleted();
    }

    /// <summary>
    /// ListのListの中身
    /// </summary>
    private void ExcuteSelectManyList()
    {
        List<string> list1 = new List<string> { "A", "B", "C" };
        List<string> list2 = new List<string> { "D", "E", "F" };
        List<string> list3 = new List<string> { "G", "H", "I" };

        List<List<string>> listToList = new List<List<string>>();
        listToList.Add(list1);
        listToList.Add(list2);
        listToList.Add(list3);

        listToList
            .ToObservable()
            .SelectMany(lists => lists)
            .Subscribe(x => {
                Debug.Log("List Value : " + x);
            }, () => {
                Debug.Log("OnCompleted");
            });
    }

    /// <summary>
    /// メッセージにonNext,onError,onCompletedの情報を付与
    /// </summary>
    private void ExcuteMaterialize() {
        // onNextとonComplete
        IObservable<int> stream = Observable.Range(0, 3);
        Observable
            .Materialize(stream)
            .Subscribe(x => {
                Debug.Log("Materialize onNext : <color=blue>" + x + "</color>");
            }, () => {
                Debug.Log("Materialize onComplete");
            });

        // onError
        Observable
            .Throw<Unit>(new Exception("Error Message"))
            .Materialize()
            .Subscribe(x => {
                Debug.Log("Materialize onNext : <color=blue>" + x + "</color>");
            });

    }

    // frameもある
    /// <summary>
    /// 前回、値が流れてきたときからの経過時間を表示
    /// </summary>
    private void ExcuteTimeInterval()
    {
        // キーが押されたらメッセージ発行
        IObservable<Unit> input = this.UpdateAsObservable()
                                      .Where(_ => Input.anyKeyDown);
        // 前回、値が流れてきたときからの経過時間
        Observable
            .TimeInterval(input)
            .Subscribe(x => {
                Debug.Log("TimeInterval : " + x.Interval);
            });
    }

    /// <summary>
    /// メッセージにタイムスタンプを付与
    /// </summary>
    private void ExcuteTimeStamp()
    {
        // キーが押されたらメッセージ発行
        IObservable<Unit> input = this.UpdateAsObservable()
                                      .Where(_ => Input.anyKeyDown);
        // タイムスタンプを付与
        Observable
            .Timestamp(input)
            .Subscribe(x => {
                Debug.Log("TimeStamp : " + x.Timestamp);
            });
    }

    /// <summary>
    /// メッセージをUnit型に変換したい
    /// </summary>
    private void ExcuteAsUnitObservable()
    {
        Observable
            .Range(0, 3)
            .AsUnitObservable()
            .Subscribe(x => {
                Debug.Log("AsUnitObservable onNext : " + x);
            }, () => {
                Debug.Log("AsUnitObservable onCompleted");
            });
    }

    private void ExcuteMethods() {
        if (!_isInitialize) {
            // return;
        }
        Clear();
        switch (_methodStatus) {
            case Methods.SELECT :
                ExcuteSelect();
            break;
            case Methods.CAST :
                ExcuteCast();
            break;
            case Methods.OF_TYPE :
                ExcuteOfType();
            break;
            case Methods.SELECT_MANY_OBSERVABLE :
                ExcuteSelectManyObservable();
            break;
            case Methods.SELECT_MANY_LIST :
                ExcuteSelectManyList();
            break;
            case Methods.MATERIALIZE :
                ExcuteMaterialize();
            break;
            case Methods.TIME_INTERVAL :
                ExcuteTimeInterval();
            break;
            case Methods.TIME_STAMP :
                ExcuteTimeStamp();
            break;
            case Methods.AS_UNIT_OBSERVABLE :
                ExcuteAsUnitObservable();
            break;
        }
    }

    /// <summary>
    /// Unityのコンソールログを消す
    /// </summary>
    private void Clear()
    {
        var type = Assembly
            .GetAssembly( typeof( SceneView ) )
#if UNITY_2017_1_OR_NEWER
            .GetType( "UnityEditor.LogEntries" )
#else
            .GetType( "UnityEditorInternal.LogEntries" )
#endif
        ;
        var method = type.GetMethod( "Clear", BindingFlags.Static | BindingFlags.Public );
        method.Invoke( null, null );
    }
}
