using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlacementManager : MonoBehaviour
{
    public static PlacementManager Instance { get; private set; }

    [Header("Scene refs")]
    public Camera worldCamera;
    public Transform spawnParent; // LevelRoot

    [Header("Layers")]
    public LayerMask placeableMask;

    [Header("Placement")]
    public float fixedZ = 0f;

    private GameObject _selectedPrefab;

    private GameObject _selectedObject;
    private bool _dragging;
    private Vector3 _dragOffset;
    private string _selectedPrefabPath;
    const string FINISH_PATH = "Prefabs/Finish Tape";

    private readonly List<GameObject> _spawnHistory = new();

    private int PlaceableLayer => LayerMask.NameToLayer("Placeable");

    private void Awake()
    {
        Instance = this;
        if (worldCamera == null) worldCamera = Camera.main;
    }

    public void SelectPrefab(GameObject prefab, string prefabPath)
    {
        _selectedPrefab = prefab;
        _selectedPrefabPath = prefabPath;
        _selectedObject = null;
        _dragging = false;
    }

    private void Update()
    {
        if (Mouse.current == null) return;

        // Ctrl+Z
        var kb = Keyboard.current;
        if (kb != null && kb.leftCtrlKey.isPressed && kb.zKey.wasPressedThisFrame)
            UndoLastSpawn();

        // R
        if (kb != null && kb.rKey.wasPressedThisFrame)
            RotateLastSpawned90();

        // ЛКМ
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            if (TrySelect2DUnderCursor())
            {
                _dragging = true;
            }
            else
            {
                TrySpawnAtCursorOnPlane();
            }
        }

        if (Mouse.current.leftButton.isPressed && _dragging && _selectedObject != null)
            DragSelectedOnPlane();

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            _dragging = false;
            _selectedObject = null;
        }
    }

    private bool TrySelect2DUnderCursor()
    {
        if (!TryGetPointOnFixedZPlane(out Vector3 p)) return false;

        Collider2D col = Physics2D.OverlapPoint(new Vector2(p.x, p.y), placeableMask);
        if (col == null) return false;
        var root = col.GetComponentInParent<PlaceableRoot>();
        if (root == null) return false;

        _selectedObject = root.gameObject;

        _dragOffset = _selectedObject.transform.position - p;
        return true;
    }

    private void TrySpawnAtCursorOnPlane()
    {
        Debug.Log($"Spawn try: prefab={_selectedPrefab}, path='{_selectedPrefabPath}'");
        if (_selectedPrefab == null) { Debug.Log("Spawn blocked: _selectedPrefab is null"); return; }
        if (string.IsNullOrWhiteSpace(_selectedPrefabPath)) { Debug.Log("Spawn blocked: _selectedPrefabPath is empty"); return; }
        if (!TryGetPointOnFixedZPlane(out Vector3 p)) return;

        var obj = Instantiate(_selectedPrefab, p, Quaternion.identity, spawnParent);

        // слой на весь объект (корень + дети)
        if (PlaceableLayer != -1)
            SetLayerRecursively(obj, PlaceableLayer);

        // тег типа для сохранения
        var tag = obj.GetComponent<PlacedObjectTag>();
        if (tag == null) tag = obj.AddComponent<PlacedObjectTag>();
        tag.prefabPath = _selectedPrefabPath;

        // маркер корня для корректного выбора/drag
        if (obj.GetComponent<PlaceableRoot>() == null)
            obj.AddComponent<PlaceableRoot>();

        _spawnHistory.Add(obj);
    }

    private void DragSelectedOnPlane()
    {
        if (!TryGetPointOnFixedZPlane(out Vector3 p)) return;

        Vector3 target = p + _dragOffset;
        target.z = fixedZ;
        _selectedObject.transform.position = target;
    }

    private Ray GetMouseRay()
    {
        Vector2 screenPos = Mouse.current.position.ReadValue();
        return worldCamera.ScreenPointToRay(screenPos);
    }

    private bool TryGetPointOnFixedZPlane(out Vector3 point)
    {
        Ray ray = GetMouseRay();
        Plane plane = new Plane(Vector3.forward, new Vector3(0f, 0f, fixedZ));

        if (plane.Raycast(ray, out float enter))
        {
            point = ray.GetPoint(enter);
            point.z = fixedZ;
            return true;
        }

        point = default;
        return false;
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
            SetLayerRecursively(child.gameObject, newLayer);
    }

    private void UndoLastSpawn()
    {
        for (int i = _spawnHistory.Count - 1; i >= 0; i--)
        {
            if (_spawnHistory[i] == null) _spawnHistory.RemoveAt(i);
            else break;
        }

        if (_spawnHistory.Count == 0) return;

        var last = _spawnHistory[^1];
        _spawnHistory.RemoveAt(_spawnHistory.Count - 1);

        if (_selectedObject == last)
        {
            _selectedObject = null;
            _dragging = false;
        }

        Destroy(last);
    }

    public void RotateLastSpawned90()
    {
        var go = GetLastSpawnedAlive();
        if (go == null) return;

        var e = go.transform.eulerAngles;
        e.z = Mathf.Repeat(e.z + 90f, 360f);
        go.transform.eulerAngles = e;
    }

    // кнопка Undo (то же, что Ctrl+Z)
    public void UndoButton()
    {
        UndoLastSpawn();
    }

    // получить последний живой объект из истории
    private GameObject GetLastSpawnedAlive()
    {
        for (int i = _spawnHistory.Count - 1; i >= 0; i--)
        {
            var go = _spawnHistory[i];
            if (go == null)
            {
                _spawnHistory.RemoveAt(i);
                continue;
            }
            return go;
        }
        return null;
    }

    // -------- Save/Load helpers --------

    public List<PlacedObjectData> ExportPlacedObjects()
    {
        bool finishSaved = false;

        var list = new List<PlacedObjectData>();
        if (spawnParent == null) return list;

        for (int i = 0; i < spawnParent.childCount; i++)
        {
            var go = spawnParent.GetChild(i).gameObject;
            var tag = go.GetComponent<PlacedObjectTag>();
            if (tag == null || string.IsNullOrWhiteSpace(tag.prefabPath)) continue;

            if (tag.prefabPath == FINISH_PATH)
            {
                if (finishSaved) continue;
                finishSaved = true;
            }

            list.Add(new PlacedObjectData
            {
                prefabPath = tag.prefabPath,
                position = go.transform.position,
                rotationZ = go.transform.eulerAngles.z
            });
        }
        return list;
    }

    public void ImportPlacedObjects(List<PlacedObjectData> data)
    {
        ClearPlacedObjects();
        if (data == null) return;

        foreach (var d in data)
        {
            if (string.IsNullOrWhiteSpace(d.prefabPath)) continue;

            var prefab = Resources.Load<GameObject>(d.prefabPath);
            if (prefab == null)
            {
                Debug.LogWarning($"[PlacementManager] Prefab not found in Resources: {d.prefabPath}");
                continue;
            }

            var obj = Instantiate(prefab, d.position, Quaternion.Euler(0, 0, d.rotationZ), spawnParent);

            if (PlaceableLayer != -1)
                SetLayerRecursively(obj, PlaceableLayer);

            var tag = obj.GetComponent<PlacedObjectTag>();
            if (tag == null) tag = obj.AddComponent<PlacedObjectTag>();
            tag.prefabPath = d.prefabPath;

            if (obj.GetComponent<PlaceableRoot>() == null)
                obj.AddComponent<PlaceableRoot>();

            _spawnHistory.Add(obj);
        }
    }

    public void ClearPlacedObjects()
    {
        if (spawnParent == null) return;
        for (int i = spawnParent.childCount - 1; i >= 0; i--)
            Destroy(spawnParent.GetChild(i).gameObject);

        _spawnHistory.Clear();
        _selectedObject = null;
        _dragging = false;
    }
}
