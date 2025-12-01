using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MonsterSpawnData
{
    public GameObject monsterPrefab;
    public int amount; // combien de ce monstre à spawn
}

public class SpawnManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    public List<MonsterSpawnData> monstersToSpawn;
    public Transform[] spawnPoints;

    void Start()
    {
        SpawnAll();
    }

    void SpawnAll()
    {
        foreach (var monster in monstersToSpawn)
        {
            for (int i = 0; i < monster.amount; i++)
            {
                SpawnMonster(monster.monsterPrefab);
            }
        }
    }

    void SpawnMonster(GameObject prefab)
    {
        if (spawnPoints.Length == 0 || prefab == null)
            return;

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
    }
}
