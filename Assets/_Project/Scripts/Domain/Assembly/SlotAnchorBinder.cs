using UnityEngine;

public class SlotAnchorBinder : MonoBehaviour
{
    [SerializeField] private RoverVisual roverVisual;
    [SerializeField] private Transform vehicleSlotsRoot;
    [SerializeField] private string anchorsRootName = "SlotAnchors";

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

            Transform anchor = null;

            if (!string.IsNullOrWhiteSpace(slot.AnchorId))
                anchor = anchorsRoot.Find(slot.AnchorId);

            if (anchor == null)
                anchor = anchorsRoot.Find(slot.Accepts.ToString());

            if (anchor == null)
            {
                Debug.LogWarning($"[SlotAnchorBinder] Anchor not found for slot '{slot.name}'. Tried '{slot.AnchorId}' and '{slot.Accepts}'.");
                continue;
            }

            slot.transform.SetPositionAndRotation(anchor.position, anchor.rotation);
        }
    }
}
