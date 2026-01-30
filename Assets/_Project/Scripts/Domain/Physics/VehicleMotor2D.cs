using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class VehicleMotor2D : MonoBehaviour
{
    [Header("Forward move")]
    [SerializeField] private float forwardSpeed = 6f;
    [SerializeField] private bool autoMove;

    [Header("Wings")]
    [SerializeField] private float wingsImpulse = 6f;
    [SerializeField] private float wingsCooldown = 1f;

    private Rigidbody2D rb;
    private float nextWingsTime;

    public bool HasWings { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 1f;
        Debug.Log("[VehicleMotor2D] Awake on " + name);
    }

    private void FixedUpdate()
    {
        if (!autoMove) return;
        rb.linearVelocity = new Vector2(forwardSpeed, rb.linearVelocity.y);
    }

    public void StartMoving()
    {
        autoMove = true;
        Debug.Log("[VehicleMotor2D] StartMoving");
    }

    public void SetHasWings(bool value)
    {
        HasWings = value;
        Debug.Log("[VehicleMotor2D] HasWings=" + HasWings);
    }

    public bool TryUseWings()
    {
        Debug.Log($"[VehicleMotor2D] TryUseWings simulated={rb.simulated} HasWings={HasWings}");

        if (!HasWings) return false;
        if (!rb.simulated) return false;          // ВАЖНО: пока simulated=false, force не сработает
        if (Time.time < nextWingsTime) return false;

        nextWingsTime = Time.time + wingsCooldown;
        rb.AddForce(Vector2.up * wingsImpulse, ForceMode2D.Impulse);

        Debug.Log("[VehicleMotor2D] Wings impulse!");
        return true;
    }

    public void StopMoving()
    {
        autoMove = false;
    }
}
