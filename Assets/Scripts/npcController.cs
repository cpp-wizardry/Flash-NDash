using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCController : MonoBehaviour
{
    [Header("Daily Routine")]
    public List<string> routine = new();

    [Header("Thresholds")]
    public float stoppedThreshold = 0.1f;
    public float runDistanceThreshold = 15f;

    public event Action OnRoutineComplete;

    private NavMeshAgent _agent;
    private Animator _animator;
    private CityPeople.CityPeople _cityPeople;

    private int _currentIndex = -1;
    private bool _waiting = false;
    private POIPoint _currentPOI;
    private string _currentClip = "";

    private Dictionary<string, List<POIPoint>> _poiCache = new();

    private string GetWaitClip(string tag) => tag switch
    {
        "POI_Park" => "Exercise",
        "POI_Shop" => "Check",
        "POI_Stage" => "Idle2",
        "POI_BusStop" => "Idle2",
        _ => "Idle1"
    };

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _cityPeople = GetComponent<CityPeople.CityPeople>();


        if (routine.Count == 0)
        {
            PlayClip("Idle1");
            return;
        }

        BuildCache();
        GoToNext();
    }

    void Update()
    {
        if (_waiting || _currentPOI == null) return;

        if (_agent.velocity.magnitude > stoppedThreshold)
            PlayClip(_agent.remainingDistance > runDistanceThreshold ? "Run" : "Walk");
        else
            PlayClip("Idle1");

        if (!_agent.pathPending && _agent.remainingDistance <= _currentPOI.arrivalRadius)
            StartCoroutine(WaitAtPOI(_currentPOI.waitTime));
    }

    private void BuildCache()
    {
        _poiCache.Clear();
        foreach (string tag in routine)
        {
            if (_poiCache.ContainsKey(tag)) continue;

            var list = new List<POIPoint>();
            GameObject[] found;
            try { found = GameObject.FindGameObjectsWithTag(tag); }
            catch { Debug.LogWarning($"[NPC] Tag '{tag}' not in Tag Manager!"); continue; }

            foreach (var go in found)
            {
                var poi = go.GetComponent<POIPoint>();
                if (poi != null) list.Add(poi);
            }

            _poiCache[tag] = list;
        }
    }

    private void GoToNext()
    {
        int nextIndex = (_currentIndex + 1) % routine.Count;

        if (nextIndex == 0 && _currentIndex != -1)
        {
            OnRoutineComplete?.Invoke();
            return;
        }

        _currentIndex = nextIndex;
        string tag = routine[_currentIndex];

        POIPoint poi = GetRandomPOIFromCache(tag);
        if (poi == null)
        {
            if (_currentIndex < routine.Count - 1) GoToNext();
            return;
        }

        _currentPOI = poi;
        _agent.SetDestination(poi.transform.position);
        _waiting = false;
        PlayClip("Walk");
    }

    private IEnumerator WaitAtPOI(float duration)
    {
        _waiting = true;
        _agent.ResetPath();
        PlayClip(GetWaitClip(routine[_currentIndex]));
        yield return new WaitForSeconds(duration);
        GoToNext();
    }

    private void PlayClip(string clipName)
    {
        if (_animator == null || clipName == _currentClip) return;
        _currentClip = clipName;
        _animator.Play(clipName);
    }

    private POIPoint GetRandomPOIFromCache(string tag)
    {
        if (!_poiCache.TryGetValue(tag, out var list) || list.Count == 0) return null;
        return list[UnityEngine.Random.Range(0, list.Count)];
    }

    public void PauseRoutine()
    {
        StopAllCoroutines();
        _agent.ResetPath();
        _waiting = true;
        PlayClip("Idle1");
    }

    public void ResumeRoutine()
    {
        _waiting = false;
        GoToNext();
    }
}