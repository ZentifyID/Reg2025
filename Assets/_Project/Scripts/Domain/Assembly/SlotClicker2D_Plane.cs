using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class SlotClicker2D_Plane : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private AssemblyController controller;
    [SerializeField] private LayerMask slotLayer;

    [Tooltip("Z плоскость, на которой физически наход€тс€ слоты (обычно 0).")]
    [SerializeField] private float slotsPlaneZ = 0f;

    private void Awake()
    {
        if (cam == null) cam = Camera.main;
    }

    private void Update()
    {
        if (Mouse.current == null) return;
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        // чтобы UI-кнопки не мешали
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (cam == null || controller == null) return;

        Vector2 screen = Mouse.current.position.ReadValue();
        Ray ray = cam.ScreenPointToRay(screen);

        // ѕлоскость XY на нужном Z
        Plane plane = new Plane(Vector3.forward, new Vector3(0f, 0f, slotsPlaneZ));

        if (!plane.Raycast(ray, out float enter))
            return;

        Vector3 hit3 = ray.GetPoint(enter);
        Vector2 hit2 = new Vector2(hit3.x, hit3.y);

        // ѕопадание в Collider2D в этой точке
        Collider2D col = Physics2D.OverlapPoint(hit2, slotLayer);
        if (col == null) return;

        var slot = col.GetComponent<AttachmentSlot>();
        if (slot == null) return;

        controller.TryPlaceOnSlot(slot);
    }
}
