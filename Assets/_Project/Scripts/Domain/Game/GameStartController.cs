using UnityEngine;

public class GameStartController : MonoBehaviour
{
    [Header("Assembly UI (hide on Start)")]
    [SerializeField] private GameObject assemblyUIRoot;

    [Header("Gameplay UI (show on Start)")]
    [SerializeField] private GameObject gameplayUIRoot;

    [Header("Gameplay logic")]
    [SerializeField] private AssemblyController assemblyController;
    [SerializeField] private VehicleStart2D vehicleStart;
    [SerializeField] private VehicleMotor2D motor;
    [SerializeField] private WingsButtonController wingsButton;
    [SerializeField] private PropellerButtonController propellerButton;
    [SerializeField] private RocketButtonController rocketButton;


    public void SetMotor(VehicleMotor2D m) => motor = m;

    public void StartRun()
    {
        if (assemblyController != null)
            assemblyController.OnStartRun();

        if (assemblyUIRoot != null)
            assemblyUIRoot.SetActive(false);

        if (gameplayUIRoot != null)
            gameplayUIRoot.SetActive(true);

        var rb = motor != null ? motor.GetComponent<Rigidbody2D>() : null;
        if (rb != null)
        {
            rb.simulated = true;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        if (vehicleStart != null)
            vehicleStart.StartRun();

        if (motor != null)
            motor.StartMoving();

        if (wingsButton != null)
            wingsButton.Refresh();

        if (propellerButton != null)
            propellerButton.Refresh();

        if (rocketButton != null) rocketButton.Refresh();
    }

    public void ResetToAssembly()
    {
        if (assemblyUIRoot != null)
        {
            assemblyUIRoot.SetActive(true);

            var cg = assemblyUIRoot.GetComponentInChildren<CanvasGroup>(true);
            if (cg != null)
            {
                cg.alpha = 1f;
                cg.interactable = true;
                cg.blocksRaycasts = true;
            }
        }

        if (gameplayUIRoot != null)
            gameplayUIRoot.SetActive(false);

        if (wingsButton != null)
            wingsButton.Refresh();

        if (propellerButton != null)
            propellerButton.Refresh();

        if (rocketButton != null) rocketButton.Refresh();

        var rb = motor != null ? motor.GetComponent<Rigidbody2D>() : null;
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false;
        }
    }
}
