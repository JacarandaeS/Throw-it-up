using UnityEngine;

public class MouseFollow : MonoBehaviour {
    public float moveSpeed = 5f; // Adjust for movement speed
    public Camera camera;

    void Update() {
        // Get the mouse position in screen space
        Vector3 mousePosition = Input.mousePosition;

        // Convert screen position to world space (using camera and z-depth)
        mousePosition.z = -.5f; // Keep the object's Z-position
        mousePosition = camera.ScreenToWorldPoint(mousePosition);

        // Move the object towards the mouse position
        transform.position = Vector3.MoveTowards(transform.position, mousePosition, moveSpeed * Time.deltaTime);
    }
}