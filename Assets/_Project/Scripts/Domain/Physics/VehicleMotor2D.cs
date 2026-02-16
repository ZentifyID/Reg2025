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

    [Header("Propeller")]
    [SerializeField] private float propellerForce = 20f;

    [Header("Rocket")]
    [SerializeField] private bool hasRocket;

    private bool hasPropeller;
    private bool propellerHeld;
    public bool HasPropeller => hasPropeller;
    public bool IsPropellerHeld => propellerHeld;

    public bool HasRocket => hasRocket;
    private RocketPod2D rocketPod;
    public RocketPod2D RocketPod => rocketPod;

    public void SetRocketPod(RocketPod2D pod) => rocketPod = pod;

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

        if (propellerHeld)
        {
            rb.AddForce(transform.right * propellerForce, ForceMode2D.Force);
        }
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

    public void SetHasPropeller(bool value)
    {
        hasPropeller = value;
        if (!hasPropeller)
            propellerHeld = false;
    }

    public void SetHasRocket(bool value)
    {
        hasRocket = value;
    }

    public void SetPropellerHeld(bool held)
    {
        if (!hasPropeller) { propellerHeld = false; return; }
        propellerHeld = held;
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
