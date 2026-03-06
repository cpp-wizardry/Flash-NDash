using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    public List<PhotoQuest> allQuests = new();

    public UnityEvent<PhotoQuest> onQuestCompleted;
    public UnityEvent<int> onPointsAwarded;

    public int TotalPoints { get; private set; } = 0;

    private PhotoValidator _validator;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        _validator = FindFirstObjectByType<PhotoValidator>();
    }

    public (bool validated, int questPoints) OnPhotoTaken(RaycastHit hit)
    {
        if (_validator == null) return (false, 0);

        PhotoFrameData frame = _validator.Evaluate();

        bool anyValidated = false;
        int totalQuestPoints = 0;

        foreach (var quest in allQuests)
        {
            if (quest.isCompleted) continue;
            if (!Validate(quest, frame)) continue;

            CompleteQuest(quest);
            anyValidated = true;
            totalQuestPoints += quest.pointReward;
        }

        return (anyValidated, totalQuestPoints);
    }

    public void AddPoints(int points)
    {
        TotalPoints += points;
        onPointsAwarded?.Invoke(TotalPoints);
    }

    private bool Validate(PhotoQuest quest, PhotoFrameData frame)
    {
        foreach (var tag in quest.requiredNPCTags)
            if (!frame.npcTags.Contains(tag)) return false;

        foreach (var tag in quest.requiredPOITags)
            if (!frame.poiTags.Contains(tag)) return false;

        if (quest.requireReactionState)
            if (!frame.reactionStates.Contains(quest.requiredReactionType)) return false;

        return true;
    }

    private void CompleteQuest(PhotoQuest quest)
    {
        quest.isCompleted = true;
        onQuestCompleted?.Invoke(quest);
        Debug.Log($"[QuestManager] Quest completed: {quest.title} | +{quest.pointReward} pts");
    }

    public List<PhotoQuest> GetActiveQuests() => allQuests.FindAll(q => !q.isCompleted);
    public List<PhotoQuest> GetCompletedQuests() => allQuests.FindAll(q => q.isCompleted);
}