using UnityEngine;

public class CarSettingsButton : MonoBehaviour
{
    [SerializeField] private GameObject CarSettings;

    public void Toggle()
    {
        if (CarSettings == null) return;

        CarSettings.SetActive(true);
    }
}
