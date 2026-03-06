using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewQuest", menuName = "PhotoGame/Quest")]
public class PhotoQuest : ScriptableObject
{
    [Header("Info")]
    public string title;
    [TextArea] public string description;
    public int pointReward = 100;

    [Header("Criteria — all filled criteria must be met")]
    [Tooltip("Unity tags that must be present on at least one NPC in the frame")]
    public List<string> requiredNPCTags = new();

    [Tooltip("Unity tags that must be present on at least one POI/decor object in the frame")]
    public List<string> requiredPOITags = new();

    [Tooltip("If set, at least one NPC in the frame must be in this reaction state")]
    public bool requireReactionState = false;
    public ReactionType requiredReactionType;

    [Header("State")]
    public bool isCompleted = false;
}