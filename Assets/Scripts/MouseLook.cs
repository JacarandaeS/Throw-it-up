using UnityEngine;

public class MouseLook : MonoBehaviour {
    [Header("Sensitivity")]
    [SerializeField] private float mouseSensitivity = 100f;

    [Header("Vertical Clamp")]
    [SerializeField] private float minPitch = -90f;
    [SerializeField] private float maxPitch = 90f;

    [Header("Horizontal Clamp")]
    [SerializeField] private float minYaw = -90f;
    [SerializeField] private float maxYaw = 90f;

    private float pitch = 0f;  // Rotation around X axis (up/down)
    private float yaw = 0f;    // Rotation around Y axis (left/right)

    void Start() {
        Vector3 euler = transform.localEulerAngles;

        // Normalize Euler angles to [-180, 180] range for consistency
        yaw = NormalizeAngle(euler.y);
        pitch = NormalizeAngle(euler.x);
    }

    void Update() {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;

        // Clamp values
        yaw = Mathf.Clamp(yaw, minYaw, maxYaw);
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        transform.localEulerAngles = new Vector3(pitch, yaw, 0f);
    }

    // Ensures angles like 350 become -10, so clamping works properly
    private float NormalizeAngle(float angle) {
        angle %= 360f;
        if (angle > 180f) angle -= 360f;
        return angle;
    }
}
