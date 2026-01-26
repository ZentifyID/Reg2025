using UnityEngine;
using UnityEngine.UI;

public class WingsButtonController : MonoBehaviour
{
    [SerializeField] private VehicleMotor2D vehicle;
    [SerializeField] private AssemblyController assembly;
    [SerializeField] private Button button;

    private void Awake()
    {
        if (button == null) button = GetComponent<Button>();
        Debug.Log("[WingsButton] Awake. vehicle=" + (vehicle != null) + " assembly=" + (assembly != null));
    }

    private void Start()
    {
        Refresh(); // чтобы при запуске сцены точно спряталась
    }

    public void Refresh()
    {
        bool canUse =
            vehicle != null &&
            assembly != null &&
            assembly.Phase == GamePhase.Run &&
            vehicle.HasWings;

        Debug.Log($"[WingsButton] Refresh canUse={canUse} phase={(assembly != null ? assembly.Phase.ToString() : "null")} hasWings={(vehicle != null && vehicle.HasWings)}");

        gameObject.SetActive(canUse);
        if (button != null) button.interactable = canUse;
    }

    public void UseWings()
    {
        Debug.Log("[WingsButton] Click!");
        if (vehicle == null) return;
        vehicle.TryUseWings();
    }
}
