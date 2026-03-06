using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class PhotoSaver : MonoBehaviour
{
    public static PhotoSaver Instance { get; private set; }

    public Camera photoCamera;
    public int textureWidth = 1920;
    public int textureHeight = 1080;

    private RenderTexture _renderTexture;
    private string _saveFolder;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        _saveFolder = Path.Combine(Application.persistentDataPath, "Photos");
        Directory.CreateDirectory(_saveFolder);
    }

    private void Start()
    {
        if (photoCamera == null)
            photoCamera = Camera.main;

        _renderTexture = new RenderTexture(textureWidth, textureHeight, 24);
    }

    private void OnDestroy()
    {
        if (_renderTexture != null)
            _renderTexture.Release();
    }

    public void SavePhoto(PhotoScoreResult result)
    {
        StartCoroutine(CaptureRoutine(result));
    }

    private IEnumerator CaptureRoutine(PhotoScoreResult result)
    {
        yield return new WaitForEndOfFrame();

        RenderTexture prev = photoCamera.targetTexture;
        int prevCulling = photoCamera.cullingMask;

        photoCamera.targetTexture = _renderTexture;
        photoCamera.cullingMask = prevCulling & ~(1 << LayerMask.NameToLayer("UI"));
        photoCamera.Render();
        photoCamera.targetTexture = prev;
        photoCamera.cullingMask = prevCulling;

        RenderTexture.active = _renderTexture;
        Texture2D tex = new Texture2D(textureWidth, textureHeight, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, textureWidth, textureHeight), 0, 0);
        tex.Apply();
        RenderTexture.active = null;

        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string pngPath = Path.Combine(_saveFolder, $"photo_{timestamp}.png");
        string jsonPath = Path.Combine(_saveFolder, $"photo_{timestamp}.json");

        File.WriteAllBytes(pngPath, tex.EncodeToPNG());
        Destroy(tex);

        PhotoMetadata meta = new PhotoMetadata
        {
            timestamp     = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            totalScore    = result.total,
            questPoints   = result.questPoints,
            framingPoints = result.framingPoints,
            distancePoints = result.distancePoints,
            posePoints    = result.posePoints
        };

        File.WriteAllText(jsonPath, JsonUtility.ToJson(meta, true));

        Debug.Log($"[PhotoSaver] Saved: {pngPath}");
    }

    public string SaveFolder => _saveFolder;
}

[Serializable]
public class PhotoMetadata
{
    public string timestamp;
    public int totalScore;
    public int questPoints;
    public int framingPoints;
    public int distancePoints;
    public int posePoints;
}