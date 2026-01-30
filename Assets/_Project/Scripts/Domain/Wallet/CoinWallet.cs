using System;
using UnityEngine;

public class CoinWallet : MonoBehaviour
{
    public static CoinWallet Instance { get; private set; }

    public event Action<int> CoinsChanged;

    public UserData Data { get; private set; } = new UserData { coins = 0 };

    private UserDataStorage storage;

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

        CoinsChanged?.Invoke(Data.coins);
    }

    public void Add(int amount)
    {
        if (storage == null) storage = new UserDataStorage();
        Data ??= new UserData { coins = 0 };

        Data.coins += Mathf.Max(0, amount);
        storage.Save(Data);
        CoinsChanged?.Invoke(Data.coins);
    }

    public bool TrySpend(int amount)
    {
        if (amount <= 0) return true;

        if (storage == null) storage = new UserDataStorage();
        Data ??= new UserData { coins = 0 };

        if (Data.coins < amount) return false;

        Data.coins -= amount;
        storage.Save(Data);
        CoinsChanged?.Invoke(Data.coins);
        return true;
    }
}
