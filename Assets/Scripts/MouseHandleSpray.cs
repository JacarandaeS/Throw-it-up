using UnityEngine;

public class MouseHandleSpray : MonoBehaviour {
    [SerializeField] GameObject pintura;

    private bool pinturaActiva = false;
    private Vector3 screenPosition;
    private Vector3 worldPosition;

    public float distance = 2f; // Base distance
    public float distanceDiference = 2f; // Max difference added
    private float currentDistance; // Current animated distance
    private float returnTimer = 0f;
    private bool isReturning = false;

    private void Start() {
        currentDistance = distance;
    }

    void Update() {
        HandleOnOff();
        HandleDistance();

        // Set the screen position
        screenPosition = Input.mousePosition;
        screenPosition.z = Mathf.Abs(Camera.main.transform.position.z + currentDistance);

        // Convert to world space
        worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        transform.position = worldPosition;
    }

    void HandleOnOff() {
        if (Input.GetMouseButton(0) && !pinturaActiva) {
            pintura.SetActive(true);
            pinturaActiva = true;
        }
        else if (!Input.GetMouseButton(0) && pinturaActiva) {
            pintura.SetActive(false);
            pinturaActiva = false;
        }
    }

    void HandleDistance() {
        if (Input.GetKey(KeyCode.LeftControl)) {
            isReturning = false;
            returnTimer = 0f;

            float target = distance + distanceDiference;
            currentDistance = Mathf.MoveTowards(currentDistance, target, Time.deltaTime * 3f);

            if (currentDistance >= target) {
                Debug.Log("Reached max distance");
            }
        }
        else {
            // Begin returning to original distance
            if (currentDistance > distance) {
                isReturning = true;
            }

            if (isReturning) {
                returnTimer += Time.deltaTime;
                float t = Mathf.Clamp01(returnTimer / 1f); // 1 second return
                currentDistance = Mathf.Lerp(currentDistance, distance, t);

                if (Mathf.Abs(currentDistance - distance) < 0.01f) {
                    currentDistance = distance;
                    isReturning = false;
                    returnTimer = 0f;
                }
            }
        }
    }
}
