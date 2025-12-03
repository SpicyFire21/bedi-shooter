using System;
using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;


public class MonsterSpawner : MonoBehaviour
{
    public MonsterDatabase monsterDatabase; // BDD de monstres
    public NavMeshSurface map;
    public Player player;

    [Header("Spawn Settings")]
    public float spawnRadiusMin = 5f;
    public float spawnRadiusMax = 15f;

    void Start()
    {
        foreach (MonsterData monster in monsterDatabase.monstersList)
        {
            StartCoroutine(SpawnRoutine(monster));
        }
    }

    IEnumerator SpawnRoutine(MonsterData monster)
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            Debug.Log("actuel : " + monster.monstersOnField);
            Debug.Log("max : " + monster.maxOnField);
            if (monster.monstersOnField >= monster.maxOnField)
                continue;

            float roll = Random.value; // si spawnrate = 0.5 alors une chance sur deux de spawn chaque secondes
            if (roll > monster.spawnRate)
                continue;



            Vector3 spawnPos = GetRandomPointOnNavMeshAroundPlayer();

            GameObject monsterObj = Instantiate(
                monster.prefab,
                spawnPos,
                Quaternion.identity
            );
            Debug.Log("instanciation");

            Monster monsterComponent = monsterObj.GetComponent<Monster>();
            monsterComponent.Spawn(spawnPos);
        }
    }

    int CountMonsterOnField(MonsterData monster)
    {
        return GameObject.FindGameObjectsWithTag(monster.prefab.tag).Length;
    }

    Vector3 GetRandomPointOnNavMeshAroundPlayer()
    {
        for (int i = 0; i < spawnRadiusMax; i++)
        {
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            float distance = Random.Range(spawnRadiusMin, spawnRadiusMax);

            Vector3 randomPos = player.transform.position +
                                new Vector3(randomDir.x, 0, randomDir.y) * distance;

            if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 3f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        // Fallback sécurité
        return player.transform.position + Vector3.forward * spawnRadiusMin;
    }
}
