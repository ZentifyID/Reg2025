using UnityEngine;

public class LevelEndController : MonoBehaviour
{
    [SerializeField] private GameObject finishUI;
    [SerializeField] private GameObject gameplayUI;
    [SerializeField] private VehicleMotor2D motor;     // чтобы остановить автодвижение
    [SerializeField] private Rigidbody2D vehicleRb;    // чтобы заморозить

    public void Win()
    {
        if (motor != null) motor.StopMoving();

        if (vehicleRb != null)
        {
            vehicleRb.linearVelocity = Vector2.zero;
            vehicleRb.angularVelocity = 0f;
            // можно заморозить, чтобы не катился дальше:
            vehicleRb.simulated = false;
        }

        if (gameplayUI != null) gameplayUI.SetActive(false);
        if (finishUI != null) finishUI.SetActive(true);
    }
}
