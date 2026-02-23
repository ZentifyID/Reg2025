using UnityEngine;

public class AutoFinishByMaxX : MonoBehaviour
{
    [SerializeField] private Transform levelRoot;                // твой LevelRoot
    [SerializeField] private string finishPrefabPath = "Prefabs/Finish"; // Resources path
    [SerializeField] private Vector3 finishOffset = new Vector3(2f, 1f, 0f);
    [SerializeField] private float fixedZ = 0f;

    public void PlaceFinish()
    {
        if (levelRoot == null)
        {
            Debug.LogError("[AutoFinishByMaxX] levelRoot is NULL");
            return;
        }

        // 1) найти rideable с максимальным X
        Transform best = null;
        float bestX = float.NegativeInfinity;

        var rideables = levelRoot.GetComponentsInChildren<RideableMarker>(true);
        foreach (var r in rideables)
        {
            if (r == null) continue;

            float x = r.transform.position.x;
            if (x > bestX)
            {
                bestX = x;
                best = r.transform;
            }
        }

        if (best == null)
        {
            Debug.LogWarning("[AutoFinishByMaxX] No RideableMarker found. Finish not placed.");
            return;
        }

        // 2) удалить старый финиш (ищем по PlacedObjectTag.prefabPath)
        var existing = levelRoot.GetComponentsInChildren<PlacedObjectTag>(true);
        foreach (var t in existing)
        {
            if (t != null && t.prefabPath == finishPrefabPath)
                Destroy(t.gameObject);
        }

        // 3) заспавнить финиш
        var finishPrefab = Resources.Load<GameObject>(finishPrefabPath);
        if (finishPrefab == null)
        {
            Debug.LogError("[AutoFinishByMaxX] Finish prefab not found in Resources: " + finishPrefabPath);
            return;
        }

        Vector3 pos = best.position + finishOffset;
        pos.z = fixedZ;

        var finishObj = Instantiate(finishPrefab, pos, Quaternion.identity, levelRoot);

        // пометить, чтобы сохранялось в JSON
        var finishTag = finishObj.GetComponent<PlacedObjectTag>();
        if (finishTag == null) finishTag = finishObj.AddComponent<PlacedObjectTag>();
        finishTag.prefabPath = finishPrefabPath;

        if (finishObj.GetComponent<PlaceableRoot>() == null)
            finishObj.AddComponent<PlaceableRoot>();

        Debug.Log($"[AutoFinishByMaxX] Finish placed near '{best.name}' at X={bestX}");
    }
}