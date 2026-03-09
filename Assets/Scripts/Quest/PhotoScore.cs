using System.Collections.Generic;
using UnityEngine;

public class PhotoScorer : MonoBehaviour
{
    public static PhotoScorer Instance { get; private set; }

    [Header("Score Weights (must add up to 1000)")]
    public int questWeight = 400;
    public int framingWeight = 300;
    public int distanceWeight = 200;
    public int poseWeight = 100;

    [Header("Optimal Distance")]
    public float optimalDistanceMin = 3f;
    public float optimalDistanceMax = 10f;

    [Header("Pose Scores (0-100)")]
    public List<PoseScore> poseScores = new()
    {
        new PoseScore { clipName = "Idle1",    score = 10 },
        new PoseScore { clipName = "Idle2",    score = 20 },
        new PoseScore { clipName = "Walk",      score = 30 },
        new PoseScore { clipName = "Run",       score = 30 },
        new PoseScore { clipName = "Check",     score = 50 },
        new PoseScore { clipName = "Exercise",  score = 60 },
        new PoseScore { clipName = "Dance",      score = 100 },
        new PoseScore { clipName = "Shy",       score = 80 },
    };

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public PhotoScoreResult Evaluate(RaycastHit hit, PhotoFrameData frame, bool questValidated, int questPoints)
    {
        var result = new PhotoScoreResult();

        int questScore = EvaluateQuest(questValidated, questPoints);
        int framingScore = EvaluateFraming(hit);
        int distanceScore = EvaluateDistance(hit.distance);
        int poseScore = EvaluatePose(hit);

        result.questPoints    = questScore;
        result.framingPoints  = framingScore;
        result.distancePoints = distanceScore;
        result.posePoints     = poseScore;
        result.total          = Mathf.Clamp(questScore + framingScore + distanceScore + poseScore, 0, 1000);
        if (result.total < 350)
        {
            SoundManager.Instance.PlayBadPicture();
        }
        else if (result.total > 600)
        {
            SoundManager.Instance.PlayNicePicture();
        }
        else
        {
            SoundManager.Instance.PlayMidPicture();
        
        }
        return result;
    }

    private int EvaluateQuest(bool validated, int questPoints)
    {
        if (!validated) return 0;
        return Mathf.RoundToInt(questPoints / 1000f * questWeight);
    }

    private int EvaluateFraming(RaycastHit hit)
    {
        Camera cam = Camera.main;
        Vector3 viewportPos = cam.WorldToViewportPoint(hit.point);

        float centerX = Mathf.Abs(viewportPos.x - 0.5f);
        float centerY = Mathf.Abs(viewportPos.y - 0.5f);
        float centerScore = 1f - Mathf.Clamp01((centerX + centerY) * 2f);

        return Mathf.RoundToInt(centerScore * framingWeight);
    }

    private int EvaluateDistance(float distance)
    {
        float score;

        if (distance >= optimalDistanceMin && distance <= optimalDistanceMax)
            score = 1f;
        else if (distance < optimalDistanceMin)
            score = Mathf.Clamp01(distance / optimalDistanceMin);
        else
            score = Mathf.Clamp01(1f - (distance - optimalDistanceMax) / optimalDistanceMax);

        return Mathf.RoundToInt(score * distanceWeight);
    }

    private int EvaluatePose(RaycastHit hit)
    {
        Animator animator = hit.collider.GetComponentInParent<Animator>();
        if (animator == null) return 0;

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        foreach (var ps in poseScores)
        {
            if (state.IsName(ps.clipName))
                return Mathf.RoundToInt((ps.score / 100f) * poseWeight);
        }

        return 0;
    }
}

[System.Serializable]
public class PoseScore
{
    public string clipName;
    [Range(0, 100)] public int score;
}

public class PhotoScoreResult
{
    public int questPoints;
    public int framingPoints;
    public int distancePoints;
    public int posePoints;
    public int total;

    public override string ToString() =>
        $"+{total} Good Vibes  (Quest:{questPoints} Framing:{framingPoints} Distance:{distancePoints} Pose:{posePoints})";
}