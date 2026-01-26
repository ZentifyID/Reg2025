using UnityEngine;

public class GameStartController : MonoBehaviour
{
    [Header("Assembly UI (hide on Start)")]
    [SerializeField] private GameObject assemblyUIRoot;   // панель/канвас сборки (кнопки выбора, слоты, подсветка)
    [SerializeField] private GameObject startButton;      // сама кнопка Start (чтобы тоже исчезла)

    [Header("Gameplay UI (show on Start)")]
    [SerializeField] private GameObject gameplayUIRoot;   // UI игры (крылья/рестарт и т.д.)

    [Header("Gameplay logic")]
    [SerializeField] private AssemblyController assemblyController;
    [SerializeField] private VehicleStart2D vehicleStart;
    [SerializeField] private VehicleMotor2D motor;
    [SerializeField] private WingsButtonController wingsButton;

    public void StartRun()
    {
        // 1) Переводим фазу
        if (assemblyController != null)
            assemblyController.OnStartRun();

        // 2) Прячем сборку
        if (assemblyUIRoot != null)
            assemblyUIRoot.SetActive(false);

        if (startButton != null)
            startButton.SetActive(false);

        // 3) Включаем геймплейный UI
        if (gameplayUIRoot != null)
            gameplayUIRoot.SetActive(true);

        // 4) Включаем физику и едем
        if (vehicleStart != null)
            vehicleStart.StartRun();

        if (motor != null)
            motor.StartMoving();

        // 5) Обновляем кнопку крыльев (покажется только если есть крылья)
        if (wingsButton != null)
            wingsButton.Refresh();
    }

}
