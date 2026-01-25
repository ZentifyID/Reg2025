using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreWindowController : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject root;        // StoreWindow panel (fullscreen)
    [SerializeField] private Button closeButton;

    [Header("Wallet + Config")]
    [SerializeField] private CoinWallet wallet;
    [SerializeField] private StoreConfig config;

    [Header("Payment Overlay")]
    [SerializeField] private GameObject paymentOverlay;

    [Header("Video Overlay")]
    [SerializeField] private GameObject videoOverlay;
    [SerializeField] private Button closeVideoButton;
    [SerializeField] private TMP_Text videoProgressTimer;

    [Header("Free coins")]
    [SerializeField] private Button freeCoinsButton;
    [SerializeField] private TMP_Text freeCooldownTimer;

    [Header("Video coins")]
    [SerializeField] private Button videoCoinsButton;
    [SerializeField] private TMP_Text videoCooldownTimer;

    private Coroutine cooldownRoutine;
    private Coroutine videoRoutine;

    private void Awake()
    {
        closeButton.onClick.AddListener(Close);

        freeCoinsButton.onClick.AddListener(TryClaimFreeCoins);
        videoCoinsButton.onClick.AddListener(TryWatchVideo);

        closeVideoButton.onClick.AddListener(CancelVideo);

        if (paymentOverlay != null) paymentOverlay.SetActive(false);
        if (videoOverlay != null) videoOverlay.SetActive(false);
    }

    public void Open()
    {
        root.SetActive(true);
        StartCooldownTick();
        RefreshCooldownUI();
    }

    public void Close()
    {
        CancelVideo();
        StopCooldownTick();
        root.SetActive(false);
    }

    // --- Coin packs (кнопки пакетов будут дергать этот метод) ---
    public void BuyCoinPack(int coinsAmount)
    {
        StartCoroutine(ProcessPaymentAndAdd(coinsAmount));
    }

    private IEnumerator ProcessPaymentAndAdd(int coinsAmount)
    {
        if (paymentOverlay != null)
        {
            paymentOverlay.SetActive(true);
        }

        yield return new WaitForSeconds(config.paymentProcessingSeconds);

        if (paymentOverlay != null)
            paymentOverlay.SetActive(false);

        var wallet = CoinWallet.Instance;
        if (wallet == null)
        {
            Debug.LogError("CoinWallet.Instance is null");
            yield break;
        }

        wallet.Add(coinsAmount);
    }

    // --- Free coins ---
    private void TryClaimFreeCoins()
    {
        long now = TimeUtil.UnixNow();
        if (now < wallet.Data.nextFreeCoinsTime) return;

        wallet.Add(config.freeCoinsAmount);
        wallet.Data.nextFreeCoinsTime = now + Mathf.RoundToInt(config.freeCoinsCooldownSeconds);

        RefreshCooldownUI();
    }

    // --- Video coins ---
    private void TryWatchVideo()
    {
        long now = TimeUtil.UnixNow();
        if (now < wallet.Data.nextVideoCoinsTime) return;

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

            if (videoProgressTimer != null)
                videoProgressTimer.text = $"{Mathf.CeilToInt(dur - t)}";

            yield return null;
        }

        // досмотрел до конца
        if (videoOverlay != null) videoOverlay.SetActive(false);

        long now = TimeUtil.UnixNow();
        wallet.Add(config.videoCoinsAmount);
        wallet.Data.nextVideoCoinsTime = now + Mathf.RoundToInt(config.videoCoinsCooldownSeconds);

        RefreshCooldownUI();
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

        // ВАЖНО: если закрыли крестиком — монеты НЕ начисляем (по ТЗ)
    }

    // --- Cooldowns UI ---
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
            RefreshCooldownUI();
            yield return new WaitForSeconds(1f);
        }
    }

    private void RefreshCooldownUI()
    {
        long now = TimeUtil.UnixNow();

        // FREE
        long freeLeft = wallet.Data.nextFreeCoinsTime - now;
        bool freeReady = freeLeft <= 0;
        freeCoinsButton.interactable = freeReady;
        if (freeCooldownTimer != null)
            freeCooldownTimer.text = freeReady ? "" : $"{freeLeft}";

        // VIDEO
        long videoLeft = wallet.Data.nextVideoCoinsTime - now;
        bool videoReady = videoLeft <= 0;
        videoCoinsButton.interactable = videoReady;
        if (videoCooldownTimer != null)
            videoCooldownTimer.text = videoReady ? "" : $"{videoLeft}";
    }
}
