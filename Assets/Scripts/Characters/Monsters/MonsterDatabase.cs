using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MonsterDatabase", menuName = "Monsters/Monster Database")]
public class MonsterDatabase : ScriptableObject
{
    public List<MonsterData> monstersList = new List<MonsterData>();

    public void AddMonstre(MonsterData monsterData)
    {
        monstersList.Add(monsterData);
    }

    public bool ContainsMonstre(MonsterData monsterData)
    {
        return monstersList.Contains(monsterData);
    }

    public MonsterData GetMonsterDataFromIndex(int index)
    {
        if (index >= 0 && index < monstersList.Count)
            return monstersList[index];
        else
        {
            return null;
        }
    }

    public void SetMonsterDatabaseEmpty()
    {
        this.monstersList = new List<MonsterData>();
    }

}