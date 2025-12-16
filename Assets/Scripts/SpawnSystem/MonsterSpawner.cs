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
    private int lastProcessedMinute = -1;
    private Monster currentBoss;
    public Text alertBossText;
    private float alertDuration = 3f;
    private int additionalLevel = 10;


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

    private IEnumerator ShowBossAlert(string message)
    {
        alertBossText.text = message;
        alertBossText.gameObject.SetActive(true); // au cas où le texte était caché
        yield return new WaitForSeconds(alertDuration);
        alertBossText.gameObject.SetActive(false); // le cacher après le temps
    }


    private void InitSpawnSystem()
    {
        monsterDatabase.SetMonsterDatabaseEmpty();
        monsterDatabase.AddMonstre(completeMonsterDatabase.GetMonsterDataFromIndex(0));
        monsterDatabase.AddMonstre(completeMonsterDatabase.GetMonsterDataFromIndex(1));
        actualSurvivalTime = 0f;
        survivalSeconds = 0;
        nextIndexMonstre = 2;
    }

    //private void DifficultyUpgrade()
    //{
    //    int currentMinute = survivalSeconds / 10;

    //    if (currentMinute == lastProcessedMinute) return; // déjà appliqué ce palier

    //    lastProcessedMinute = currentMinute; // on marque le palier comme traité

    //    foreach (MonsterData monster in monsterDatabase.monstersList)
    //    {
    //        // scaling asymptotique : monte vite au début, puis ralentit
    //        float scaled = 1f - Mathf.Pow(0.98f, currentMinute); // on utilise currentMinute au lieu de survivalSeconds
    //        monster.spawnRate = Mathf.Clamp(monster.spawnRate * (1f + scaled), 0f, 1f);
    //    }

    //    Debug.Log("Augmentation de difficulté !");
    //}


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
            //DifficultyUpgrade();
        }
    }

    private void handleSpawns() // la logique est qu'on prend la database complete les fonctions gerent les index automatiquement
        // et pour les boss, on force leurs spawn, puis quand ils sont battu, ils peuvent spawn naturellement
    {
        AddNewMonsterPossiblySpawning(1); // dark skeleton au bout de 30 secondes
        MonsterBossEvent(completeMonsterDatabase.GetMonsterDataFromIndex(nextIndexMonstre), 180); // BOSS GOLEM A 3mn
        MonsterBossEvent(completeMonsterDatabase.GetMonsterDataFromIndex(nextIndexMonstre), 360); // BOSS DRAGON A 6mn 
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

    private void AddNewMonsterPossiblySpawning(MonsterData newMonster)
    {
            if (!monsterDatabase.ContainsMonstre(newMonster))
            {
                monsterDatabase.AddMonstre(newMonster);

                StartCoroutine(SpawnRoutine(newMonster));
                nextIndexMonstre++;
        }
    }

    private void MonsterBossEvent(MonsterData monsterData, int seconds)
    {
        if (survivalSeconds == seconds)
        {
            GameObject bossObj;
            if (monsterData.onGround)
            {
                bossObj = Instantiate(
                    monsterData.prefab,
                    GetRandomPointOnNavMeshAroundPlayer(monsterData),
                    Quaternion.identity
                );
            } else
            {
                bossObj = Instantiate(
                    monsterData.prefab,
                    GetRandomAirSpawnPositionAroundPlayer(monsterData),
                    Quaternion.identity);
            }

            currentBoss = bossObj.GetComponent<Monster>();
            currentBoss.Spawn(bossObj.transform.position, player.level + additionalLevel + 3);
            StartCoroutine(ShowBossAlert("A new boss has spawned: <color=red>" + monsterData.name + "</color>"));
            additionalLevel += 9;
        }
    }


    private void HandleMonsterDeath(Monster monster) // lorsque l'un monstre meurt, si c'est le boss actuel et qu'on le tue, on va lui ajouté une coroutine de spawn --> devient
        // un mob commun
    {
        if (monster == currentBoss)
        {
            OnBossKilled();
        }
    }

    void OnEnable() // sur chaque monstre on écoute si il meurt
    {
        Monster.OnMonsterDeath += HandleMonsterDeath;
    }

    void OnDisable()
    {
        Monster.OnMonsterDeath -= HandleMonsterDeath;
    }



    private void OnBossKilled()
    {
        AddNewMonsterPossiblySpawning(currentBoss.data);
        StartCoroutine(ShowBossAlert("A boss has been killed: <color=red>" + currentBoss.data.name + "</color>, \nhe can now spawn on the map"));
        currentBoss = null;
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
            monsterComponent.Spawn(spawnPos, Random.Range(1, player.level + (Random.Range(3, additionalLevel))));
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
