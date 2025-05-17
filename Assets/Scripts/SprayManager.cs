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

    private Vector3 screenPosition;
    // private Vector3 targetWorldPosition;
    [HideInInspector] public bool raycastEnabled = true; // add this at the top of the class

    [HideInInspector] public Vector3 targetWorldPosition;



    public List<GameObject> aerosoles = new List<GameObject>();
    [HideInInspector] public GameObject currentSpray;

    private int currentIndex = 0;

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

    void Update() {
        HandleSprayMovement();
        handleSprayChange();

        if (raycastEnabled) {
            HandleRaycast();
        }
    }

    void handleSprayChange() {
        if (Input.GetKeyDown(KeyCode.Q)) {
            currentSpray.SetActive(false);
            currentIndex = (currentIndex + 1) % aerosoles.Count;
            currentSpray = aerosoles[currentIndex];
            currentSpray.SetActive(true);

            Debug.Log("Selected spray: " + currentSpray.name);
        }
    }

    void HandleRaycast() {
        Ray ray = new Ray(transform.position, transform.forward);
        //Ray rayBehind = new Ray(transform.position, transform.back);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10f)) {
            Debug.DrawRay(ray.origin, hit.point - ray.origin, Color.red);
            // transform.position = hit.point;

            Paintable p = hit.collider.GetComponent<Paintable>();
            if (p != null) {
                crosshairCanvas.transform.position = hit.point - new Vector3(0, 0, 0.001f);
                crosshairCanvas.transform.rotation = Quaternion.LookRotation(hit.normal);
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
    }

    void HandleSprayMovement() {
        if (currentSpray == null || !currentSpray.activeSelf)
            return;

        float smoothSpeed = GameManager.instance != null ? GameManager.instance.GetSmoothSpeed() : 20f;

        screenPosition = Input.mousePosition;
        screenPosition.z = Camera.main.WorldToScreenPoint(crosshairCanvas.transform.position - new Vector3(0, 0, distance)).z;
        Debug.Log(Camera.main.name);

        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

        targetWorldPosition = Vector3.Lerp(targetWorldPosition, mouseWorldPosition, Time.deltaTime * smoothSpeed);
        transform.position = targetWorldPosition;
    }


    //void handleSprayZAxis() {
    //    screenPosition.z = Camera.main.WorldToScreenPoint(crosshairCanvas.transform.position - new Vector3(0, 0, distance)).z;
    //}
}