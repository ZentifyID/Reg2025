using UnityEngine;

public class EditorSettingsButton : MonoBehaviour
{
    [SerializeField] private GameObject EditorSettings;
    [SerializeField] private GameObject CarSettings;

    public void Toggle()
    {
        if (EditorSettings == null) return;
        if (CarSettings == null) return;

        EditorSettings.SetActive(!EditorSettings.activeSelf);
        CarSettings.SetActive(false);
    }
}
