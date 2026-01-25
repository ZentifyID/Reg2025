using UnityEngine;

public class SlotClick : MonoBehaviour
{
    [SerializeField] private AssemblyController controller;
    [SerializeField] private AttachmentSlot slot;

    private void Awake()
    {
        if (slot == null) slot = GetComponent<AttachmentSlot>();
    }

    private void OnMouseDown()
    {
        if (controller == null || slot == null) return;
        Debug.Log("SlotClick: OnMouseDown " + name);
        controller.TryPlaceOnSlot(slot);
    }
}
