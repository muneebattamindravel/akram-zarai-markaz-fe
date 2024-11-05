using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class ImageUtils : MonoBehaviour
{
    public static ImageUtils Instance = null;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void LoadImage(RawImage image, string path)
    {
        //StartCoroutine(LoadProductImageFromDisk(image, path));
    }

    IEnumerator LoadProductImageFromDisk(RawImage image, string path)
    {
        if (image != null)
        {
            if (!File.Exists(path))
                yield break;

            var request = UnityWebRequestTexture.GetTexture("file://" + path);
            yield return request.SendWebRequest();
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            image.texture = texture;
        }
    }
}
