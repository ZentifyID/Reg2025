using UnityEngine;

public class WheelRotation : MonoBehaviour
{
    private AssemblyController assembly;

    [SerializeField] private float speed = 500f;
    [SerializeField] private Vector3 axis = Vector3.forward;

    private void Awake()
    {
        assembly = FindFirstObjectByType<AssemblyController>();
    }

    private void Update()
    {
        if (assembly != null && assembly.Phase == GamePhase.Run)
            transform.Rotate(axis * speed * Time.deltaTime);
    }
}
