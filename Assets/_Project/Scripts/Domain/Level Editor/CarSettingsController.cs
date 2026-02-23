using System.Collections.Generic;
using UnityEngine;

public class CarSettingsController : MonoBehaviour
{
    [Header("Links")]
    [SerializeField] private LevelEditorLevelsMenu levelsMenu; // где выбранный уровень

    [Header("Models")]
    [SerializeField] private List<ModelButton> modelButtons = new(); // слева

    [Header("Items")]
    [SerializeField] private List<ItemToggle> itemToggles = new();   // справа

    // Заполни в инспекторе какие ItemType конфликтуют:
    [SerializeField] private ItemType rocketType = ItemType.Rocket;
    [SerializeField] private ItemType wingsType = ItemType.Wings;
    // [SerializeField] private ItemType spikedWheelsType = ItemType.SpikedWheels;

    private void OnEnable()
    {
        // подписки
        foreach (var mb in modelButtons)
            mb.Bind(OnSelectModel);

        foreach (var it in itemToggles)
            it.Bind(OnToggleItem);

        // отрисовать текущее
        var level = levelsMenu.SelectedLevel;
        if (level != null) ApplyFromLevel(level);
    }

    public void ApplyFromLevel(LevelData level)
    {
        // модели
        foreach (var mb in modelButtons)
            mb.SetSelected(level.vehiclePrefab == mb.vehiclePrefab);

        // предметы
        foreach (var it in itemToggles)
            it.SetSelected(level.requiredItems.Contains(it.itemType));
    }

    private void OnSelectModel(ModelButton button)
    {
        var level = levelsMenu.SelectedLevel;
        if (level == null) return;

        // одна модель
        level.vehiclePrefab = button.vehiclePrefab;

        foreach (var mb in modelButtons)
            mb.SetSelected(mb == button);

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(levelsMenu.GetComponent<LevelEditorLevelsMenu>()); // не обязательно
#endif
    }

    private void OnToggleItem(ItemToggle toggle, bool isOn)
    {
        var level = levelsMenu.SelectedLevel;
        if (level == null) return;

        var items = level.requiredItems;

        if (isOn)
        {
            // взаимное исключение Rocket/Wings
            if (toggle.itemType == rocketType && items.Contains(wingsType)) RemoveItem(wingsType);
            if (toggle.itemType == wingsType && items.Contains(rocketType)) RemoveItem(rocketType);

            if (!items.Contains(toggle.itemType))
                items.Add(toggle.itemType);
        }
        else
        {
            items.Remove(toggle.itemType);
        }

        // “обычные колёса по умолчанию”:
        // ничего делать не надо — просто если SpikedWheels не выбран, значит обычные.
        // (то есть НЕ храним обычные колёса в requiredItems)

        // обновим галочки на UI (на случай автосброса конфликта)
        ApplyFromLevel(level);

        void RemoveItem(ItemType t)
        {
            items.Remove(t);
            // снять галочку у соответствующего UI
            foreach (var it in itemToggles)
                if (it.itemType == t) it.SetSelected(false);
        }
    }
}

[System.Serializable]
public class ModelButton
{
    public GameObject vehiclePrefab;
    public GameObject checkmark; // галочка справа от кнопки
    public UnityEngine.UI.Button button;

    private System.Action<ModelButton> _onClick;

    public void Bind(System.Action<ModelButton> onClick)
    {
        _onClick = onClick;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => _onClick?.Invoke(this));
    }

    public void SetSelected(bool selected)
    {
        if (checkmark != null) checkmark.SetActive(selected);
    }
}

[System.Serializable]
public class ItemToggle
{
    public ItemType itemType;
    public GameObject checkmark;
    public UnityEngine.UI.Button button;

    private bool _selected;
    private System.Action<ItemToggle, bool> _onToggle;

    public void Bind(System.Action<ItemToggle, bool> onToggle)
    {
        _onToggle = onToggle;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            _selected = !_selected;
            SetSelected(_selected);
            _onToggle?.Invoke(this, _selected);
        });
    }

    public void SetSelected(bool selected)
    {
        _selected = selected;
        if (checkmark != null) checkmark.SetActive(selected);
    }
}
