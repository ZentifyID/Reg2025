using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelDataJson
{
    public int rewardCoins = 25;
    public string vehiclePrefabPath;         
    public List<int> requiredItems = new();
    public List<PlacedObjectJson> placedObjects = new();
}

[Serializable]
public class PlacedObjectJson
{
    public string prefabPath;
    public Vector3 position;
    public float rotationZ;
}