using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class WorldClickPlacer : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private AssemblyController controller;
    [SerializeField] private LayerMask slotLayer; // слой слотов

    private void Awake()
    {
        if (cam == null) cam = Camera.main;
    }

    private void Update()
    {
        if (Mouse.current == null) return;
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        // чтобы клики по UI (кнопкам) не ставили предметы
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (cam == null || controller == null) return;

        Vector2 screenPos = Mouse.current.position.ReadValue();
        Ray ray = cam.ScreenPointToRay(screenPos);

        if (!Physics.Raycast(ray, out RaycastHit hit, 1000f, slotLayer))
            return;

        var slot = hit.collider.GetComponent<AttachmentSlot>();
        if (slot == null) return;

        Debug.Log("Clicked slot: " + hit.collider.name);
        controller.TryPlaceOnSlot(slot);
    }
}
