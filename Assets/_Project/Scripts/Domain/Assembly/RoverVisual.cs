using UnityEngine;

public class RoverVisual : MonoBehaviour
{
    [SerializeField] private Transform modelRoot;
    private GameObject current;
    public Transform CurrentModelTransform => current != null ? current.transform : null;
    public GameObject SetModel(GameObject modelPrefab)
    {
        if (modelRoot == null) return null;

        if (current != null)
            Destroy(current);

        if (modelPrefab == null) return null;

        current = Instantiate(modelPrefab, modelRoot);
        current.transform.localPosition = Vector3.zero;
        current.transform.localRotation = Quaternion.identity;
        current.transform.localScale = Vector3.one;

        return current;
    }
}
