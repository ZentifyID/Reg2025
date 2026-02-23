using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private LevelDatas levelDatas;

    [Header("Scene refs")]
    [SerializeField] private Transform levelRoot;
    [SerializeField] private Transform vehicleSpawnPoint;
    [SerializeField] private TMP_Text levelText;

    [Header("Other systems")]
    [SerializeField] private AssemblyController assemblyController;
    [SerializeField] private LevelEndController endController;
    [SerializeField] private CoinWallet wallet;
    [SerializeField] private SlotAnchorBinder slotAnchorBinder;

    public int CurrentLevelIndex { get; private set; } = 0;

    private void Awake()
    {
        if (wallet == null)
            wallet = CoinWallet.Instance != null ? CoinWallet.Instance : FindFirstObjectByType<CoinWallet>();

        CurrentLevelIndex = wallet != null
            ? wallet.GetSavedLevel(levelDatas.Count)
            : 0;
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

        BuildLevelFromData(lvl);

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
        {
            roverVisual.SetModel(lvl.vehiclePrefab);
            if (slotAnchorBinder != null) slotAnchorBinder.Rebind();
        }

        if (assemblyController != null)
            assemblyController.SetupForLevel(lvl);

        var startUI = FindFirstObjectByType<GameStartController>();
        if (startUI != null)
        {
            if (motor != null) startUI.SetMotor(motor);
            startUI.ResetToAssembly();
        }

        if (endController != null)
            endController.ResetToPlaying();

        if (levelText != null)
            levelText.text = $"{CurrentLevelIndex + 1}";

        if (wallet != null)
            wallet.SaveProgress(CurrentLevelIndex);
    }

    private void BuildLevelFromData(LevelData lvl)
    {
        if (levelRoot == null)
        {
            Debug.LogError("[LevelManager] levelRoot is NULL");
            return;
        }

        // очистить старый уровень
        for (int i = levelRoot.childCount - 1; i >= 0; i--)
            Destroy(levelRoot.GetChild(i).gameObject);

        // заспавнить объекты
        if (lvl.placedObjects != null)
        {
            foreach (var p in lvl.placedObjects)
            {
                if (string.IsNullOrWhiteSpace(p.prefabPath))
                {
                    Debug.LogWarning("[LevelManager] PlacedObjectData.prefabPath is NULL/empty");
                    continue;
                }

                var prefab = Resources.Load<GameObject>(p.prefabPath);
                if (prefab == null)
                {
                    Debug.LogWarning($"[LevelManager] Prefab not found in Resources: {p.prefabPath}");
                    continue;
                }

                var go = Instantiate(
                    prefab,
                    p.position,
                    Quaternion.Euler(0, 0, p.rotationZ),
                    levelRoot
                );

                var tag = go.GetComponent<PlacedObjectTag>();
                if (tag == null) tag = go.AddComponent<PlacedObjectTag>();
                tag.prefabPath = p.prefabPath;
            }
        }

        ResetFinishInLevelRoot();
        ResetObstaclesInLevelRoot();
    }

    private void ResetFinishInLevelRoot()
    {
        if (levelRoot == null) return;
        var finish = levelRoot.GetComponentInChildren<FinishTrigger>(true);
        if (finish != null) finish.ResetTrigger();
    }

    private void ResetObstaclesInLevelRoot()
    {
        if (levelRoot == null) return;
        var obstacles = levelRoot.GetComponentsInChildren<DestructibleObstacle>(true);
        foreach (var o in obstacles)
            o.ResetObstacle();
    }

    public void OnWin() => OnWin(1);

    public void OnWin(int rewardMultiplier = 1)
    {
        var lvl = levelDatas.GetByIndex(CurrentLevelIndex);
        if (lvl != null && wallet != null)
        {
            int multiplier = Mathf.Max(1, rewardMultiplier);
            wallet.Add(lvl.rewardCoins * multiplier);
        }

        if (wallet != null && wallet.Data != null)
        {
            int next = CurrentLevelIndex + 1;
            if (next >= levelDatas.Count) next = 0;

            wallet.Data.lastUnlockedLevel = Mathf.Max(wallet.Data.lastUnlockedLevel, next);
            wallet.SaveAll();
        }
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

    public void OnLose() { }
}
