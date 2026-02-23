using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    [SerializeField] private string vehicleTag = "Player";
    [SerializeField] private LevelEndController endController;

    private bool triggered;

    private void Awake()
    {
        AutoFindController();
    }

    private void OnEnable()
    {
        // на случай если объект был заспавнен/включен позже
        AutoFindController();
    }

    private void AutoFindController()
    {
        if (endController != null) return;

        // найти в сцене
        endController = FindFirstObjectByType<LevelEndController>();

        if (endController == null)
            Debug.LogWarning("[FinishTrigger] LevelEndController not found in scene.");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;

        var rb = other.attachedRigidbody;
        if (rb != null)
        {
            if (!rb.CompareTag(vehicleTag)) return;
        }
        else
        {
            if (!other.transform.root.CompareTag(vehicleTag)) return;
        }

        triggered = true;

        if (endController == null)
            AutoFindController();

        endController?.Win();
    }

    public void ResetTrigger()
    {
        triggered = false;

        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = true;
    }
}