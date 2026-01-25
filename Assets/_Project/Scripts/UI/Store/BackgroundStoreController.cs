using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundStoreController : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject root;
    [SerializeField] private Button closeButton;

    [Header("Config + Wallet")]
    [SerializeField] private BackgroundsShopConfig config;
    [SerializeField] private CoinWallet wallet;

    [Header("Items")]
    [SerializeField] private List<BackgroundItemView> itemViews = new();
    [SerializeField] private Image selectedPreviewImage; // для изменения превью

    [Header("Video reward UI")]
    [SerializeField] private Button videoRewardButton;
    [SerializeField] private TMP_Text videoLabelText; // "Get\n150"
    [SerializeField] private TMP_Text videoTimerText; // "29s"

    [Header("Video Overlay")]
    [SerializeField] private GameObject videoOverlay;
    [SerializeField] private Button closeVideoButton;
    [SerializeField] private TMP_Text videoProgressText;

    private Coroutine cooldownRoutine;
    private Coroutine videoRoutine;

    // отдельный кулдаун для фонов (чтобы не смешивать с магазином монет)
    private const string VideoCooldownKey = "bg_video_cooldown_unix";

    private void Awake()
    {
        closeButton.onClick.AddListener(Close);

        videoRewardButton.onClick.AddListener(TryWatchVideoForCoins);
        closeVideoButton.onClick.AddListener(CancelVideo);

        if (videoOverlay != null) videoOverlay.SetActive(false);
    }

    private void OnEnable()
    {
        if (wallet != null)
            wallet.CoinsChanged += _ => RefreshAll();

        if (LocalizationService.Instance != null)
            LocalizationService.Instance.LanguageChanged += RefreshAll;
    }

    private void OnDisable()
    {
        if (wallet != null)
            wallet.CoinsChanged -= _ => RefreshAll();

        if (LocalizationService.Instance != null)
            LocalizationService.Instance.LanguageChanged -= RefreshAll;
    }

    public void Open()
    {
        root.SetActive(true);

        EnsureDefaultBackgroundOwned();
        // биндим карточки из конфига (если ещё не биндили)
        BindIfNeeded();

        RefreshAll();
        StartCooldownTick();
    }

    public void Close()
    {
        CancelVideo();
        StopCooldownTick();
        root.SetActive(false);
    }

    private void BindIfNeeded()
    {
        // Ожидаем, что itemViews уже назначены вручную (3 штуки),
        // а config.items тоже 3 штуки. Если у тебя всегда 3 — это ок.
        int n = Mathf.Min(itemViews.Count, config.items.Count);
        for (int i = 0; i < n; i++)
        {
            var item = config.items[i];
            itemViews[i].Bind(this, item.id, item.priceCoins, item.preview);
        }
    }

    public (bool isOwned, bool isSelected, bool canBuy) GetStateFor(string id, int price)
    {
        bool owned = wallet.Data != null && wallet.Data.IsOwned(id);
        bool selected = owned && wallet.Data.selectedBackgroundId == id;
        bool canBuy = wallet.Data != null && wallet.Data.coins >= price;
        return (owned, selected, canBuy);
    }

    public void OnSelectPressed(string id)
    {
        if (wallet.Data == null) return;

        // выбрать можно только купленное
        if (!wallet.Data.IsOwned(id))
            return;

        wallet.Data.selectedBackgroundId = id;
        RefreshAll();
        UpdateSelectedPreview();
    }

    public void OnBuyPressed(string id)
    {
        if (wallet.Data == null) return;

        var item = config.items.Find(x => x.id == id);
        if (item == null) return;

        // уже куплен
        if (wallet.Data.IsOwned(id))
        {
            // можно сразу выбрать
            wallet.Data.selectedBackgroundId = id;
            RefreshAll();
            return;
        }

        // пробуем потратить
        if (!wallet.TrySpend(item.priceCoins))
        {
            RefreshAll();
            return;
        }

        // покупка успешна
        wallet.Data.AddOwned(id);
        wallet.Data.selectedBackgroundId = id; // можно авто-выбирать после покупки

        RefreshAll();
        UpdateSelectedPreview();
    }

    private void RefreshAll()
    {
        foreach (var view in itemViews)
            view.Refresh();

        RefreshVideoCooldownUI();
        UpdateSelectedPreview();
    }

    // ------------------ VIDEO REWARD (+150) ------------------

    private long GetNextVideoUnix()
    {
        return wallet.Data.nextBackgroundVideoTime;
    }

    private void SetNextVideoUnix(long unix)
    {
        wallet.Data.nextBackgroundVideoTime = unix;
    }

    private void TryWatchVideoForCoins()
    {
        long now = TimeUtil.UnixNow();
        long next = GetNextVideoUnix();
        if (now < next) return;

        if (videoRoutine != null) StopCoroutine(videoRoutine);
        videoRoutine = StartCoroutine(VideoFlow());
    }

    private IEnumerator VideoFlow()
    {
        if (videoOverlay != null) videoOverlay.SetActive(true);

        float dur = Mathf.Max(1f, config.videoDurationSeconds);
        float t = 0f;

        while (t < dur)
        {
            t += Time.deltaTime;
            if (videoProgressText != null)
                videoProgressText.text = $"{Mathf.CeilToInt(dur - t)}";
            yield return null;
        }

        if (videoOverlay != null) videoOverlay.SetActive(false);

        // награда
        wallet.Add(config.videoRewardCoins);

        long now = TimeUtil.UnixNow();
        SetNextVideoUnix(now + Mathf.RoundToInt(config.videoCooldownSeconds));

        RefreshVideoCooldownUI();
        videoRoutine = null;
    }

    private void CancelVideo()
    {
        if (videoRoutine != null)
        {
            StopCoroutine(videoRoutine);
            videoRoutine = null;
        }

        if (videoOverlay != null)
            videoOverlay.SetActive(false);

        // закрыли крестиком — награды нет
    }

    private void StartCooldownTick()
    {
        if (cooldownRoutine != null) StopCoroutine(cooldownRoutine);
        cooldownRoutine = StartCoroutine(CooldownTick());
    }

    private void StopCooldownTick()
    {
        if (cooldownRoutine != null)
        {
            StopCoroutine(cooldownRoutine);
            cooldownRoutine = null;
        }
    }

    private IEnumerator CooldownTick()
    {
        while (root.activeSelf)
        {
            RefreshVideoCooldownUI();
            yield return new WaitForSeconds(1f);
        }
    }

    private void RefreshVideoCooldownUI()
    {
        if (wallet.Data == null) return;

        long now = TimeUtil.UnixNow();
        long left = wallet.Data.nextBackgroundVideoTime - now; // важно: отдельное поле!
        bool ready = left <= 0;

        videoRewardButton.interactable = ready;

        if (ready)
        {
            // показываем "Get/Получить" + перенос строки + 150
            string getWord = LocalizationService.Instance != null
                ? LocalizationService.Instance.Get("bg_get")
                : "Get";

            if (videoLabelText != null)
                videoLabelText.text = getWord + "\n" + config.videoRewardCoins;

            if (videoTimerText != null)
                videoTimerText.text = "";
        }
        else
        {
            // показываем только таймер
            if (videoLabelText != null)
                videoLabelText.text = "";

            if (videoTimerText != null)
                videoTimerText.text = $"{left}s";
        }
    }

    private void EnsureDefaultBackgroundOwned()
    {
        if (wallet.Data == null) return;
        if (config == null || config.items == null || config.items.Count == 0) return;

        string defaultId = config.items[0].id;

        // первый фон всегда куплен
        if (!wallet.Data.IsOwned(defaultId))
            wallet.Data.AddOwned(defaultId);

        // выбран по умолчанию
        if (string.IsNullOrEmpty(wallet.Data.selectedBackgroundId))
            wallet.Data.selectedBackgroundId = defaultId;

    }

    private void UpdateSelectedPreview() // обновление превью
    {
        if (selectedPreviewImage == null) return;
        if (wallet.Data == null) return;

        string selectedId = wallet.Data.selectedBackgroundId;
        if (string.IsNullOrEmpty(selectedId))
            selectedId = config.items[0].id;

        var item = config.items.Find(x => x.id == selectedId);
        if (item == null) return;

        selectedPreviewImage.sprite = item.preview;
        selectedPreviewImage.enabled = (item.preview != null);
    }
}
