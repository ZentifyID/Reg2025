using UnityEngine;

public class TooltipService : MonoBehaviour
{
    public static TooltipService Instance { get; private set; }

    [SerializeField] private TooltipUI tooltipUI;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Show(string text)
    {
        if (tooltipUI == null)
        {
            return;
        }

        tooltipUI.Show(text);
    }

    public void Hide()
    {
        if (tooltipUI == null)
            return;

        tooltipUI.Hide();
    }
}
