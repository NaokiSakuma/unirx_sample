using UnityEngine;

public class UniTaskLogObject : MonoBehaviour {

    void Awake() {
        Debug.Log("Awake");
    }

    void Start() {
        Debug.Log("Start");
    }

    void OnDestroy() {
        Debug.Log("OnDestroy");
    }
}
