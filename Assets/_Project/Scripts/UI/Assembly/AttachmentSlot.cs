using UnityEngine;

public class AttachmentSlot : MonoBehaviour
{
    [field: SerializeField] public ItemType Accepts { get; private set; }

    [SerializeField] private GameObject highlight;

    public bool IsOccupied { get; private set; }

    public void SetHighlighted(bool value)
    {
        if (highlight == null) return;
        highlight.SetActive(value && !IsOccupied);
    }

    public void Place(GameObject prefab)
    {
        if (IsOccupied) return;

        var obj = Instantiate(prefab, transform);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;

        IsOccupied = true;
        SetHighlighted(false);
    }

    public void Clear()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);

        IsOccupied = false;
        SetHighlighted(false);
    }
}
