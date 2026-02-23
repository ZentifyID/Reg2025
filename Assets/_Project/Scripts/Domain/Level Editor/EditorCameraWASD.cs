using UnityEngine;
using UnityEngine.InputSystem;

public class EditorCameraWASD : MonoBehaviour
{
    [SerializeField] private Transform cam;
    [SerializeField] private float speed = 10f;
    [SerializeField] private bool useUnscaledTime = true;

    private void Awake()
    {
        if (cam == null) cam = transform;
    }

    private void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        Vector3 dir = Vector3.zero;

        if (kb.aKey.isPressed) dir.x -= 1f;
        if (kb.dKey.isPressed) dir.x += 1f;
        if (kb.wKey.isPressed) dir.y += 1f;
        if (kb.sKey.isPressed) dir.y -= 1f;

        if (dir.sqrMagnitude < 0.0001f) return;

        dir.Normalize();

        float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        cam.position += dir * speed * dt;
    }
}