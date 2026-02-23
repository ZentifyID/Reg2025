using UnityEngine;
using UnityEngine.UI;

public class SpikesButtonController : MonoBehaviour
{
    [SerializeField] private VehicleMotor2D vehicle;
    [SerializeField] private AssemblyController assembly;
    [SerializeField] private Button button;

    private void Awake()
    {
        if (button == null) button = GetComponent<Button>();
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
            vehicle.HasSpikedWheels; // ключевое условие

        gameObject.SetActive(canUse);
        if (button != null) button.interactable = canUse;
    }

    public void ToggleSpikes()
    {
        if (vehicle == null) return;
        vehicle.ToggleSpikes();
    }
}