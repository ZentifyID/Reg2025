using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Dropdown))]
public class LocalizedDropdownOptions : MonoBehaviour
{
    [SerializeField] private List<string> optionKeys = new(); // по индексу

    private TMP_Dropdown dropdown;

    private void Awake()
    {
        dropdown = GetComponent<TMP_Dropdown>();
    }

    private void OnEnable()
    {
        Apply();
        if (LocalizationService.Instance != null)
            LocalizationService.Instance.LanguageChanged += Apply;
    }

    private void OnDisable()
    {
        if (LocalizationService.Instance != null)
            LocalizationService.Instance.LanguageChanged -= Apply;
    }

    private void Apply()
    {
        if (LocalizationService.Instance == null) return;
        if (dropdown.options.Count != optionKeys.Count) return;

        for (int i = 0; i < dropdown.options.Count; i++)
            dropdown.options[i].text = LocalizationService.Instance.Get(optionKeys[i]);

        dropdown.RefreshShownValue();
    }
}
