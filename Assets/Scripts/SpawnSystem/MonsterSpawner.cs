using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class MonsterSpawner : MonoBehaviour
{
    public MonsterDatabase completeMonsterDatabase;
    public MonsterDatabase monsterDatabase; // BDD de monstres
    public NavMeshSurface map;
    public Player player;
    private float actualSurvivalTime;
    private string timerTextValue;
    public Text timerText;
    private int survivalSeconds;
    private int nextIndexMonstre;
    private int lastProcessedSecond = -1;


    void Start()
    {
        InitSpawnSystem();
        foreach (MonsterData monster in completeMonsterDatabase.monstersList)
        {
            monster.ResetOnField(); // on reset a chaque lancement de jeu
        }
        foreach (MonsterData monster in monsterDatabase.monstersList)
        {
            StartCoroutine(SpawnRoutine(monster));
        }
    }

    public string getTimerText()
    {
        return timerTextValue;
    }

    private void InitSpawnSystem()
    {
        monsterDatabase.SetMonsterDatabaseEmpty();
        monsterDatabase.AddMonstre(completeMonsterDatabase.GetMonsterDataFromIndex(0));
        monsterDatabase.AddMonstre(completeMonsterDatabase.GetMonsterDataFromIndex(1));
        monsterDatabase.AddMonstre(completeMonsterDatabase.GetMonsterDataFromIndex(2));
        actualSurvivalTime = 0f;
        survivalSeconds = 0;
        nextIndexMonstre = 3;
    }

    void Update()
    {
        actualSurvivalTime += Time.deltaTime; 
        timerTextValue = FormatTime(actualSurvivalTime);
        timerText.text = timerTextValue;
        survivalSeconds = Mathf.FloorToInt(actualSurvivalTime);
        if (survivalSeconds != lastProcessedSecond)
        {
            lastProcessedSecond = survivalSeconds;
            handleSpawns();
        }
    }

    private void handleSpawns()
    {
        AddNewMonsterPossiblySpawning(5); // golem
        AddNewMonsterPossiblySpawning(10); // dragon
    }

    private void AddNewMonsterPossiblySpawning(int seconds)
    {
        if (survivalSeconds == seconds)
        {
            MonsterData newMonster = completeMonsterDatabase.GetMonsterDataFromIndex(nextIndexMonstre);
            if (!monsterDatabase.ContainsMonstre(newMonster))
            {
                monsterDatabase.AddMonstre(newMonster);

                StartCoroutine(SpawnRoutine(newMonster));
                nextIndexMonstre++;
            }
        }
    }

    private void MonsterBossEvent()
    {

    }




    IEnumerator SpawnRoutine(MonsterData monster)
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (!monsterDatabase.ContainsMonstre(monster)) continue;
            if (monster.monstersOnField >= monster.maxOnField)
                continue;

            float roll = Random.value; // si spawnrate = 0.5 alors une chance sur deux de spawn chaque secondes
            if (roll > monster.spawnRate)
                continue;
            Vector3 spawnPos;
            if (monster.onGround)
            {
                spawnPos = GetRandomPointOnNavMeshAroundPlayer(monster);
            } else
            {
                spawnPos = GetRandomAirSpawnPositionAroundPlayer(monster);
            }

                GameObject monsterObj = Instantiate(
                    monster.prefab,
                    spawnPos,
                    Quaternion.identity
                );
            Monster monsterComponent = monsterObj.GetComponent<Monster>();
            monsterComponent.Spawn(spawnPos, Random.Range(1, player.level + (Random.Range(1, 10))));
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

    Vector3 GetRandomAirSpawnPositionAroundPlayer(MonsterData monster)
    {
        // Choisit une direction aléatoire autour du joueur
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        float distance = Random.Range(monster.minRangeRadius, monster.maxRangeRadius);

        // Choisit une hauteur aléatoire ou fixe
        float height = Random.Range(5, 10);

        // Calcul de la position aérienne
        Vector3 spawnPos = player.transform.position + new Vector3(randomDir.x, 0, randomDir.y) * distance;
        spawnPos.y += height;

        return spawnPos;
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

}
