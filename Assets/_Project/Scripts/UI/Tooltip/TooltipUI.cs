using UnityEngine;
using TMPro;

public class TooltipUI : MonoBehaviour
{
    [SerializeField] private RectTransform background;
    [SerializeField] private TMP_Text tooltipText;
    [SerializeField] private Vector2 offset = new Vector2(20, -20);

    public void Show(string text)
    {
        tooltipText.text = text;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
