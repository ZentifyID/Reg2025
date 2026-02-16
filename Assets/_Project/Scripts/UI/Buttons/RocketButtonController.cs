using UnityEngine;
using UnityEngine.UI;

public class RocketButtonController : MonoBehaviour
{
    [SerializeField] private AssemblyController assembly;
    [SerializeField] private VehicleMotor2D vehicle;
    [SerializeField] private Button button;

    private void Awake()
    {
        if (button == null) button = GetComponent<Button>();
        if (assembly == null) assembly = FindFirstObjectByType<AssemblyController>();
        if (vehicle == null) vehicle = FindFirstObjectByType<VehicleMotor2D>();
    }

    private void Start() => Refresh();

    private void Update()
    {
        if (!gameObject.activeSelf) return;

        var pod = vehicle != null ? vehicle.RocketPod : null;
        if (button != null)
            button.interactable = (pod != null && pod.CanFire);
    }

    public void Refresh()
    {
        bool show =
            assembly != null &&
            vehicle != null &&
            assembly.Phase == GamePhase.Run &&
            vehicle.HasRocket &&
            vehicle.RocketPod != null;

        gameObject.SetActive(show);
    }

    public void OnClick()
    {
        var pod = vehicle != null ? vehicle.RocketPod : null;
        pod?.Fire();
    }
}
