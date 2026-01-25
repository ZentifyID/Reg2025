using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class LocalizedTMPText : MonoBehaviour
{
    [SerializeField] private string key;

    private TMP_Text textField;

    private void Awake()
    {
        textField = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        // если сервис уже есть Ч примен€ем сразу
        if (LocalizationService.Instance != null)
        {
            Apply();
            LocalizationService.Instance.LanguageChanged += Apply;
        }
        else
        {
            // ждЄм по€влени€ сервиса
            LocalizationService.ServiceReady += OnServiceReady;
        }
    }

    private void OnDisable()
    {
        LocalizationService.ServiceReady -= OnServiceReady;

        if (LocalizationService.Instance != null)
            LocalizationService.Instance.LanguageChanged -= Apply;
    }

    private void OnServiceReady()
    {
        LocalizationService.ServiceReady -= OnServiceReady;

        if (LocalizationService.Instance == null) return;

        Apply();
        LocalizationService.Instance.LanguageChanged += Apply;
    }

    public void Apply()
    {
        if (string.IsNullOrWhiteSpace(key)) return;
        if (LocalizationService.Instance == null) return;

        textField.text = LocalizationService.Instance.Get(key);
    }
}
