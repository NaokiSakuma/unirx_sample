
using UnityEngine;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;

public class UniTaskCanceledException : MonoBehaviour {

    /// <summary>
    /// 行っては駄目なメソッド
    /// </summary>
    /// <param name="pathName"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    private async UniTask DontUseMethod(string path, CancellationToken token) {
        await UniTask.Run(() => {
            Texture2D texture = FindTexture(path);
            // テクスチャが見つからなかったら例外を投げる
            if (texture == null) {
                // 処理に失敗したときにログを出したくないから使うのような使用用途はNG
                throw new OperationCanceledException();
            }
        });
    }

    /// <summary>
    /// 上記メソッド用に制作した空メソッド
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private Texture2D FindTexture(string path) {
        return new Texture2D(0, 0, TextureFormat.Alpha8, false);
    }
}