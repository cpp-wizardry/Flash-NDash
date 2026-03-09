using UnityEngine;

public class TagArtSpawner : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite[] sprites;

    [Header("Settings")]
    public Vector2 size = new Vector2(2f, 2f);

    private void Start()
    {
        if (sprites == null || sprites.Length == 0)
        {
            Debug.LogWarning("No sprites assigned"+this);
            return;
        }

        GameObject[] tagPoints;
        try { tagPoints = GameObject.FindGameObjectsWithTag("POI_TagArt"); }
        catch { Debug.LogError("no tag"+this); return; }

        foreach (GameObject point in tagPoints)
            SpawnSprite(point);
    }

    private void SpawnSprite(GameObject point)
    {
        Sprite sprite = sprites[Random.Range(0, sprites.Length)];

        GameObject go = new GameObject("TagArt_Sprite");
        go.transform.SetParent(point.transform, false);
        go.transform.localPosition = new Vector3(0f, 0f, 0.5f);
        go.transform.localRotation = Quaternion.Euler(0, 90f, 0f);

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.drawMode = SpriteDrawMode.Sliced;
        sr.size = size;
    }
}