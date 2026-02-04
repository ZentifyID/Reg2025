using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class SlotClicker : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private AssemblyController controller;
    [SerializeField] private LayerMask slotLayer;

    private void Awake()
    {
        if (cam == null) cam = Camera.main;
    }

    private void Update()
    {
        if (Mouse.current == null) return;
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (cam == null || controller == null) return;

        Vector2 screenPos = Mouse.current.position.ReadValue();
        Ray ray = cam.ScreenPointToRay(screenPos);

        if (!Physics.Raycast(ray, out RaycastHit hit, 1000f, slotLayer))
            return;

        var slot = hit.collider.GetComponent<AttachmentSlot>();
        if (slot == null) return;

        controller.TryPlaceOnSlot(slot);
    }
}
