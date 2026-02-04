using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelRewardAdWindow : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject root;
    [SerializeField] private Button watchAdButton;
    [SerializeField] private Button skipButton;

    [Header("Video Overlay")]
    [SerializeField] private GameObject videoOverlay;
    [SerializeField] private Button closeVideoButton;
    [SerializeField] private TMP_Text videoProgressText;

    [Header("Config")]
    [SerializeField] private float videoDurationSeconds = 3f;
    [SerializeField] private int rewardMultiplier = 3;

    private Coroutine videoRoutine;
    private LevelManager levelManager;

    private Action<int> onCompleted;
    private Action onCanceled;

    private void Awake()
    {
        if (watchAdButton != null)
            watchAdButton.onClick.AddListener(TryWatchVideo);

        if (skipButton != null)
            skipButton.onClick.AddListener(CancelAndClose);

        if (closeVideoButton != null)
            closeVideoButton.onClick.AddListener(CancelAndClose);

        if (videoOverlay != null) videoOverlay.SetActive(false);
        if (root != null) root.SetActive(false);
    }

    public void Open(LevelManager manager, Action<int> onCompleted, Action onCanceled)
    {
        levelManager = manager;
        this.onCompleted = onCompleted;
        this.onCanceled = onCanceled;

        if (videoOverlay != null) videoOverlay.SetActive(false);
        if (videoProgressText != null) videoProgressText.text = "";

        if (root != null) root.SetActive(true);

        TryWatchVideo();
    }

    private void TryWatchVideo()
    {
        if (videoRoutine != null) return;

        if (watchAdButton != null) watchAdButton.interactable = false;
        if (skipButton != null) skipButton.interactable = false;

        videoRoutine = StartCoroutine(VideoFlow());
    }

    private IEnumerator VideoFlow()
    {
        if (videoOverlay != null) videoOverlay.SetActive(true);

        float duration = Mathf.Max(1f, videoDurationSeconds);
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            if (videoProgressText != null)
                videoProgressText.text = $"{Mathf.CeilToInt(duration - timer)}";
            yield return null;
        }

        if (videoOverlay != null) videoOverlay.SetActive(false);
        videoRoutine = null;

        CompleteReward(Mathf.Max(1, rewardMultiplier));
    }

    private void CancelAndClose()
    {
        if (videoRoutine != null)
        {
            StopCoroutine(videoRoutine);
            videoRoutine = null;
        }

        if (videoOverlay != null) videoOverlay.SetActive(false);

        if (watchAdButton != null) watchAdButton.interactable = true;
        if (skipButton != null) skipButton.interactable = true;

        CloseRoot();
        onCanceled?.Invoke();
        ClearCallbacks();
    }

    private void CompleteReward(int multiplier)
    {
        if (watchAdButton != null) watchAdButton.interactable = true;
        if (skipButton != null) skipButton.interactable = true;

        CloseRoot();
        onCompleted?.Invoke(multiplier);
        ClearCallbacks();
    }

    private void CloseRoot()
    {
        if (root != null) root.SetActive(false);
    }

    private void ClearCallbacks()
    {
        onCompleted = null;
        onCanceled = null;
        levelManager = null;
    }
}
