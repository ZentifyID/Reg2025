using UnityEngine;

public class VehicleVisualRoot : MonoBehaviour
{
    public void SetVisual(GameObject visualPrefab)
    {
        // удалить старую модель
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);

        if (visualPrefab != null)
            Instantiate(visualPrefab, transform);
    }
}
