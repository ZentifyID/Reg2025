using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    [SerializeField] private string vehicleTag = "Player";
    [SerializeField] private LevelEndController endController;

    private bool triggered;

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
        if (endController != null)
            endController.Win();
    }

    public void ResetTrigger()
    {
        triggered = false;
        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = true;
    }
}
