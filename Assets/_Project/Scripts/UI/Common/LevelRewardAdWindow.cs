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

    private void Awake()
    {
        if (watchAdButton != null)
            watchAdButton.onClick.AddListener(TryWatchVideo);

        if (skipButton != null)
            skipButton.onClick.AddListener(() => CompleteReward(1));

        if (closeVideoButton != null)
            closeVideoButton.onClick.AddListener(CancelVideo);

        if (videoOverlay != null) videoOverlay.SetActive(false);
        if (root != null) root.SetActive(false);
    }

    public void Open(LevelManager manager)
    {
        levelManager = manager;
        if (root != null) root.SetActive(true);
    }

    private void TryWatchVideo()
    {
        if (videoRoutine != null) return;
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

    private void CancelVideo()
    {
        if (videoRoutine != null)
        {
            StopCoroutine(videoRoutine);
            videoRoutine = null;
        }

        if (videoOverlay != null)
            videoOverlay.SetActive(false);
    }

    private void CompleteReward(int multiplier)
    {
        Close();
        if (levelManager != null)
            levelManager.CompleteLevelAndStartNext(multiplier);
    }

    private void Close()
    {
        CancelVideo();
        if (root != null) root.SetActive(false);
    }
}
