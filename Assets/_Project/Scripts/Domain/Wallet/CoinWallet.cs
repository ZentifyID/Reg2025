using System;
using UnityEngine;

public class CoinWallet : MonoBehaviour
{
    public static CoinWallet Instance { get; private set; }

    public event Action<int> CoinsChanged;

    public UserData Data { get; private set; } = new UserData { coins = 0 };

    private UserDataStorage storage;

    public int Coins { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        storage = new UserDataStorage();
        var loaded = storage.LoadOrCreate();
        Data = loaded ?? new UserData { coins = 0 };

        Coins = Data.coins;
        CoinsChanged?.Invoke(Coins);
    }

    public void Add(int amount)
    {
        if (storage == null) storage = new UserDataStorage();
        Data ??= new UserData();

        Data.coins += Mathf.Max(0, amount);
        Coins = Data.coins;

        storage.Save(Data);
        CoinsChanged?.Invoke(Coins);
    }

    public bool TrySpend(int amount)
    {
        if (amount <= 0) return true;

        if (storage == null) storage = new UserDataStorage();
        Data ??= new UserData();

        if (Data.coins < amount) return false;

        Data.coins -= amount;
        Coins = Data.coins;

        storage.Save(Data);
        CoinsChanged?.Invoke(Coins);
        return true;
    }

    public void SaveProgress(int currentLevel)
    {
        if (storage == null) storage = new UserDataStorage();
        Data ??= new UserData();

        Data.lastPlayedLevel = currentLevel;
        storage.Save(Data);
    }

    public int GetSavedLevel(int maxLevelCount)
    {
        Data ??= new UserData();
        return Mathf.Clamp(Data.lastPlayedLevel, 0, Mathf.Max(0, maxLevelCount - 1));
    }

    public void SaveAll()
    {
        storage.Save(Data);
    }
}
