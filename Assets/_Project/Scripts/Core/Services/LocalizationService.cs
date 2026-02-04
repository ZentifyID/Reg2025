using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LocalizationService : MonoBehaviour
{
    public static LocalizationService Instance { get; private set; }

    public static event Action ServiceReady;

    public event Action LanguageChanged;

    [SerializeField] private string startLanguageCode = "en";

    private Dictionary<string, string> localizedText = new();

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        SetLanguage(startLanguageCode);

        ServiceReady?.Invoke();
    }

    public void SetLanguage(string languageCode)
    {
        LoadLanguage(languageCode);
        LanguageChanged?.Invoke();
    }

    public string Get(string key)
    {
        if (string.IsNullOrEmpty(key)) return "";
        return localizedText.TryGetValue(key, out var value) ? value : $"#{key}";
    }

    private void LoadLanguage(string languageCode)
    {
        string path = Path.Combine(Application.streamingAssetsPath, $"Localization_{languageCode}.json");
        if (!File.Exists(path))
        {
            Debug.LogError($"Localization file not found: {path}");
            localizedText = new();
            return;
        }

        string json = File.ReadAllText(path);
        var data = JsonUtility.FromJson<LocalizationData>(json);

        localizedText = data != null ? data.ToDictionary() : new();
    }
}
