using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using System;

public class UniTaskDownloadTexture : MonoBehaviour {

    [SerializeField]
    private Image _image;
    private const string TEXTURE_URL = "https://1.bp.blogspot.com/-cT-3wp1oE8E/X9lJoOcpbnI/AAAAAAABc7U/l27LG4wgsoc-D9AhUwDuYfF70r6-A2ccQCNcBGAsYHQ/s872/sori_snow_boy.png";

    void Start() {
        SetTexture();
    }

    private async void SetTexture() {
        Texture2D texture = await UniTaskGetTexture();
        _image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
    }

    /// <summary>
    /// UniTaskでURLから画像を取得
    /// </summary>
    /// <returns></returns>
    private async UniTask<Texture2D> UniTaskGetTexture() {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(TEXTURE_URL);
        await www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError) {
            throw new Exception(www.error);
        }
        return ((DownloadHandlerTexture)www.downloadHandler).texture;
    }
}

