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

    void Start()
    {
        foreach (MonsterData monster in monsterDatabase.monstersList)
        {
            monster.ResetOnField(); // on reset a chaque lancement de jeu
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



            Vector3 spawnPos = GetRandomPointOnNavMeshAroundPlayer(monster);

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

    Vector3 GetRandomPointOnNavMeshAroundPlayer(MonsterData monster) // on prend un point aléatoire sur le mesh en respectant le radius mit dans l'inspecteur -->                                              
                                                  // on essaye tant qu'on a pas trouvé de bonne position (20 fois en tout)
    {
        for (int i = 0; i < 20; i++)
        {
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            float distance = Random.Range(monster.minRangeRadius, monster.maxRangeRadius);

            Vector3 randomPos = player.transform.position +
                                new Vector3(randomDir.x, 0, randomDir.y) * distance;

            if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 3f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }
        return player.transform.position + Vector3.forward * monster.minRangeRadius;
    }
}
