using System;
using System.Collections.Generic;
using UnityEngine;

public class SprayManager : MonoBehaviour {
    public static SprayManager instance;

    [SerializeField] private List<GameObject> caps;
    [SerializeField] private GameObject crosshairCanvas;
    [SerializeField] private float distance = 2f;
    [SerializeField] private float loweredYAmount = 0.05f;
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private float positionLerpSpeed = 5f;
    [SerializeField] private float tiltAmount = 45;

    private Vector3 screenPosition;
    [HideInInspector] public bool raycastEnabled = true;
    [HideInInspector] public Vector3 targetWorldPosition;

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

        foreach (var spray in caps) {
            spray.SetActive(false);
        }

        if (caps.Count > 0) {
            currentSpray = caps[0];
            currentSpray.SetActive(true);
        }
        else {
            Debug.LogWarning("No spray caps assigned in the list!");
        }
        //PaintManager.instance.currentBrushMode = PaintManager.BrushMode.Circle;
    }

    void Start() {
        if (currentSpray != null) {
            originalRotation = currentSpray.transform.localRotation;
            originalLocalPosition = currentSpray.transform.localPosition;
            targetLocalPosition = originalLocalPosition;
        }
        PaintManager.instance.currentBrushMode = PaintManager.BrushMode.Circle;

    }
    //private void OnEnable() {
    //    if (PaintManager.instance != null) {
    //        PaintManager.instance.currentBrushMode = PaintManager.BrushMode.Circle;
    //    }
    //}

    void Update() {
        HandleSprayChange();
    }

    void FixedUpdate() {
        HandleSprayMovement();
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
            currentSpray.transform.localRotation = Quaternion.Lerp(
                currentSpray.transform.localRotation,
                targetRotation,
                Time.deltaTime * rotationSpeed
            );

            currentSpray.transform.localPosition = Vector3.Lerp(
                currentSpray.transform.localPosition,
                targetLocalPosition,
                Time.deltaTime * positionLerpSpeed
            );

            if (Quaternion.Angle(currentSpray.transform.localRotation, targetRotation) < 0.1f &&
                Vector3.Distance(currentSpray.transform.localPosition, targetLocalPosition) < 0.001f) {
                isRotating = false;
            }
        }
    }

    void HandleSprayChange() {
        if (Input.GetKeyDown(KeyCode.Q)) {
            if (caps.Count == 0) {
                Debug.LogWarning("No spray caps available!");
                return;
            }

            if (currentSpray != null) {
                currentSpray.SetActive(false);
            }

            currentIndex = (currentIndex + 1) % caps.Count;
            currentSpray = caps[currentIndex];
            currentSpray.SetActive(true);

            originalRotation = currentSpray.transform.localRotation;
            originalLocalPosition = currentSpray.transform.localPosition;
            targetLocalPosition = originalLocalPosition;

            Debug.Log($"Selected spray: {currentSpray.name} (Index: {currentIndex})");
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
                crosshairCanvas.transform.position = hitForward.point + hitForward.normal * 0.01f;
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

        float screenWidth = Screen.width;
        float normalizedX = Mathf.Clamp01(screenPosition.x / screenWidth);
        float targetRotationY = Mathf.Lerp(-40f, 40f, normalizedX);

        float screenHeight = Screen.height;
        float normalizedY = Mathf.Clamp01(screenPosition.y / screenHeight);
        float targetRotationX = Mathf.Lerp(25f, -25f, normalizedY);

        Quaternion targetRotation = Quaternion.Euler(targetRotationX, targetRotationY, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * smoothSpeed);
    }
}