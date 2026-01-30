using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/LevelDatas", fileName = "LevelDatas")]
public class LevelDatas : ScriptableObject
{
    public List<LevelData> levels = new List<LevelData>();

    public int Count => levels?.Count ?? 0;

    public LevelData GetByIndex(int index)
    {
        if (levels == null || levels.Count == 0) return null;
        index = Mathf.Clamp(index, 0, levels.Count - 1);
        return levels[index];
    }
}

[Serializable]
public class LevelData
{
    public int rewardCoins = 25;
    public List<ItemType> requiredItems = new List<ItemType>();

    [Header("Vehicle")]
    public GameObject vehiclePrefab;

    [Header("Which child in LevelDesignRoot to enable (0 = first child)")]
    public int levelDesignIndex = 0;
}