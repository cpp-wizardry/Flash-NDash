using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestUI : MonoBehaviour
{
    public Transform questListContainer;
    public GameObject questEntryPrefab;

    private List<GameObject> _entries = new();

    private void Start()
    {
        QuestManager.Instance.onQuestCompleted.AddListener(_ => Refresh());
        Refresh();
    }

    public void Refresh()
    {
        foreach (var e in _entries) Destroy(e);
        _entries.Clear();

        foreach (var quest in QuestManager.Instance.GetActiveQuests())
        {
            GameObject entry = Instantiate(questEntryPrefab, questListContainer);
            _entries.Add(entry);

            entry.transform.Find("QuestTits").GetComponent<TextMeshProUGUI>().text = quest.title;
            entry.transform.Find("QuestDesc").GetComponent<TextMeshProUGUI>().text = quest.description;
            entry.transform.Find("Rew").GetComponent<TextMeshProUGUI>().text = $"{quest.pointReward} GV";
        }
    }
}