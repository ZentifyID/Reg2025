using UnityEngine;

public class DestructibleObstacle : MonoBehaviour
{
    private Vector3 startPos;
    private Quaternion startRot;
    private bool saved;

    private void Awake()
    {
        SaveIfNeeded();
    }

    private void OnEnable()
    {
        SaveIfNeeded();
    }

    private void SaveIfNeeded()
    {
        if (saved) return;
        startPos = transform.position;
        startRot = transform.rotation;
        saved = true;
    }

    public void HitByRocket()
    {
        // вместо Destroy
        gameObject.SetActive(false);
    }

    public void ResetObstacle()
    {
        // вернуть на место и включить
        transform.position = startPos;
        transform.rotation = startRot;
        gameObject.SetActive(true);
    }
}
