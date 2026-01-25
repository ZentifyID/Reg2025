using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class TooltipTrigger : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [SerializeField] private string localizationKey;
    [SerializeField] private float delay = 2f;

    private Coroutine showRoutine;
    private PointerEventData cachedEventData;

    public void OnPointerEnter(PointerEventData eventData)
    {
        cachedEventData = eventData;
        showRoutine = StartCoroutine(ShowTooltipDelayed());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (showRoutine != null)
        {
            StopCoroutine(showRoutine);
            showRoutine = null;
        }

        if (TooltipService.Instance != null)
            TooltipService.Instance.Hide();
    }

    private IEnumerator ShowTooltipDelayed()
    {
        yield return new WaitForSeconds(delay);

        if (TooltipService.Instance == null)
        {
            yield break;
        }  

        string text = LocalizationService.Instance != null
            ? LocalizationService.Instance.Get(localizationKey)
            : localizationKey;

        TooltipService.Instance.Show(text);
    }
}
