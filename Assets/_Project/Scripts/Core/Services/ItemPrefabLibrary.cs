using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/ItemPrefabLibrary", fileName = "ItemPrefabLibrary")]
public class ItemPrefabLibrary : ScriptableObject
{
    [Serializable]
    public struct Entry
    {
        public ItemType type;
        public GameObject prefab;
    }

    public List<Entry> entries = new();

    private Dictionary<ItemType, GameObject> _map;

    public GameObject Get(ItemType type)
    {
        _map ??= Build();
        return _map.TryGetValue(type, out var prefab) ? prefab : null;
    }

    private Dictionary<ItemType, GameObject> Build()
    {
        var dict = new Dictionary<ItemType, GameObject>();
        foreach (var e in entries)
            if (e.prefab != null)
                dict[e.type] = e.prefab;
        return dict;
    }
}
