using UnityEngine;

public class RocketLauncher : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private VehicleMotor2D vehicle;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject rocketPrefab;

    [Header("Tuning")]
    [SerializeField] private float cooldown = 2f;

    private float nextFireTime;

    private void Awake()
    {
        if (vehicle == null) vehicle = GetComponentInParent<VehicleMotor2D>();
    }

    public bool CanFire =>
        vehicle != null && vehicle.HasRocket && Time.time >= nextFireTime;

    public float CooldownRemaining =>
        Mathf.Max(0f, nextFireTime - Time.time);

    public void Fire()
    {
        if (!CanFire) return;
        if (rocketPrefab == null || firePoint == null) return;

        nextFireTime = Time.time + cooldown;

        var go = Instantiate(rocketPrefab, firePoint.position, firePoint.rotation);
    }
}
