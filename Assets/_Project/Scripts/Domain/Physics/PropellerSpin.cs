using UnityEngine;

public class PropellerSpin : MonoBehaviour
{
    [SerializeField] private float speed = 1000f;
    [SerializeField] private Vector3 axis = Vector3.forward;

    private VehicleMotor2D motor;

    private void Awake()
    {
        motor = GetComponentInParent<VehicleMotor2D>();
        if (motor == null)
            motor = FindFirstObjectByType<VehicleMotor2D>();
    }

    private void Update()
    {
        if (motor != null && motor.IsPropellerHeld)
            transform.Rotate(axis * speed * Time.deltaTime);
    }
}
