using UnityEngine;
using UnityEngine.UI;

public class SelectPrefabButton : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private string prefabPath;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            Debug.Log($"SelectPrefabButton: clicked, prefab={(prefab ? prefab.name : "NULL")}, PM={(PlacementManager.Instance ? "OK" : "NULL")}");

            if (PlacementManager.Instance != null)
                PlacementManager.Instance.SelectPrefab(prefab, prefabPath);
        });
    }
}
