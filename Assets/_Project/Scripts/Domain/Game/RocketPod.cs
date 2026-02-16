using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class RocketPod2D : MonoBehaviour
{
    [Header("Flight")]
    [SerializeField] private float speed = 25f;
    [SerializeField] private float maxFlightTime = 2.5f;

    [Header("Cooldown")]
    [SerializeField] private float cooldown = 2f;

    [Header("Direction")]
    [SerializeField] private bool useRightAxis = true;   // true = летит по transform.right, false = по transform.up
    [SerializeField] private bool invertAxis = false;    // если летит "в обратную сторону" — включи

    [Header("What can be destroyed")]
    [SerializeField] private LayerMask obstacleMask;     // поставь слой препятствий (рекомендую)

    private Rigidbody2D rb;
    private Collider2D col;

    private float nextFireTime;
    private bool inFlight;

    // где ракета "паркуется"
    private Transform dockParent;
    private Vector3 dockLocalPos;
    private Quaternion dockLocalRot;

    private float flightEndTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        // пока ракета на платформе — физика выключена
        rb.simulated = false;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    public bool CanFire => !inFlight && Time.time >= nextFireTime;

    /// <summary>Выстрел: ракета отстыковывается и летит по своему углу.</summary>
    public void Fire()
    {
        if (!CanFire) return;

        // запоминаем "док" один раз (если вдруг не сохранили ранее)
        if (dockParent == null)
        {
            dockParent = transform.parent;
            dockLocalPos = transform.localPosition;
            dockLocalRot = transform.localRotation;
        }

        inFlight = true;
        nextFireTime = Time.time + cooldown;
        flightEndTime = Time.time + maxFlightTime;

        // отстыковали от платформы (чтобы не улетала вместе с машиной)
        transform.SetParent(null, true);

        // включили физику
        rb.simulated = true;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        Vector2 dir = useRightAxis ? (Vector2)transform.right : (Vector2)transform.up;
        if (invertAxis) dir = -dir;

        rb.linearVelocity = dir.normalized * speed;

        // можно сделать collider trigger, чтобы не цепляться за машину:
        // col.isTrigger = true;
    }

    private void Update()
    {
        // если улетела слишком долго — возвращаем
        if (inFlight && Time.time >= flightEndTime)
            ReturnToDock();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!inFlight) return;

        // проверка слоя препятствий
        if (((1 << other.gameObject.layer) & obstacleMask) == 0)
            return;

        // простой вариант: уничтожить препятствие
        var destr = other.GetComponentInParent<DestructibleObstacle>();
        if (destr != null) destr.HitByRocket();
        else Destroy(other.gameObject);

        ReturnToDock();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!inFlight) return;

        if (((1 << collision.gameObject.layer) & obstacleMask) == 0)
            return;

        var destr = collision.collider.GetComponentInParent<DestructibleObstacle>();
        if (destr != null) destr.HitByRocket();
        else Destroy(collision.gameObject);

        ReturnToDock();
    }

    private void ReturnToDock()
    {
        inFlight = false;

        // выключаем физику и возвращаем на платформу
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.simulated = false;

        if (dockParent != null)
        {
            transform.SetParent(dockParent, true);
            transform.localPosition = dockLocalPos;
            transform.localRotation = dockLocalRot;
        }
    }
}