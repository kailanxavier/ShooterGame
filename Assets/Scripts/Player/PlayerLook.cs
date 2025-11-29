using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [Header("POV settings: ")]
    [SerializeField, Range(0.0f, 500.0f)] private float sensX;
    [SerializeField, Range(0.0f, 500.0f)] private float sensY;
    [SerializeField, Range(0.0f, 90.0f)] private float maxLookAngle = 75.0f;

    [Header("References: ")]
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform camRef;

    float pitch;
    float yaw;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        Look();
    }

    private void Look()
    {
        Vector2 lookInput = InputManager.Instance.Look;
        float mouseX = lookInput.x * sensX * Time.deltaTime;
        float mouseY = lookInput.y * sensY * Time.deltaTime;

        pitch += mouseX;
        yaw -= mouseY;
        yaw = Mathf.Clamp(yaw, -maxLookAngle, maxLookAngle);

        camRef.transform.rotation = Quaternion.Euler(yaw, pitch, 0);
        orientation.rotation = Quaternion.Euler(0, pitch, 0);
    }
}
