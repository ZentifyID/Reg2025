using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class SlotClicker2D : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private AssemblyController controller;
    [SerializeField] private LayerMask slotLayer;

    [Header("Debug")]
    [SerializeField] private bool logClicks = true;
    [SerializeField] private float maxDistance = 5000f;

    private void Awake()
    {
        if (cam == null) cam = Camera.main;
        Debug.Log($"[SlotClicker2D] Awake cam={(cam != null ? cam.name : "NULL")} controller={(controller != null ? controller.name : "NULL")} layerMask={slotLayer.value}");
    }

    private void Update()
    {
        if (Mouse.current == null)
        {
            if (logClicks) Debug.Log("[SlotClicker2D] Mouse.current == null");
            return;
        }

        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        if (logClicks) Debug.Log("[SlotClicker2D] Click detected");

        // ВРЕМЕННО: отключи блокировку UI, чтобы исключить “UI съедает клик”
        // if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        // {
        //     if (logClicks) Debug.Log("[SlotClicker2D] Click over UI -> ignored");
        //     return;
        // }

        if (cam == null)
        {
            Debug.LogError("[SlotClicker2D] Camera is null");
            return;
        }

        if (controller == null)
        {
            Debug.LogError("[SlotClicker2D] AssemblyController is null");
            return;
        }

        Vector2 screenPos = Mouse.current.position.ReadValue();
        Ray ray = cam.ScreenPointToRay(screenPos);

        if (logClicks)
        {
            Debug.Log($"[SlotClicker2D] Screen={screenPos} Ray origin={ray.origin} dir={ray.direction}");
        }

        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, maxDistance, slotLayer);

        if (hit.collider == null)
        {
            Debug.Log("[SlotClicker2D] NO HIT. Check: slots have Collider2D, correct Layer, slotLayer mask not empty, Physics2D settings.");
            return;
        }

        Debug.Log($"[SlotClicker2D] HIT collider={hit.collider.name} point={hit.point}");

        var slot = hit.collider.GetComponent<AttachmentSlot>();
        if (slot == null)
        {
            Debug.Log("[SlotClicker2D] Hit object has no AttachmentSlot: " + hit.collider.name);
            return;
        }

        controller.TryPlaceOnSlot(slot);
    }
}
