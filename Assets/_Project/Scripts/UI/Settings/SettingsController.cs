using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [Header("Window")]
    [SerializeField] private GameObject settingsRoot;          // SettingsWindow object
    [SerializeField] private Button closeButton;

    [Header("Controls")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Toggle audioToggle;
    [SerializeField] private TMP_Dropdown languageDropdown;
    [SerializeField] private Button resetButton;

    [Header("Confirm Save Panel")]
    [SerializeField] private GameObject confirmPanel;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;
    [SerializeField] private TMP_Text noTimerText;            // например "10, 9, 8..."

    private const string FileName = "UserGameSettings.json";

    private GameSettingsData baseDefaults;      // BaseGameSettings (дефолт)
    private GameSettingsData loadedSettings;    // что было применено/загружено
    private GameSettingsData workingSettings;   // текущие изменения в UI
    private GameSettingsData snapshotBeforeEdit;// снимок до правок (для отката)

    private Coroutine noCountdownRoutine;
    private bool suppressUiEvents; // чтобы не ловить события при заполнении UI

    private void Awake()
    {
        // 1) Дефолт по ТЗ (можно потом грузить из BaseGameSettings.json / SO)
        baseDefaults = new GameSettingsData
        {
            resolutionIndex = 3, // 1920x1080
            fullscreen = true,
            audioEnabled = true,
            languageIndex = 0 // ru по умолчанию (или 1 en — как нужно)
        };

        // 2) Загружаем пользовательские (если нет — используем дефолт)
        loadedSettings = LoadOrDefault(baseDefaults).Clone();
        workingSettings = loadedSettings.Clone();
        snapshotBeforeEdit = loadedSettings.Clone();

        // 3) Подписки UI
        closeButton.onClick.AddListener(OnClosePressed);

        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
        audioToggle.onValueChanged.AddListener(OnAudioChanged);
        languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
        resetButton.onClick.AddListener(OnResetPressed);

        yesButton.onClick.AddListener(OnConfirmYes);
        noButton.onClick.AddListener(OnConfirmNo);

        // 4) Заполняем UI из настроек
        ApplyToUI(workingSettings);

        // 5) Применяем настройки в систему (чтобы реально работало при старте)
        ApplyToSystem(loadedSettings);

        // Confirm panel скрыт
        if (confirmPanel != null) confirmPanel.SetActive(false);
    }

    public void Open()
    {
        snapshotBeforeEdit = loadedSettings.Clone(); // фиксируем “что было”
        workingSettings = loadedSettings.Clone();
        ApplyToUI(workingSettings);
        settingsRoot.SetActive(true);
    }

    private void OnClosePressed()
    {
        // Если изменений нет — просто закрываем
        if (workingSettings.EqualsTo(snapshotBeforeEdit))
        {
            settingsRoot.SetActive(false);
            return;
        }

        // Иначе показываем подтверждение
        ShowConfirm();
    }

    private void ShowConfirm()
    {
        if (confirmPanel == null) return;

        confirmPanel.SetActive(true);

        // Запускаем таймер 10 сек на кнопке "Нет"
        if (noCountdownRoutine != null) StopCoroutine(noCountdownRoutine);
        noCountdownRoutine = StartCoroutine(NoCountdown(10));
    }

    private IEnumerator NoCountdown(int seconds)
    {
        int t = seconds;
        UpdateNoTimerText(t);


        while (t > 0)
        {
            yield return new WaitForSeconds(1f);
            t--;
            UpdateNoTimerText(t);
        }

        // Таймер истёк = как будто нажали "Нет"
        OnConfirmNo();
    }

    private void UpdateNoTimerText(int secondsLeft)
    {
        if (noTimerText == null) return;

        noTimerText.text = secondsLeft > 0
            ? $"({secondsLeft})"
            : string.Empty;
    }


    private void OnConfirmYes()
    {
        HideConfirm();

        // Применяем то, что выбрано сейчас
        loadedSettings = workingSettings.Clone();
        ApplyToSystem(loadedSettings);
        Save(loadedSettings);

        settingsRoot.SetActive(false);
    }

    private void OnConfirmNo()
    {
        HideConfirm();

        // Откат к снапшоту до редактирования
        workingSettings = snapshotBeforeEdit.Clone();
        ApplyToSystem(snapshotBeforeEdit);
        ApplyToUI(workingSettings);

        settingsRoot.SetActive(false);
    }

    private void HideConfirm()
    {
        if (noCountdownRoutine != null)
        {
            StopCoroutine(noCountdownRoutine);
            noCountdownRoutine = null;
        }

        if (noTimerText != null)
            noTimerText.text = string.Empty;

        if (confirmPanel != null)
            confirmPanel.SetActive(false);
    }

    private void OnResetPressed()
    {
        // Сброс к дефолту (но пока не сохраняем, пока не нажали Да)
        workingSettings = baseDefaults.Clone();
        ApplyToUI(workingSettings);
        ApplyToSystem(workingSettings);
    }

    // --- UI callbacks ---
    private void OnResolutionChanged(int index)
    {
        if (suppressUiEvents) return;
        workingSettings.resolutionIndex = index;
        ApplyResolution(index);
    }

    private void OnFullscreenChanged(bool isOn)
    {
        if (suppressUiEvents) return;
        workingSettings.fullscreen = isOn;
        ApplyFullscreen(isOn);
    }

    private void OnAudioChanged(bool isOn)
    {
        if (suppressUiEvents) return;
        workingSettings.audioEnabled = isOn;
        ApplyAudio(isOn);
    }

    private void OnLanguageChanged(int index)
    {
        if (suppressUiEvents) return;
        workingSettings.languageIndex = index;
        ApplyLanguage(index);
    }

    // --- Apply ---
    private void ApplyToUI(GameSettingsData data)
    {
        suppressUiEvents = true;

        resolutionDropdown.value = Mathf.Clamp(data.resolutionIndex, 0, ResolutionCatalog.Resolutions.Length - 1);
        resolutionDropdown.RefreshShownValue();

        fullscreenToggle.isOn = data.fullscreen;
        audioToggle.isOn = data.audioEnabled;

        languageDropdown.value = Mathf.Clamp(data.languageIndex, 0, languageDropdown.options.Count - 1);
        languageDropdown.RefreshShownValue();

        suppressUiEvents = false;
    }

    private void ApplyToSystem(GameSettingsData data)
    {
        ApplyFullscreen(data.fullscreen);
        ApplyResolution(data.resolutionIndex);
        ApplyAudio(data.audioEnabled);
        ApplyLanguage(data.languageIndex);
    }

    private void ApplyResolution(int index)
    {
        index = Mathf.Clamp(index, 0, ResolutionCatalog.Resolutions.Length - 1);
        var r = ResolutionCatalog.Resolutions[index];

        var mode = workingSettings.fullscreen
            ? FullScreenMode.FullScreenWindow
            : FullScreenMode.Windowed;

        Screen.SetResolution(r.x, r.y, mode);
    }

    private void ApplyFullscreen(bool isFullscreen)
    {
        Screen.fullScreenMode = isFullscreen
        ? FullScreenMode.FullScreenWindow   // нормальный fullscreen без смены монитора
        : FullScreenMode.Windowed;          // окно
    }

    private void ApplyAudio(bool enabled)
    {
        // Простейший глобальный mute
        AudioListener.pause = !enabled;
        AudioListener.volume = enabled ? 1f : 0f;

        // Если у тебя есть свой AudioService — лучше делегировать ему
        // audioService.SetMute(!enabled);
    }

    private void ApplyLanguage(int index)
    {
        // 0 = ru, 1 = en (приведи к своему списку)
        string code = index == 0 ? "ru" : "en";

        if (LocalizationService.Instance != null)
            LocalizationService.Instance.SetLanguage(code);
    }

    // --- Save/Load JSON ---
    private GameSettingsData LoadOrDefault(GameSettingsData fallback)
    {
        string path = GetPath();
        if (!File.Exists(path))
            return fallback;

        try
        {
            string json = File.ReadAllText(path);
            var data = JsonUtility.FromJson<GameSettingsData>(json);
            return data ?? fallback;
        }
        catch
        {
            return fallback;
        }
    }

    private void Save(GameSettingsData data)
    {
        string path = GetPath();
        Directory.CreateDirectory(Path.GetDirectoryName(path));

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
    }

    private string GetPath()
    {
        // хранить в persistentDataPath правильно для билда
        return Path.Combine(Application.persistentDataPath, FileName);
    }
}
