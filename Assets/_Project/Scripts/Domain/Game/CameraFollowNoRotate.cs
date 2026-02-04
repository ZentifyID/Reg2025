using UnityEngine;

public class CameraFollowNoRotate : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 2, -10);
    [SerializeField] private float smooth = 5f;

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPos, smooth * Time.deltaTime);

        transform.rotation = Quaternion.Euler(14, 20, 0);
    }
}
