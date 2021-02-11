using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class DownloadTexture : MonoBehaviour {

    [SerializeField]
    private Image _image;
    private const string TEXTURE_URL = "https://1.bp.blogspot.com/-cT-3wp1oE8E/X9lJoOcpbnI/AAAAAAABc7U/l27LG4wgsoc-D9AhUwDuYfF70r6-A2ccQCNcBGAsYHQ/s872/sori_snow_boy.png";

    void Start() {
        StartCoroutine(CoroutineGetTexture());
    }

    /// <summary>
    /// コルーチンでURLから画像を取得
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoroutineGetTexture() {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(TEXTURE_URL);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) {
            Debug.LogError(www.error);
        } else {
            Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            _image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }
    }
}
