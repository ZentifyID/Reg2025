using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AssemblyController : MonoBehaviour
{
    [Header("Slots on vehicle")]
    [SerializeField] private List<AttachmentSlot> slots;

    [Header("Prefabs to place")]
    [SerializeField] private GameObject wheelsPrefab;
    [SerializeField] private GameObject wingsPrefab;

    [Header("UI")]
    [SerializeField] private GameObject startButton;

    [Header("Wings Check")]
    [SerializeField] private VehicleMotor2D vehicle;
    [SerializeField] private WingsButtonController wingsButton;

    [Header("Item Buttons")]
    [SerializeField] private GameObject wheelsAButton;
    [SerializeField] private GameObject wheelsBButton;
    [SerializeField] private GameObject wingsButtonUI;

    private ItemType? selectedItem;

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
        Debug.Log($"TryPlaceOnSlot: selected={selectedItem}, slotAccepts={slot.Accepts}, occupied={slot.IsOccupied}");

        if (selectedItem == null) return;
        if (slot.IsOccupied) return;
        if (slot.Accepts != selectedItem.Value) return;

        var prefab = GetPrefab(selectedItem.Value);
        if (prefab == null) return;

        slot.Place(prefab);
        if (selectedItem == ItemType.Wings && vehicle != null)
            vehicle.SetHasWings(true);

        UpdateHighlights();
        CheckAllPlaced();
    }

    private GameObject GetPrefab(ItemType type)
    {
        return type switch
        {
            ItemType.WheelsA => wheelsPrefab,
            ItemType.WheelsB => wheelsPrefab,
            ItemType.Wings => wingsPrefab,
            _ => null
        };
    }

    private void UpdateHighlights()
    {
        ClearHighlights();
        if (selectedItem == null) return;

        foreach (var s in slots)
            s.SetHighlighted(s.Accepts == selectedItem.Value);
    }

    private void ClearHighlights()
    {
        foreach (var s in slots)
            s.SetHighlighted(false);
    }

    private void CheckAllPlaced()
    {
        bool allOccupied = slots.All(s => s.IsOccupied);
        if (startButton != null) startButton.SetActive(allOccupied);
    }

    public void OnStartRun()
    {
        Phase = GamePhase.Run;
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
        Debug.Log($"[Assembly] SetupForLevel called. Level is null? {level == null}. " +
          $"requiredItems: {(level?.requiredItems == null ? "null" : string.Join(",", level.requiredItems))}");

        Phase = GamePhase.Assembly;
        selectedItem = null;

        if (vehicle != null)
            slots = vehicle.GetComponentsInChildren<AttachmentSlot>(true).ToList();

        if (startButton != null)
            startButton.SetActive(false);

        if (vehicle != null)
            vehicle.SetHasWings(false);

        if (wingsButton != null)
            wingsButton.Refresh();

        foreach (var s in slots)
        {
            if (s == null) continue;

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

        bool Has(ItemType t) => level != null && level.requiredItems != null && level.requiredItems.Contains(t);

        SetButton(wheelsAButton, Has(ItemType.WheelsA));
        SetButton(wheelsBButton, Has(ItemType.WheelsB));
        SetButton(wingsButtonUI, Has(ItemType.Wings));

        ClearHighlights();
    }

    public void SetVehicle(VehicleMotor2D newVehicle)
    {
        vehicle = newVehicle;

        if (vehicle != null)
            slots = vehicle.GetComponentsInChildren<AttachmentSlot>(true).ToList();
    }
}
