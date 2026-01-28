using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    [SerializeField] private string vehicleTag = "Player"; // или "Vehicle"
    [SerializeField] private LevelEndController endController;

    private bool triggered;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;

        if (!other.CompareTag(vehicleTag))
            return;

        triggered = true;

        if (endController != null)
            endController.Win();
    }
}
