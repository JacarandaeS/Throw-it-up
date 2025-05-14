using Unity.VisualScripting;
using UnityEngine;

public class MarkerHandler : MonoBehaviour {
    private Vector3 screenPosition;
    private Vector3 targetWorldPosition;
    [SerializeField] private float distance = 2f;
    [SerializeField] private float smoothSpeed = 20f;
    public void Update() {
        HandleMarkerMovement();
    }
    void HandleMarkerMovement() {
        // Get mouse world position
        screenPosition = Input.mousePosition;
        screenPosition.z = Mathf.Abs(Camera.main.transform.position.z + distance);
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

        // Smoothly move towards the mouse
        targetWorldPosition = Vector3.Lerp(targetWorldPosition, mouseWorldPosition, Time.deltaTime * smoothSpeed);
        transform.position = targetWorldPosition;
    }
}
