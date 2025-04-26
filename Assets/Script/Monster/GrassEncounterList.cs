using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GrassEncounter", menuName = "Monster/Create new grass encounter")]
public class GrassEncounterList : ScriptableObject
{
    [SerializeField] string areaName;
    [SerializeField] List<GrassEncounterItem> encounterItems;

    public string AreaName => areaName;
    public List<GrassEncounterItem> EncounterItems => encounterItems;

    public MonsterBase GetRandomMonster()
    {
        float randomValue = Random.Range(0f, 100f);
        float currentProbability = 0f;

        foreach (var item in encounterItems)
        {
            currentProbability += item.spawnChance;
            if (randomValue <= currentProbability)
            {
                return item.monster;
            }
        }

        // Fallback in case probabilities don't add up to 100
        return encounterItems[0].monster;
    }
}

[System.Serializable]
public class GrassEncounterItem
{
    [SerializeField] public MonsterBase monster;
    [Range(0f, 100f)]
    [SerializeField] public float spawnChance;
} 