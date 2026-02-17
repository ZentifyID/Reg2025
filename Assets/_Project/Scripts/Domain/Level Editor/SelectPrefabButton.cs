using UnityEngine;
using UnityEngine.UI;

public class SelectPrefabButton : MonoBehaviour
{
    public GameObject prefab;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            Debug.Log($"SelectPrefabButton: clicked, prefab={(prefab ? prefab.name : "NULL")}, PM={(PlacementManager.Instance ? "OK" : "NULL")}");
            if (PlacementManager.Instance != null)
                PlacementManager.Instance.SelectPrefab(prefab);
        });
    }
}
