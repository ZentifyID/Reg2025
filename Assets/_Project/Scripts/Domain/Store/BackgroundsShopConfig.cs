using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Reg2025/Store/BackgroundsShopConfig")]
public class BackgroundsShopConfig : ScriptableObject
{
    public List<BackgroundItem> items = new();

    [Header("Video reward")]
    public int videoRewardCoins = 150;
    public float videoCooldownSeconds = 30f;
    public float videoDurationSeconds = 5f;
}

[Serializable]
public class BackgroundItem
{
    public string id;         // "bg_blue"
    public int priceCoins;    // 300
    public Sprite preview;    // иконка/превью, если нужно
}
