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

    [Header("Vehicle")]
    public GameObject vehiclePrefab;

    [Header("Allowed items / required items")]
    public List<ItemType> requiredItems = new List<ItemType>();

    [Header("Placed objects in level")]
    public List<PlacedObjectData> placedObjects = new List<PlacedObjectData>();
}

[Serializable]
public class PlacedObjectData
{
    public string prefabPath;
    public Vector3 position;
    public float rotationZ;
}