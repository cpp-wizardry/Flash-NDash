using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreNotification : MonoBehaviour
{
    public GameObject notifPrefab;
    public Transform notifContainer;
    public float displayDuration = 3f;

    private GameObject _current;

    public void Show(PhotoScoreResult result)
    {
        if (_current != null)
            Destroy(_current);

        _current = Instantiate(notifPrefab, notifContainer);
        _current.GetComponentInChildren<TMP_Text>().text = $"+{result.total} Good Vibes";

        StartCoroutine(DestroyAfter(displayDuration));
    }

    private IEnumerator DestroyAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(_current);
        _current = null;
    }
}