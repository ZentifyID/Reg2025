using UnityEngine;

public class SettingsWindow : MonoBehaviour
{
    [SerializeField] private GameObject root;

    public void Open()
    {
        root.SetActive(true);
    }

    public void Close()
    {
        root.SetActive(false);
    }
}