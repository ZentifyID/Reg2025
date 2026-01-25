using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Reg2025/Store/StoreConfig")]
public class StoreConfig : ScriptableObject
{
    [Header("Coin packs")]
    public List<CoinPack> coinPacks = new();

    [Header("Free coins")]
    public int freeCoinsAmount = 100;
    public float freeCoinsCooldownSeconds = 60f;

    [Header("Video coins")]
    public int videoCoinsAmount = 100;
    public float videoCoinsCooldownSeconds = 30f;

    [Header("Fake payment/video timings")]
    public float paymentProcessingSeconds = 2f;
    public float videoDurationSeconds = 5f;
}

[Serializable]
public class CoinPack
{
    public string id;        // pack_1
    public int coins;        // 500
    public int priceRub;     // 99 (для отображения)
    public string titleKey;  // ключ локализации (например "store_pack_500")
}
