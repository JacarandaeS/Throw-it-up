using UnityEngine;
using Unity.Cinemachine;

public class PlayerStateController : MonoBehaviour {
    [SerializeField] private CinemachineCamera camara1;
    [SerializeField] private GameObject activePainter;
    [SerializeField] private GameObject photocamera;
    [SerializeField] private GameObject CrosshairCanvas;

    private bool isPainterActive = true;
    private bool isCameraActive = false;
    private bool isFreeLookActive = false;
    private bool isCanvasActive = true;
    private MouseLook mouseLook;
    private Quaternion originalCamRotation;

    void Start() {
        if (camara1 != null) {
            mouseLook = camara1.GetComponent<MouseLook>();
            originalCamRotation = camara1.transform.rotation;
        }
    }

    void Update() {
        EnableMouseLook();
        HandlePhotoCameraEnable();
    }

    void EnableMouseLook() {
        if (Input.GetKeyDown(KeyCode.X)) {
            if (isPainterActive) {
                // Disable the painter
                activePainter.SetActive(false);
                CrosshairCanvas.SetActive(false);
                isPainterActive = false;

                if (mouseLook != null) {
                    originalCamRotation = camara1.transform.rotation; // store current rotation
                    mouseLook.enabled = true;
                    isFreeLookActive = true;
                }
            }
            else {
                // Only enable the painter if the photo camera is NOT active
                if (!photocamera.activeSelf) {
                    activePainter.SetActive(true);
                    CrosshairCanvas.SetActive(true);
                    isPainterActive = true;

                    if (mouseLook != null) {
                        mouseLook.enabled = false;
                        isFreeLookActive = false;
                        camara1.transform.rotation = originalCamRotation; // reset rotation
                    }
                }
                else {
                    Debug.Log("Cannot activate painter while photo camera is active.");
                }
            }
        }
    }

    void HandlePhotoCameraEnable() {
        if (Input.GetKeyDown(KeyCode.E)) {
            Debug.Log("is free look active: " + isFreeLookActive);

            if (!isFreeLookActive) {
                Debug.Log("Tried to activate photo camera with mouse look disabled.");
                return;
            }

            if (!isCameraActive) {
                photocamera.SetActive(true);
                isCameraActive = true;
            }
            else {
                photocamera.SetActive(false);
                isCameraActive = false;
            }
        }
    }
}
