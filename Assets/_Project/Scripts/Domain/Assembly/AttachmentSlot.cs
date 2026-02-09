using UnityEngine;

public class AttachmentSlot : MonoBehaviour
{
    public ItemType Accepts;
    public bool IsOccupied { get; private set; }

    [Header("Visual")]
    [SerializeField] private GameObject highlight; // твой Highlight объект

    private GameObject placedInstance;

    public void Place(GameObject prefab)
    {
        if (IsOccupied) return;
        if (prefab == null) return;

        placedInstance = Instantiate(prefab, transform);
        placedInstance.transform.localPosition = Vector3.zero;
        placedInstance.transform.localRotation = Quaternion.identity;
        placedInstance.transform.localScale = Vector3.one;

        IsOccupied = true;
        SetHighlighted(false);
    }

    public void Clear()
    {
        if (placedInstance != null)
            Destroy(placedInstance);

        placedInstance = null;
        IsOccupied = false;
        SetHighlighted(false);
    }

    public void SetHighlighted(bool value)
    {
        if (highlight != null)
            highlight.SetActive(value);
    }
}
