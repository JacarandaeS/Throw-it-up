using System.Collections.Generic;
using UnityEngine;

public class SprayManager : MonoBehaviour {
    public static SprayManager instance;

    [SerializeField] private GameObject skinniCap;
    [SerializeField] private GameObject mediumCap;
    [SerializeField] private GameObject biseladoCap;
    [SerializeField] private GameObject mixerCap;
    [SerializeField] private GameObject crosshairCanvas;
    [SerializeField] private float distance = 2f;
    [SerializeField] private float loweredYAmount = 0.05f;
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private float positionLerpSpeed = 5f;

    private Vector3 screenPosition;
    [HideInInspector] public bool raycastEnabled = true;
    [HideInInspector] public Vector3 targetWorldPosition;

    public List<GameObject> aerosoles = new List<GameObject>();
    [HideInInspector] public GameObject currentSpray;

    private int currentIndex = 0;
    private Quaternion targetRotation;
    private Quaternion originalRotation;
    private bool isRotating = false;

    private Vector3 originalLocalPosition;
    private Vector3 targetLocalPosition;

    void Awake() {
        if (instance == null) {
            instance = this;
        }

        aerosoles.Add(skinniCap);
        aerosoles.Add(mediumCap);
        aerosoles.Add(biseladoCap);
        aerosoles.Add(mixerCap);

        foreach (var spray in aerosoles) {
            spray.SetActive(false);
        }

        currentSpray = aerosoles[0];
        currentSpray.SetActive(true);
    }

    void Start() {
        if (currentSpray != null) {
            originalRotation = currentSpray.transform.localRotation;
            originalLocalPosition = currentSpray.transform.localPosition;
            targetLocalPosition = originalLocalPosition;
        }
    }

    void Update() {
        HandleSprayMovement();
        handleSprayChange();
        handleRotation();

        if (raycastEnabled) {
            HandleRaycast();
        }
    }

    void handleRotation() {
        if (currentSpray == null) return;

        if (Input.GetMouseButton(1)) {
            targetRotation = Quaternion.Euler(-35f, originalRotation.eulerAngles.y, originalRotation.eulerAngles.z);
            targetLocalPosition = originalLocalPosition - new Vector3(0, loweredYAmount, 0);
            isRotating = true;
        }
        else {
            targetRotation = originalRotation;
            targetLocalPosition = originalLocalPosition;
            isRotating = true;
        }

        if (isRotating) {
            // Smooth rotation
            currentSpray.transform.localRotation = Quaternion.Lerp(
                currentSpray.transform.localRotation,
                targetRotation,
                Time.deltaTime * rotationSpeed
            );

            // Smooth vertical movement
            currentSpray.transform.localPosition = Vector3.Lerp(
                currentSpray.transform.localPosition,
                targetLocalPosition,
                Time.deltaTime * positionLerpSpeed
            );

            // Stop when close enough
            if (Quaternion.Angle(currentSpray.transform.localRotation, targetRotation) < 0.1f &&
                Vector3.Distance(currentSpray.transform.localPosition, targetLocalPosition) < 0.001f) {
                isRotating = false;
            }
        }
    }

    void handleSprayChange() {
        if (Input.GetKeyDown(KeyCode.Q)) {
            currentSpray.SetActive(false);
            currentIndex = (currentIndex + 1) % aerosoles.Count;
            currentSpray = aerosoles[currentIndex];
            currentSpray.SetActive(true);

            originalRotation = currentSpray.transform.localRotation;
            originalLocalPosition = currentSpray.transform.localPosition;
            targetLocalPosition = originalLocalPosition;

            Debug.Log("Selected spray: " + currentSpray.name);
        }
    }

    void HandleRaycast() {
        Ray rayForward = new Ray(transform.position, transform.forward);
        RaycastHit hitForward;

        if (Physics.Raycast(rayForward, out hitForward, 10f)) {
            Debug.DrawRay(rayForward.origin, hitForward.point - rayForward.origin, Color.red);

            Paintable p = hitForward.collider.GetComponent<Paintable>();
            if (p != null) {
                crosshairCanvas.transform.position = hitForward.point - new Vector3(0, 0, 0.001f);
                crosshairCanvas.transform.rotation = Quaternion.LookRotation(hitForward.normal);
                crosshairCanvas.SetActive(true);
            }
            else {
                crosshairCanvas.SetActive(false);
                screenPosition.z = Camera.main.WorldToScreenPoint(crosshairCanvas.transform.position - new Vector3(0, 0, distance)).z;
            }
        }
        else {
            crosshairCanvas.SetActive(false);
        }

        //Ray rayBackward = new Ray(transform.position, -transform.forward);
        //RaycastHit hitBackward;

        //if (Physics.Raycast(rayBackward, out hitBackward, 10f)) {
        //    Debug.DrawRay(rayBackward.origin, hitBackward.point - rayBackward.origin, Color.blue);

        //    Paintable pBack = hitBackward.collider.GetComponent<Paintable>();
        //    if (pBack != null) {
        //        Debug.Log("Hit behind: " + hitBackward.collider.name);

        //        Vector3 offsetHitPoint = hitBackward.point + hitBackward.normal * 0.001f;
        //        crosshairCanvas.transform.position = offsetHitPoint;
        //        crosshairCanvas.transform.rotation = Quaternion.LookRotation(hitBackward.normal);
        //        crosshairCanvas.SetActive(true);

        //        screenPosition.z = Camera.main.WorldToScreenPoint(offsetHitPoint + new Vector3(0, 0, (distance + 12))).z;
        //    }
        //}
    }

    void HandleSprayMovement() {
        if (currentSpray == null || !currentSpray.activeSelf)
            return;

        float smoothSpeed = GameManager.instance != null ? GameManager.instance.GetSmoothSpeed() : 20f;

        screenPosition = Input.mousePosition;
        screenPosition.z = Camera.main.WorldToScreenPoint(crosshairCanvas.transform.position - new Vector3(0, 0, distance)).z;
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

        targetWorldPosition = Vector3.Lerp(targetWorldPosition, mouseWorldPosition, Time.deltaTime * smoothSpeed);
        transform.position = targetWorldPosition;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 10f);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position - transform.forward * 10f);
    }
}
