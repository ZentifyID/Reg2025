using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlacementManager : MonoBehaviour
{
    public static PlacementManager Instance { get; private set; }

    [Header("Placement")]
    public Camera worldCamera;
    public LayerMask placementMask;
    public Transform spawnParent;

    private GameObject _selectedPrefab;

    private void Awake()
    {
        Instance = this;
    }

    public void SelectPrefab(GameObject prefab)
    {
        _selectedPrefab = prefab;
    }

    private void Update()
    {
        if (_selectedPrefab == null) return;
        if (Mouse.current == null) return; 

        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        Vector2 screenPos = Mouse.current.position.ReadValue();
        Ray ray = worldCamera.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out var hit, 1000f, placementMask))
        {
            Instantiate(_selectedPrefab, hit.point, Quaternion.identity, spawnParent);
        }
    }
}
