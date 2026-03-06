using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCSpawner : MonoBehaviour
{
    public static NPCSpawner Instance { get; private set; }

    public List<GameObject> npcPrefabs = new();
    public int maxNPCs = 40;
    public float spawnInterval = 3f;

    private static int _aliveCount = 0;
    private static readonly object _lock = new();

    private Transform _spawnPoint;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        Transform found = transform.Find("SpawnPoint");
        _spawnPoint = found != null ? found : transform;
    }

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    public void Configure(List<GameObject> prefabs, int max, float interval)
    {
        npcPrefabs = prefabs;
        maxNPCs = max;
        spawnInterval = interval;
    }

    private IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(Random.Range(0f, spawnInterval));

        while (true)
        {
            if (_aliveCount < maxNPCs && npcPrefabs.Count > 0)
                SpawnOne();

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnOne()
    {
        Vector3 spawnPos;

        if (!SampleNavMesh(_spawnPoint.position, out spawnPos))
        {
            Debug.LogWarning("No valid navmesh you retard either add radius or wtf" + this);
            return;
        }

        GameObject prefab = npcPrefabs[Random.Range(0, npcPrefabs.Count)];
        GameObject npc = Instantiate(prefab, spawnPos, Quaternion.identity);

        NPCController controller = npc.GetComponent<NPCController>();
        if (controller != null)
            controller.OnRoutineComplete += () => Despawn(npc);
        else
            Debug.LogWarning("goofy this mfer got no buzz" + prefab.name);

        lock (_lock) { _aliveCount++; }
    }

    private bool SampleNavMesh(Vector3 origin, out Vector3 result)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(origin, out hit, 10f, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }
        result = origin;
        return false;
    }

    private void Despawn(GameObject npc)
    {
        lock (_lock) { _aliveCount = Mathf.Max(0, _aliveCount - 1); }
        Destroy(npc);
    }

    public int AliveCount => _aliveCount;
}