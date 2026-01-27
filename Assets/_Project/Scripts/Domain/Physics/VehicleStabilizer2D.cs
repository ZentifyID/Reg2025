using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class VehicleStabilizer2D : MonoBehaviour
{
    [Header("Limits")]
    [SerializeField] private float maxAngularSpeed = 200f; // deg/s
    [SerializeField] private float maxTilt = 35f;          // градусов от "нормального" положения

    [Header("Stabilization force")]
    [SerializeField] private float stabilizeTorque = 10f;  // сила выравнивания
    [SerializeField] private bool onlyInAir = true;

    [Header("Ground check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        // 1) ограничиваем угловую скорость
        rb.angularVelocity = Mathf.Clamp(rb.angularVelocity, -maxAngularSpeed, maxAngularSpeed);

        // 2) если нужно — стабилизируем только в воздухе
        if (onlyInAir && IsGrounded())
            return;

        // 3) выравниваем к 0 град (чтобы банан не крутился)
        float angle = NormalizeAngle(rb.rotation);
        float target = Mathf.Clamp(angle, -maxTilt, maxTilt);

        // torque = "как сильно мы хотим вернуть к target"
        float error = target - angle;
        rb.AddTorque(error * stabilizeTorque, ForceMode2D.Force);
    }

    private bool IsGrounded()
    {
        if (groundCheck == null) return false;
        return Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer) != null;
    }

    private float NormalizeAngle(float a)
    {
        a %= 360f;
        if (a > 180f) a -= 360f;
        if (a < -180f) a += 360f;
        return a;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }
}
