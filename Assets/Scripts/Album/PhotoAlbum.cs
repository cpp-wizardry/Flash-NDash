using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PhotoAlbum : MonoBehaviour
{
    [Header("References")]
    public GameObject albumPanel;
    public Transform gridContainer;
    public GameObject thumbnailPrefab;

    [Header("Detail View")]
    public GameObject detailPanel;
    public RawImage detailImage;
    public TextMeshProUGUI detailScore;
    public TextMeshProUGUI detailDate;
    public TextMeshProUGUI detailBreakdown;

    private List<Texture2D> _loadedTextures = new();
    private bool _isOpen = false;

    private void Start()
    {
        albumPanel.SetActive(false);
        detailPanel.SetActive(false);
    }

    public void ToggleAlbum()
    {
        _isOpen = !_isOpen;

        albumPanel.SetActive(_isOpen);

        if (_isOpen)
            StartCoroutine(LoadPhotos());
        else
            StartCoroutine(CloseAlbumRoutine());
    }

    private IEnumerator LoadPhotos()
    {
        yield return StartCoroutine(CleanupRoutine());

        string folder = PhotoSaver.Instance?.SaveFolder
            ?? Path.Combine(Application.persistentDataPath, "Photos");

        if (!Directory.Exists(folder))
            yield break;

        string[] pngFiles = Directory.GetFiles(folder, "*.png");

        foreach (string pngPath in pngFiles)
        {
            byte[] bytes = File.ReadAllBytes(pngPath);
            Texture2D tex = new Texture2D(2, 2, TextureFormat.RGB24, false);
            tex.LoadImage(bytes);
            _loadedTextures.Add(tex);

            PhotoMetadata meta = LoadMeta(pngPath);

            GameObject thumb = Instantiate(thumbnailPrefab, gridContainer);
            RawImage img = thumb.GetComponentInChildren<RawImage>();
            if (img != null) img.texture = tex;

            Button btn = thumb.GetComponent<Button>();
            if (btn != null)
            {
                var capturedTex = tex;
                var capturedMeta = meta;
                btn.onClick.AddListener(() => ShowDetail(capturedTex, capturedMeta));
            }

            yield return null;
        }
    }

    private IEnumerator CloseAlbumRoutine()
    {
        detailPanel.SetActive(false);
        yield return StartCoroutine(CleanupRoutine());
    }

    private IEnumerator CleanupRoutine()
    {
        if (detailImage != null)
            detailImage.texture = null;

        foreach (Transform child in gridContainer)
        {
            RawImage img = child.GetComponentInChildren<RawImage>();
            if (img != null) img.texture = null;
        }

        yield return null;

        foreach (Transform child in gridContainer)
            Destroy(child.gameObject);

        yield return null;

        foreach (var tex in _loadedTextures)
            if (tex != null) Destroy(tex);

        _loadedTextures.Clear();
    }

    private PhotoMetadata LoadMeta(string pngPath)
    {
        string jsonPath = Path.ChangeExtension(pngPath, ".json");
        if (!File.Exists(jsonPath)) return null;
        return JsonUtility.FromJson<PhotoMetadata>(File.ReadAllText(jsonPath));
    }

    private void ShowDetail(Texture2D tex, PhotoMetadata meta)
    {
        detailPanel.SetActive(true);
        detailImage.texture = tex;

        if (meta != null)
        {
            detailScore.text     = $"{meta.totalScore} Good Vibes";
            detailDate.text      = meta.timestamp;
            detailBreakdown.text = $"Quest: {meta.questPoints}  Framing: {meta.framingPoints}  Distance: {meta.distancePoints}  Pose: {meta.posePoints}";
        }
        else
        {
            detailScore.text     = "No data";
            detailDate.text      = "";
            detailBreakdown.text = "";
        }
    }

    public void CloseDetail()
    {
        detailPanel.SetActive(false);
    }
}