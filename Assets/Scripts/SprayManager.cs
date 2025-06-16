using System;
using System.Collections.Generic;
using UnityEngine;

public class SprayManager : MonoBehaviour {
    public static SprayManager instance;

    [SerializeField] private GameObject skinniCap;
    // [SerializeField] private GameObject mediumCap;
    //   [SerializeField] private GameObject biseladoCap;
    [SerializeField] private GameObject mixerCap;
    [SerializeField] private GameObject crosshairCanvas;
    [SerializeField] private float distance = 2f;
    [SerializeField] private float loweredYAmount = 0.05f;
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private float positionLerpSpeed = 5f;
    [SerializeField] private float tiltAmount = 45;

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

    private LayerMask pintableLayer = 8;
    void Awake() {
        if (instance == null) {
            instance = this;
        }

        aerosoles.Add(skinniCap);
       // aerosoles.Add(mediumCap);
       // aerosoles.Add(biseladoCap);
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
   

    void FixedUpdate() {
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
            targetRotation = Quaternion.Euler(-tiltAmount, originalRotation.eulerAngles.y, originalRotation.eulerAngles.z);
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
           // Ray cameraRaycast = new Ray();
        }
    }

    void HandleRaycast() {
        Ray rayForward = new Ray(transform.position, transform.forward);
        RaycastHit hitForward;
        bool hitSomething = Physics.Raycast(rayForward, out hitForward, 10f);

        if (hitSomething) {
            Debug.DrawRay(rayForward.origin, hitForward.point - rayForward.origin, Color.red);

            Paintable p = hitForward.collider.GetComponent<Paintable>();
            if (p != null) {
                // Position the crosshair slightly in front of the surface (0.01 units)
                crosshairCanvas.transform.position = hitForward.point + hitForward.normal * .01f;
                // Make the crosshair face the camera while aligning with the surface normal
                //crosshairCanvas.transform.rotation = Quaternion.LookRotation(-hitForward.normal, Camera.main.transform.up);

                crosshairCanvas.SetActive(true);
            }
            else {
                crosshairCanvas.SetActive(false);
            }
        }
        else {
            crosshairCanvas.SetActive(false);
        }
    }

    void HandleSprayMovement() {
        if (currentSpray == null || !currentSpray.activeSelf)
            return;

        float smoothSpeed = GameManager.instance != null ? GameManager.instance.GetSmoothSpeed() : 20f;

        screenPosition = Input.mousePosition;

        // Raycast to get depth from a paintable surface
        Ray cameraRay = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(cameraRay, out RaycastHit camHit, 100f)) {
            Paintable p = camHit.collider.GetComponent<Paintable>();
            if (p != null) {
                Vector3 worldPoint = camHit.point;
                screenPosition.z = Camera.main.WorldToScreenPoint(worldPoint).z + distance;
            }
        }

        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        targetWorldPosition = Vector3.Lerp(targetWorldPosition, mouseWorldPosition, Time.deltaTime * smoothSpeed);
        transform.position = targetWorldPosition;

        // **Calculate Y rotation (left/right) based on screen X position**
        float screenWidth = Screen.width;
        float normalizedX = Mathf.Clamp01(screenPosition.x / screenWidth);
        float targetRotationY = Mathf.Lerp(-45f, 45f, normalizedX);

        // **Calculate X rotation (up/down) based on screen Y position**
        float screenHeight = Screen.height;
        float normalizedY = Mathf.Clamp01(screenPosition.y / screenHeight);
        float targetRotationX = Mathf.Lerp(25f, -25f, normalizedY);

        // Apply rotations smoothly, forcing Z = 0
        Quaternion targetRotation = Quaternion.Euler(targetRotationX, targetRotationY, 0); // Explicitly set Z to 0
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * smoothSpeed);
    }
}