using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PropellerButtonController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private VehicleMotor2D vehicle;
    [SerializeField] private AssemblyController assembly;
    [SerializeField] private Button button;

    private void Awake()
    {
        if (button == null) button = GetComponent<Button>();

        if (vehicle == null) vehicle = FindFirstObjectByType<VehicleMotor2D>();
        if (assembly == null) assembly = FindFirstObjectByType<AssemblyController>();
    }

    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        bool canUse =
            vehicle != null &&
            assembly != null &&
            assembly.Phase == GamePhase.Run &&
            vehicle.HasPropeller;

        Debug.Log($"[PropellerButton] Refresh canUse={canUse} phase={(assembly != null ? assembly.Phase.ToString() : "null")} hasPropeller={(vehicle != null && vehicle.HasPropeller)}");

        gameObject.SetActive(canUse);
        if (button != null) button.interactable = canUse;

        if (!canUse && vehicle != null)
            vehicle.SetPropellerHeld(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (vehicle == null || assembly == null) return;
        if (assembly.Phase != GamePhase.Run) return;
        if (!vehicle.HasPropeller) return;

        vehicle.SetPropellerHeld(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (vehicle == null) return;
        vehicle.SetPropellerHeld(false);
    }
}
