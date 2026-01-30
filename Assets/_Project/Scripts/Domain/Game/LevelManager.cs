using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private LevelDatas levelDatas;

    [Header("Scene refs")]
    [SerializeField] private Transform levelDesignRoot;      // LevelDesignRoot
    [SerializeField] private Transform vehicleSpawnPoint;    // где спавнится транспорт
    [SerializeField] private TMP_Text levelText;

    [Header("Other systems")]
    [SerializeField] private AssemblyController assemblyController;
    [SerializeField] private LevelEndController endController; // твой Win/Lose контроллер
    [SerializeField] private CoinWallet wallet;

    private UserDataStorage storage;
    private UserData data;

    private GameObject currentVehicle;

    public int CurrentLevelIndex { get; private set; } = 0;

    private void Awake()
    {
        storage = new UserDataStorage();
        data = storage.LoadOrCreate() ?? new UserData { coins = 0 };

        // если wallet живёт через DontDestroyOnLoad — можно найти так:
        if (wallet == null) wallet = CoinWallet.Instance != null ? CoinWallet.Instance : FindFirstObjectByType<CoinWallet>();

        // стартуем с последнего открытого
        CurrentLevelIndex = Mathf.Clamp(data.lastUnlockedLevel, 0, Mathf.Max(0, levelDatas.Count - 1));
    }

    private void Start()
    {
        ApplyLevel(CurrentLevelIndex);
    }

    public void ApplyLevel(int levelIndex)
    {
        CurrentLevelIndex = Mathf.Clamp(levelIndex, 0, levelDatas.Count - 1);

        var lvl = levelDatas.GetByIndex(CurrentLevelIndex);
        if (lvl == null) return;

        EnableLevelDesign(lvl.levelDesignIndex);

        SpawnVehicle(lvl.vehiclePrefab);

        assemblyController.SetupForLevel(level);

        if (assemblyController != null)
            assemblyController.SetupForLevel(lvl);

        if (endController != null)
            endController.ResetToPlaying();

        if (levelText != null)
            levelText.text = $"{CurrentLevelIndex + 1}";
    }

    private void EnableLevelDesign(int childIndex)
    {
        if (levelDesignRoot == null) return;

        for (int i = 0; i < levelDesignRoot.childCount; i++)
            levelDesignRoot.GetChild(i).gameObject.SetActive(i == childIndex);
    }

    private void SpawnVehicle(GameObject prefab)
    {
        if (prefab == null || vehicleSpawnPoint == null) return;

        if (currentVehicle != null)
            Destroy(currentVehicle);

        currentVehicle = Instantiate(prefab, vehicleSpawnPoint.position, vehicleSpawnPoint.rotation);
    }

    public void OnWin()
    {
        var lvl = levelDatas.GetByIndex(CurrentLevelIndex);
        if (lvl != null && wallet != null)
            wallet.Add(lvl.rewardCoins);

        int next = CurrentLevelIndex + 1;
        if (next >= levelDatas.Count) next = 0;

        data.lastUnlockedLevel = Mathf.Max(data.lastUnlockedLevel, next);
        storage.Save(data);
    }

    public void NextLevelButton()
    {
        int next = CurrentLevelIndex + 1;
        if (next >= levelDatas.Count) next = 0;
        ApplyLevel(next);
    }

    public void OnLose()
    {
        data.lastUnlockedLevel = 0;
        storage.Save(data);
    }
}
