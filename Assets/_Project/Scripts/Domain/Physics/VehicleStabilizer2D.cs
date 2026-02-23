using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class VehicleStabilizer2D : MonoBehaviour
{
    [Header("Limits")]
    [SerializeField] private float maxAngularSpeed = 200f; // deg/s
    [SerializeField] private float maxTilt = 35f;          // градусов от 0

    [Header("Stabilization (PD controller)")]
    [SerializeField] private float stabilizeStrength = 20f; // "пружина" к 0
    [SerializeField] private float stabilizeDamping = 6f;   // демпфер (гасит раскачку)
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
        // ќграничение угловой скорости (м€гко)
        rb.angularVelocity = Mathf.Clamp(rb.angularVelocity, -maxAngularSpeed, maxAngularSpeed);

        // ≈сли стабилизаци€ только в воздухе и мы на земле Ч не мешаем физике
        if (onlyInAir && IsGrounded())
            return;

        float angle = NormalizeAngle(rb.rotation);

        // ÷≈Ћ№ всегда 0∞, но если угол вышел за пределы maxTilt Ч возвращаем к границе
        float targetAngle = 0f;
        if (angle > maxTilt) targetAngle = maxTilt;
        else if (angle < -maxTilt) targetAngle = -maxTilt;

        float error = targetAngle - angle;           // deg
        float angVel = rb.angularVelocity;           // deg/s

        // PD: torque = P*error - D*angVel
        float torque = (error * stabilizeStrength) - (angVel * stabilizeDamping);

        rb.AddTorque(torque, ForceMode2D.Force);
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