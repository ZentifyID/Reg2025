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
    [SerializeField] private GameObject propellerPrefab;
    [SerializeField] private GameObject spikedWheelsPrefab;
    [SerializeField] private GameObject rocketPrefab;

    [Header("UI")]
    [SerializeField] private GameObject startButton;

    [Header("Ability Check")]
    [SerializeField] private VehicleMotor2D vehicle;
    [SerializeField] private WingsButtonController wingsButton;
    [SerializeField] private PropellerButtonController propellerButton;

    [Header("Item Buttons")]
    [SerializeField] private GameObject wheelsAButton;
    [SerializeField] private GameObject wheelsBButton;
    [SerializeField] private GameObject wingsButtonUI;
    [SerializeField] private GameObject propellerButtonUI;
    [SerializeField] private GameObject spikedWheelsAButton;
    [SerializeField] private GameObject spikedWheelsBButton;
    [SerializeField] private GameObject rocketButton;

    private ItemType? selectedItem;
    private HashSet<ItemType> required = new HashSet<ItemType>();

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
        {
            vehicle.SetHasWings(true);
        }

        if (selectedItem == ItemType.Propeller && vehicle != null)
        {
            vehicle.SetHasPropeller(true);
            Debug.Log("[Assembly] Propeller installed, HasPropeller=" + vehicle.HasPropeller);
        }

        if (selectedItem == ItemType.Rocket && vehicle != null)
        {
            vehicle.SetHasRocket(true);
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
            ItemType.Propeller => propellerPrefab,
            ItemType.SpikedWheelsA => spikedWheelsPrefab,
            ItemType.SpikedWheelsB => spikedWheelsPrefab,
            ItemType.Rocket => rocketPrefab,

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
        if (startButton == null) return;

        if (required == null || required.Count == 0)
        {
            startButton.SetActive(true);
            return;
        }

        bool ok = true;

        foreach (var req in required)
        {
            var slot = slots.FirstOrDefault(s => s != null && s.Accepts == req);
            if (slot == null || !slot.IsOccupied)
            {
                ok = false;
                break;
            }
        }
        startButton.SetActive(ok);
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
        required = (level != null && level.requiredItems != null)
            ? new HashSet<ItemType>(level.requiredItems)
            : new HashSet<ItemType>();

        Phase = GamePhase.Assembly;
        selectedItem = null;

        if (vehicle != null)
            slots = vehicle.GetComponentsInChildren<AttachmentSlot>(true).ToList();

        ApplySlotActiveByLevel();

        if (startButton != null)
        {
            startButton.SetActive(false);
        }

        if (vehicle != null)
        {
            vehicle.SetHasWings(false);
            vehicle.SetHasPropeller(false);
            vehicle.SetHasRocket(false);
        }

        if (wingsButton != null)
        {
            wingsButton.Refresh();
        }

        if (propellerButton != null)
        {
            propellerButton.Refresh();
        }

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

        bool Has(ItemType t) => required.Contains(t);

        SetButton(wheelsAButton, Has(ItemType.WheelsA));
        SetButton(wheelsBButton, Has(ItemType.WheelsB));
        SetButton(wingsButtonUI, Has(ItemType.Wings));
        SetButton(propellerButtonUI, Has(ItemType.Propeller));
        SetButton(spikedWheelsAButton, Has(ItemType.SpikedWheelsA));
        SetButton(spikedWheelsBButton, Has(ItemType.SpikedWheelsB));
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

            bool needed = required != null && required.Contains(s.Accepts);

            // если слот не нужен Ч чистим установленный предмет, чтобы не оставалс€ в пам€ти
            if (!needed)
                s.Clear();

            // главное требование: включить/выключить сам слот
            s.gameObject.SetActive(needed);
        }
    }
}
