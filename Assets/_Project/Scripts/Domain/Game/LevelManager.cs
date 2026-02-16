using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private LevelDatas levelDatas;

    [Header("Scene refs")]
    [SerializeField] private Transform levelDesignRoot;
    [SerializeField] private Transform vehicleSpawnPoint;
    [SerializeField] private TMP_Text levelText;

    [Header("Other systems")]
    [SerializeField] private AssemblyController assemblyController;
    [SerializeField] private LevelEndController endController;
    [SerializeField] private CoinWallet wallet;
    [SerializeField] private SlotAnchorBinder slotAnchorBinder;

    private UserDataStorage storage;
    private UserData data;

    private GameObject currentVehicle;

    public int CurrentLevelIndex { get; private set; } = 0;

    private void Awake()
    {
        Debug.Log($"[LevelManager] Awake on {gameObject.name}", this);
        storage = new UserDataStorage();
        data = storage.LoadOrCreate() ?? new UserData { coins = 0 };

        if (wallet == null) wallet = CoinWallet.Instance != null ? CoinWallet.Instance : FindFirstObjectByType<CoinWallet>();

        CurrentLevelIndex = Mathf.Clamp(data.lastUnlockedLevel, 0, Mathf.Max(0, levelDatas.Count - 1));
    }

    private void Start()
    {
        ApplyLevel(CurrentLevelIndex);
    }

    public void ApplyLevel(int levelIndex)
    {
        CurrentLevelIndex = Mathf.Clamp(levelIndex, 0, levelDatas.Count - 1);
        Debug.Log($"[LevelManager] ApplyLevel {CurrentLevelIndex}. assemblyController null? {assemblyController == null}");

        var lvl = levelDatas.GetByIndex(CurrentLevelIndex);
        if (lvl == null) return;

        EnableLevelDesign(lvl.levelDesignIndex);
        ResetObstaclesForActiveDesign(lvl.levelDesignIndex);
        ResetFinishForActiveDesign(lvl.levelDesignIndex);

        var motor = FindFirstObjectByType<VehicleMotor2D>();
        if (motor != null)
        {
            if (vehicleSpawnPoint != null)
            {
                motor.transform.position = vehicleSpawnPoint.position;
                motor.transform.rotation = vehicleSpawnPoint.rotation;
            }

            var rb = motor.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                // rb.simulated = false; // если хочешь стоять до StartRun
            }

            if (assemblyController != null) assemblyController.SetVehicle(motor);
            if (endController != null) endController.SetVehicle(motor);
        }
        else
        {
            Debug.LogWarning("[LevelManager] VehicleMotor2D not found in scene!");
        }

        var roverVisual = FindFirstObjectByType<RoverVisual>();
        if (roverVisual != null)
            roverVisual.SetModel(lvl.vehiclePrefab);

        roverVisual.SetModel(lvl.vehiclePrefab);
        slotAnchorBinder.Rebind();

        if (assemblyController != null)
            assemblyController.SetupForLevel(lvl);

        var startUI = FindFirstObjectByType<GameStartController>();
        if (startUI != null)
        {
            if (motor != null) startUI.SetMotor(motor); // добавь метод
            startUI.ResetToAssembly();
        }

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

    public void OnWin()
    {
        OnWin(1);
    }

    public void OnWin(int rewardMultiplier = 1)
    {
        var lvl = levelDatas.GetByIndex(CurrentLevelIndex);
        if (lvl != null && wallet != null)
        {
            int multiplier = Mathf.Max(1, rewardMultiplier);
            wallet.Add(lvl.rewardCoins * multiplier);
        }

        int next = CurrentLevelIndex + 1;
        if (next >= levelDatas.Count) next = 0;

        data.lastUnlockedLevel = Mathf.Max(data.lastUnlockedLevel, next);
        storage.Save(data);
    }

    public void CompleteLevelAndStartNext(int rewardMultiplier = 1)
    {
        OnWin(rewardMultiplier);
        NextLevelButton();
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

    private void ResetFinishForActiveDesign(int childIndex)
    {
        if (levelDesignRoot == null) return;
        if (childIndex < 0 || childIndex >= levelDesignRoot.childCount) return;

        var active = levelDesignRoot.GetChild(childIndex);
        var finish = active.GetComponentInChildren<FinishTrigger>(true);
        if (finish != null) finish.ResetTrigger();
    }

    private void ResetObstaclesForActiveDesign(int childIndex)
    {
        if (levelDesignRoot == null) return;
        if (childIndex < 0 || childIndex >= levelDesignRoot.childCount) return;

        var root = levelDesignRoot.GetChild(childIndex);
        var obstacles = root.GetComponentsInChildren<DestructibleObstacle>(true);
        foreach (var o in obstacles)
            o.ResetObstacle();
    }
}
