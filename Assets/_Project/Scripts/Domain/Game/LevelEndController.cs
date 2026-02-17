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
    [SerializeField] private VehicleMotor2D vehicle;

    [Header("Level Manager")]
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private LevelRewardAdWindow rewardAdWindow;

    public void Win()
    {
        if (CurrentState != State.Playing) return;
        CurrentState = State.Won;

        if (vehicle != null) vehicle.StopMoving();

        if (gameplayUI != null) gameplayUI.SetActive(false);
        if (loseUI != null) loseUI.SetActive(false);

        if (finishUI != null) finishUI.SetActive(true);
    }

    public void Lose()
    {
        if (CurrentState != State.Playing) return;
        CurrentState = State.Lost;

        if (vehicle != null) vehicle.StopMoving();

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

    public void SetVehicle(VehicleMotor2D newVehicle)
    {
        vehicle = newVehicle;
    }

    public void OnFinishSkip()
    {
        if (finishUI != null) finishUI.SetActive(false);
        levelManager.CompleteLevelAndStartNext(1);
    }

    public void OnFinishWatchAd()
    {
        if (rewardAdWindow == null)
        {
            OnFinishSkip();
            return;
        }

        if (finishUI != null) finishUI.SetActive(false);

        rewardAdWindow.Open(
            levelManager,
            onCompleted: (multiplier) =>
            {
                levelManager.CompleteLevelAndStartNext(multiplier);
            },
            onCanceled: () =>
            {
                if (finishUI != null) finishUI.SetActive(true);
            }
        );
    }

    public void OnLoseRetry()
    {
        if (loseUI != null) loseUI.SetActive(false);
        levelManager.ApplyLevel(0);
    }

    public void OnLoseWatchAd()
    {
        if (rewardAdWindow == null)
        {
            OnLoseRetry();
            return;
        }

        if (loseUI != null) loseUI.SetActive(false);

        rewardAdWindow.Open(
            levelManager,
            onCompleted: _ => levelManager.ApplyLevel(levelManager.CurrentLevelIndex),
            onCanceled: () => { levelManager.ApplyLevel(0); }
        );
    }
}
