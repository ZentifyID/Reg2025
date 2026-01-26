using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AssemblyController : MonoBehaviour
{
    [Header("Slots on vehicle")]
    [SerializeField] private List<AttachmentSlot> slots;

    [Header("Prefabs to place")]
    [SerializeField] private GameObject wheelsPrefab; // внешний вид колёс
    [SerializeField] private GameObject wingsPrefab;  // внешний вид крыльев

    [Header("UI")]
    [SerializeField] private GameObject startButton; // пока просто объект/кнопка

    [Header("Wings Check")]
    [SerializeField] private VehicleMotor2D vehicle;
    [SerializeField] private WingsButtonController wingsButton;

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

    // Вызываем из клика по слоту (см. SlotClick ниже)
    public void TryPlaceOnSlot(AttachmentSlot slot)
    {
        Debug.Log($"TryPlaceOnSlot: selected={selectedItem}, slotAccepts={slot.Accepts}, occupied={slot.IsOccupied}");

        if (selectedItem == null) return;
        if (slot.IsOccupied) return;
        if (slot.Accepts != selectedItem.Value) return;

        var prefab = GetPrefab(selectedItem.Value);
        if (prefab == null) return;

        slot.Place(prefab);
        if (selectedItem == ItemType.Wings)
        {
            vehicle.SetHasWings(true);
        }

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
        // MVP: считаем "все слоты заняты"
        // позже сделаем проверку по требованиям уровня (2 пары колёс + крылья).
        bool allOccupied = slots.All(s => s.IsOccupied);
        if (startButton != null) startButton.SetActive(allOccupied);
    }

    public void OnStartRun()
    {
        Phase = GamePhase.Run;
        ClearHighlights();
    }
}
