using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static AttachmentSlot;

public class AssemblyController : MonoBehaviour
{
    [Header("Slots on vehicle")]
    [SerializeField] private List<AttachmentSlot> slots;

    [Header("Prefabs to place")]
    [SerializeField] private GameObject wheelsFrontPrefab;          
    [SerializeField] private GameObject wheelsRearPrefab;           
    [SerializeField] private GameObject spikedWheelsFrontPrefab;    
    [SerializeField] private GameObject spikedWheelsRearPrefab;     

    [SerializeField] private GameObject wingsPrefab;
    [SerializeField] private GameObject propellerPrefab;
    [SerializeField] private GameObject rocketPrefab;

    [Header("UI")]
    [SerializeField] private GameObject startButton;

    [Header("Ability Check")]
    [SerializeField] private VehicleMotor2D vehicle;
    [SerializeField] private WingsButtonController wingsButton;
    [SerializeField] private PropellerButtonController propellerButton;
    [SerializeField] private SpikesButtonController spikesButton;

    [Header("Item Buttons")]
    [SerializeField] private GameObject[] wheelsButtons;
    [SerializeField] private GameObject[] spikedWheelsButtons;
    [SerializeField] private GameObject wingsButtonUI;
    [SerializeField] private GameObject propellerButtonUI;
    [SerializeField] private GameObject rocketButton;

    private ItemType? selectedItem;
    private HashSet<ItemType> required = new HashSet<ItemType>();
    private readonly HashSet<AttachmentSlot> spikedWheelSlots = new HashSet<AttachmentSlot>();
    public bool RequiresSpikedWheels => required != null && required.Contains(ItemType.SpikedWheels);

    public GamePhase Phase { get; private set; } = GamePhase.Assembly;

    private void Awake()
    {
        if (startButton != null) startButton.SetActive(false);
        ClearHighlights();
    }

    public void SelectItem(ItemType type)
    {
        selectedItem = type;
        UpdateHighlights();
    }

    public void ClearSelection()
    {
        selectedItem = null;
        ClearHighlights();
    }

    public void TryPlaceOnSlot(AttachmentSlot slot)
    {
        if (selectedItem == null) return;
        if (slot.IsOccupied) return;

        if (!SlotAccepts(slot, selectedItem.Value)) return;

        var prefab = GetPrefab(selectedItem.Value, slot);
        if (prefab == null) return;

        slot.Place(prefab);

        if (vehicle != null)
        {
            if (selectedItem == ItemType.SpikedWheels)
                vehicle.SetHasSpikedWheels(true);

            if (selectedItem == ItemType.Wheels)
                vehicle.SetHasSpikedWheels(false);
        }

        if (selectedItem == ItemType.Wings && vehicle != null)
            vehicle.SetHasWings(true);

        if (selectedItem == ItemType.Propeller && vehicle != null)
            vehicle.SetHasPropeller(true);

        if (selectedItem == ItemType.Rocket && vehicle != null)
            vehicle.SetHasRocket(true);

        if (selectedItem == ItemType.SpikedWheels && vehicle != null)
            vehicle.SetHasSpikedWheels(true);

        UpdateHighlights();
        CheckAllPlaced();
    }

    private bool SlotAccepts(AttachmentSlot slot, ItemType item)
    {
        if (slot.Accepts == item) return true;

        // универсальный слот колЄс
        if (slot.Accepts == ItemType.Wheels && (item == ItemType.Wheels || item == ItemType.SpikedWheels))
            return true;

        return false;
    }

    private GameObject GetPrefab(ItemType type, AttachmentSlot slot)
    {
        switch (type)
        {
            case ItemType.Wheels:
                return slot.wheelKind == WheelKind.Rear ? wheelsRearPrefab : wheelsFrontPrefab;

            case ItemType.SpikedWheels:
                return slot.wheelKind == WheelKind.Rear ? spikedWheelsRearPrefab : spikedWheelsFrontPrefab;

            case ItemType.Wings:
                return wingsPrefab;

            case ItemType.Propeller:
                return propellerPrefab;

            case ItemType.Rocket:
                return rocketPrefab;

            default:
                return null;
        }
    }

    private void UpdateHighlights()
    {
        ClearHighlights();
        if (selectedItem == null) return;

        foreach (var s in slots)
            s.SetHighlighted(s != null && s.gameObject.activeSelf && SlotAccepts(s, selectedItem.Value));
    }

    private void ClearHighlights()
    {
        foreach (var s in slots)
            if (s != null) s.SetHighlighted(false);
    }

    private void CheckAllPlaced()
    {
        if (startButton == null) return;

        if (required == null || required.Count == 0)
        {
            startButton.SetActive(true);
            return;
        }

        foreach (var req in required)
        {
            if (req == ItemType.Wheels || req == ItemType.SpikedWheels)
            {
                var wheelSlots = slots.Where(s => s != null && s.gameObject.activeSelf && s.Accepts == ItemType.Wheels).ToList();
                if (wheelSlots.Count == 0) { startButton.SetActive(false); return; }

                if (wheelSlots.Any(s => !s.IsOccupied))
                {
                    startButton.SetActive(false);
                    return;
                }
            }
            else
            {
                var relevant = slots.Where(s => s != null && s.gameObject.activeSelf && s.Accepts == req).ToList();
                if (relevant.Count == 0) { startButton.SetActive(false); return; }

                if (relevant.Any(s => !s.IsOccupied))
                {
                    startButton.SetActive(false);
                    return;
                }
            }
        }

        startButton.SetActive(true);
    }

    public void OnStartRun()
    {
        Phase = GamePhase.Run;

        wingsButton?.Refresh();
        propellerButton?.Refresh();
        spikesButton?.Refresh();

        ClearHighlights();

        if (startButton != null)
            startButton.SetActive(false);

        foreach (var s in slots)
        {
            if (s == null) continue;

            var col3d = s.GetComponent<Collider>();
            if (col3d != null) col3d.enabled = false;
        }
    }

    public void SetupForLevel(LevelData level)
    {
        required = NormalizeRequired(level);

        if (level != null)
            level.requiredItems = required.ToList();

        Phase = GamePhase.Assembly;
        selectedItem = null;

        if (vehicle != null)
            slots = vehicle.GetComponentsInChildren<AttachmentSlot>(true).ToList();

        ApplySlotActiveByLevel();

        if (startButton != null)
            startButton.SetActive(false);

        if (vehicle != null)
        {
            vehicle.SetHasWings(false);
            vehicle.SetHasPropeller(false);
            vehicle.SetHasRocket(false);
            vehicle.SetHasSpikedWheels(false);
            vehicle.SetSpikesActive(false);
        }

        wingsButton?.Refresh();
        propellerButton?.Refresh();
        spikesButton?.Refresh();

        foreach (var s in slots)
        {
            if (s == null) continue;
            if (!s.gameObject.activeSelf) continue;

            foreach (var col2d in s.GetComponentsInChildren<Collider2D>(true))
                col2d.enabled = true;

            var col3d = s.GetComponent<Collider>();
            if (col3d != null) col3d.enabled = true;

            s.Clear();
        }

        void SetButton(GameObject go, bool active)
        {
            if (go != null) go.SetActive(active);
        }

        void SetButtons(GameObject[] arr, bool active)
        {
            if (arr == null) return;
            foreach (var go in arr)
                if (go != null) go.SetActive(active);
        }

        bool Has(ItemType t) => required.Contains(t);

        SetButtons(wheelsButtons, Has(ItemType.Wheels));
        SetButtons(spikedWheelsButtons, Has(ItemType.SpikedWheels));
        SetButton(wingsButtonUI, Has(ItemType.Wings));
        SetButton(propellerButtonUI, Has(ItemType.Propeller));
        SetButton(rocketButton, Has(ItemType.Rocket));

        ClearHighlights();
        CheckAllPlaced();
    }

    public void SetVehicle(VehicleMotor2D newVehicle)
    {
        vehicle = newVehicle;

        if (vehicle != null)
            slots = vehicle.GetComponentsInChildren<AttachmentSlot>(true).ToList();
    }

    private void ApplySlotActiveByLevel()
    {
        if (slots == null) return;

        foreach (var s in slots)
        {
            if (s == null) continue;

            bool needed;

            if (s.Accepts == ItemType.Wheels)
                needed = required != null && (required.Contains(ItemType.Wheels) || required.Contains(ItemType.SpikedWheels));
            else
                needed = required != null && required.Contains(s.Accepts);

            if (!needed)
                s.Clear();

            s.gameObject.SetActive(needed);
        }
    }

    private HashSet<ItemType> NormalizeRequired(LevelData level)
    {
        var set = (level != null && level.requiredItems != null)
            ? new HashSet<ItemType>(level.requiredItems)
            : new HashSet<ItemType>();

        bool hasWheels = set.Contains(ItemType.Wheels);
        bool hasSpiked = set.Contains(ItemType.SpikedWheels);

        // если шипованные выбраны Ч обычные убираем
        if (hasSpiked)
            set.Remove(ItemType.Wheels);

        // если вообще ничего про колЄса не выбрано Ч по умолчанию Wheels
        if (!hasSpiked && !hasWheels)
            set.Add(ItemType.Wheels);

        if (set.Contains(ItemType.Wings) && set.Contains(ItemType.Rocket))
            set.Remove(ItemType.Rocket);

        return set;
    }
}
