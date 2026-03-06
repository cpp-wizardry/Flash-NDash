using System.Collections.Generic;
using UnityEngine;

public class NPCSpawnerInitializer : MonoBehaviour
{
    public List<GameObject> npcPrefabs = new();
    public int maxNPCs = 40;
    public float spawnInterval = 3f;

    private void Awake()
    {
        GameObject[] houses;

        try { houses = GameObject.FindGameObjectsWithTag("POI_house"); }
        catch
        {
            Debug.LogError("Yeah we fucked up" + this);
            return;
        }

        if (houses.Length == 0)
        {
            Debug.Log("Bro forgot to add POI" + this);
            return;
        }

        foreach (GameObject house in houses)
        {
            NPCSpawner spawner = house.AddComponent<NPCSpawner>();
            spawner.Configure(npcPrefabs, maxNPCs, spawnInterval);
        }
    }
}