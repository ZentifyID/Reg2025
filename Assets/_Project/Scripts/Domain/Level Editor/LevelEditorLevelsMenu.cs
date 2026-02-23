using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelEditorLevelsMenu : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private LevelDatas levelDatas;

    [Header("UI")]
    [SerializeField] private Transform levelsListParent;      // контейнер для кнопок уровней (Content)
    [SerializeField] private LevelSlotButtonUI levelButtonPrefab;

    [Header("Other panels")]
    [SerializeField] private GameObject levelsPanel;          // панель уровней (опц.)
    [SerializeField] private GameObject carSettingsPanel;     // панель машины (ты уже сделал toggle, но оставлю)

    [Header("Finish Placer")]
    [SerializeField] private AutoFinishByMaxX autoFinish;

    private readonly List<LevelSlotButtonUI> _buttons = new();
    private int _selectedIndex;

    public int SelectedIndex => _selectedIndex;
    public LevelData SelectedLevel => (_selectedIndex >= 0 && _selectedIndex < levelDatas.Count) ? levelDatas.levels[_selectedIndex] : null;

    public PlacementManager placement;
    public ItemPrefabLibrary library;

    private void Start()
    {
        RebuildButtons();
        if (levelDatas.Count > 0) SelectLevel(0);
    }

    // ---- UI CALLS ----

    public void OnAddLevel()
    {
        levelDatas.levels.Add(new LevelData());
        RebuildButtons();
        SelectLevel(levelDatas.Count - 1);
        MarkDirty();
    }

    public void OnDeleteSelectedLevel()
    {
        if (_selectedIndex < 0 || _selectedIndex >= levelDatas.Count) return;

        levelDatas.levels.RemoveAt(_selectedIndex);

        RebuildButtons();

        if (levelDatas.Count == 0) _selectedIndex = -1;
        else SelectLevel(Mathf.Clamp(_selectedIndex, 0, levelDatas.Count - 1));

        MarkDirty();
    }

    public void OnSaveSelected()
    {
        autoFinish?.PlaceFinish();

        var level = levelDatas.GetByIndex(_selectedIndex);
        if (level == null || placement == null) return;

        level.placedObjects = placement.ExportPlacedObjects();

        string path = Path.Combine(Application.persistentDataPath, "levels", $"level_{_selectedIndex + 1}.json");
        LevelJsonIO.SaveLevelToJson(level, path);

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(levelDatas);
        UnityEditor.AssetDatabase.SaveAssets();
#endif
    }

    public void OnLoadSelected()
    {
        var level = levelDatas.GetByIndex(_selectedIndex);
        if (level == null) return;

        placement.ImportPlacedObjects(level.placedObjects);
    }

    public void OnOpenCarSettings()
    {
        if (carSettingsPanel != null) carSettingsPanel.SetActive(true);
        if (levelsPanel != null) levelsPanel.SetActive(false);
    }

    // ---- INTERNAL ----

    private void RebuildButtons()
    {
        // очистка
        for (int i = levelsListParent.childCount - 1; i >= 0; i--)
            Destroy(levelsListParent.GetChild(i).gameObject);

        _buttons.Clear();

        for (int i = 0; i < levelDatas.Count; i++)
        {
            var btn = Instantiate(levelButtonPrefab, levelsListParent);
            btn.Init(i, SelectLevel);
            btn.SetNumber(i + 1);
            btn.SetSelected(i == _selectedIndex);
            _buttons.Add(btn);
        }
    }

    private void SelectLevel(int index)
    {
        _selectedIndex = Mathf.Clamp(index, 0, levelDatas.Count - 1);

        for (int i = 0; i < _buttons.Count; i++)
            _buttons[i].SetSelected(i == _selectedIndex);
    }

    private void MarkDirty()
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(levelDatas);
        UnityEditor.AssetDatabase.SaveAssets();
#endif
    }
}
