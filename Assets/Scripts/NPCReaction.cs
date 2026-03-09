using System.Collections;
using UnityEngine;

public enum ReactionType { Poser, Shy, Ignore }

public class NPCReaction : MonoBehaviour
{
    public ReactionType reactionType = ReactionType.Ignore;

    public float reactionDistance = 8f;
    public float shyFleeDistance = 5f;
    public float insistenceThreshold = 2f;

    public string poseClip = "Pose";
    public string shyClip = "Shy";
    public string surpriseClip = "Surprise";
    public string fleeClip = "Jog";

    public bool IsReacting { get; private set; } = false;
    public ReactionType CurrentReaction => reactionType;

    private NPCController _controller;
    private Animator _animator;
    private Transform _playerTransform;
    private float _aimedDuration = 0f;
    private Coroutine _reactionRoutine;

    void Start()
    {
        _controller = GetComponent<NPCController>();
        _animator = GetComponent<Animator>();
        _playerTransform = Camera.main.transform;
    }

    public void OnAimedAt()
    {
        if (IsReacting) return;

        float distance = Vector3.Distance(transform.position, _playerTransform.position);
        if (distance > reactionDistance) return;

        _aimedDuration = 0f;

        switch (reactionType)
        {
            case ReactionType.Poser:
                _reactionRoutine = StartCoroutine(PoserRoutine());
                break;
            case ReactionType.Shy:
                _reactionRoutine = StartCoroutine(ShyAimedRoutine());
                break;
        }
    }

    public void OnAimCleared()
    {
        if (reactionType == ReactionType.Ignore) return;
        if (_reactionRoutine != null)
            StopCoroutine(_reactionRoutine);

        if (!IsReacting) return;

        IsReacting = false;
        _controller?.ResumeRoutine();
    }

    public void OnPhotographed()
    {
        float distance = Vector3.Distance(transform.position, _playerTransform.position);
        if (distance > reactionDistance) return;
        SoundManager.Instance.PlayVoiceLine();
        switch (reactionType)
        {
            case ReactionType.Poser:
                PlayClip(surpriseClip);
                break;
            case ReactionType.Shy:
                StartCoroutine(FleeRoutine());
                break;
        }
    }

    private IEnumerator PoserRoutine()
    {
        IsReacting = true;
        _controller?.PauseRoutine();
        FacePlayer();
        PlayClip(poseClip);

        yield return new WaitForSeconds(4f);

        IsReacting = false;
        _controller?.ResumeRoutine();
    }

    private IEnumerator ShyAimedRoutine()
    {
        while (true)
        {
            _aimedDuration += Time.deltaTime;

            if (_aimedDuration >= insistenceThreshold)
            {
                StartCoroutine(FleeRoutine());
                yield break;
            }

            if (!IsReacting)
            {
                IsReacting = true;
                _controller?.PauseRoutine();
                FaceAwayFromPlayer();
                PlayClip(shyClip);
            }

            yield return null;
        }
    }

    private IEnumerator FleeRoutine()
    {
        IsReacting = true;
        _controller?.PauseRoutine();

        Vector3 fleeDir = (transform.position - _playerTransform.position).normalized;
        Vector3 fleeTarget = transform.position + fleeDir * shyFleeDistance;

        var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
        {
            PlayClip(fleeClip);
            agent.SetDestination(fleeTarget);
            yield return new WaitForSeconds(3f);
            agent.ResetPath();
        }

        IsReacting = false;
        _controller?.ResumeRoutine();
    }

    private void FacePlayer()
    {
        Vector3 dir = (_playerTransform.position - transform.position).normalized;
        dir.y = 0;
        if (dir != Vector3.zero) transform.rotation = Quaternion.LookRotation(dir);
    }

    private void FaceAwayFromPlayer()
    {
        Vector3 dir = (transform.position - _playerTransform.position).normalized;
        dir.y = 0;
        if (dir != Vector3.zero) transform.rotation = Quaternion.LookRotation(dir);
    }

    private void PlayClip(string clipName)
    {
        if (_animator == null || string.IsNullOrEmpty(clipName)) return;
        _animator.CrossFadeInFixedTime(clipName, 0.3f);
    }
}