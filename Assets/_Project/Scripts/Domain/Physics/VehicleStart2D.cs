using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class VehicleStart2D : MonoBehaviour
{
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.simulated = false;
    }

    public void StartRun()
    {
        rb.simulated = true;
        Debug.Log("[VehicleStart2D] simulated=true");
    }
}
