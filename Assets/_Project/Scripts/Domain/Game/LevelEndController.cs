using UnityEngine;

public class LevelEndController : MonoBehaviour
{
    public enum State { Playing, Won, Lost }
    public State CurrentState { get; private set; } = State.Playing;

    [Header("UI")]
    [SerializeField] private GameObject finishUI;
    [SerializeField] private GameObject loseUI;
    [SerializeField] private GameObject gameplayUI;

    [Header("Vehicle")]
    [SerializeField] private Rigidbody2D vehicleRb;
    [SerializeField] private VehicleMotor2D motor;

    [Header("Level Manager")]
    [SerializeField] private LevelManager levelManager;

    public void Win()
    {
        if (CurrentState != State.Playing) return;
        CurrentState = State.Won;

        levelManager.OnWin();

        // стоп автодвижения, но физику не выключаем
        if (motor != null) motor.StopMoving();

        if (gameplayUI != null) gameplayUI.SetActive(false);
        if (loseUI != null) loseUI.SetActive(false);
        if (finishUI != null) finishUI.SetActive(true);
    }

    public void Lose()
    {
        if (CurrentState != State.Playing) return;
        CurrentState = State.Lost;

        levelManager.OnLose();

        if (motor != null) motor.StopMoving();

        if (gameplayUI != null) gameplayUI.SetActive(false);
        if (finishUI != null) finishUI.SetActive(false);
        if (loseUI != null) loseUI.SetActive(true);
    }

    public void ResetToPlaying()
    {
        CurrentState = State.Playing;
        if (finishUI != null) finishUI.SetActive(false);
        if (loseUI != null) loseUI.SetActive(false);
        if (gameplayUI != null) gameplayUI.SetActive(true);
    }
}
