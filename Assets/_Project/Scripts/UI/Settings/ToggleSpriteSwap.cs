using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleSpriteSwap : MonoBehaviour
{
    [SerializeField] private Image targetImage;
    [SerializeField] private Sprite offSprite;
    [SerializeField] private Sprite onSprite;

    private Toggle toggle;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnValueChanged);
        OnValueChanged(toggle.isOn); // применить сразу при старте
    }

    private void OnDestroy()
    {
        if (toggle != null)
            toggle.onValueChanged.RemoveListener(OnValueChanged);
    }

    private void OnValueChanged(bool isOn)
    {
        if (targetImage == null) return;
        targetImage.sprite = isOn ? onSprite : offSprite;
    }
}
