using System.Linq;
using UnityEngine;

public class SlotAnchorBinder : MonoBehaviour
{
    [SerializeField] private RoverVisual roverVisual;      // где спавнится модель
    [SerializeField] private Transform vehicleSlotsRoot;     // где лежат реальные AttachmentSlot
    [SerializeField] private string anchorsRootName = "SlotAnchors"; // контейнер внутри модели

    public void Rebind()
    {
        if (roverVisual == null || vehicleSlotsRoot == null) return;

        var model = roverVisual.CurrentModelTransform;
        if (model == null) return;

        var anchorsRoot = model.Find(anchorsRootName);
        if (anchorsRoot == null)
        {
            Debug.LogWarning($"[SlotAnchorBinder] Anchors root '{anchorsRootName}' not found under model '{model.name}'.");
            return;
        }

        var slots = vehicleSlotsRoot.GetComponentsInChildren<AttachmentSlot>(true);
        foreach (var slot in slots)
        {
            if (slot == null) continue;

            var anchor = anchorsRoot.Find(slot.Accepts.ToString());
            if (anchor == null)
            {
                Debug.LogWarning($"[SlotAnchorBinder] Anchor '{slot.Accepts}' not found in '{anchorsRootName}' for model '{model.name}'.");
                continue;
            }

            slot.transform.position = anchor.position;
            slot.transform.rotation = anchor.rotation;

        }
    }
}
