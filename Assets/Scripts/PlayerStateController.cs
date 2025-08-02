using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.UI;
using TMPro;

public class PlayerStateController : MonoBehaviour {
    [SerializeField] private CinemachineCamera camara1;
    [SerializeField] private GameObject activePainter;
    [SerializeField] private GameObject photocamera;
    [SerializeField] private GameObject crosshair;
    [SerializeField] private GameObject colorPickerCanvas;
    [SerializeField] private TextMeshProUGUI textMeshPro;
    
    private bool isPainterActive = true;
    private bool isCameraActive = false;
    private bool isFreeLookActive = false;
    private MouseLook mouseLook;
    private Quaternion originalCamRotation;

    void Start() {
        if (camara1 != null) {
            mouseLook = camara1.GetComponent<MouseLook>();
            originalCamRotation = camara1.transform.rotation;
        }
    }

    void Update() {
        HandleChangeOfView();
        HandlePhotoCameraEnable();

        if(isFreeLookActive == true && isCameraActive == false) {
            textMeshPro.text = "press x to paint";
            }else {
            textMeshPro.text = "";
        }
        }

    void HandleChangeOfView() {
        if (Input.GetKeyDown(KeyCode.X)) {
            if (isPainterActive) {
                enableFreelook();
            }
            else {
                // Only enable the painter if the photo camera is NOT active
                if (!photocamera.activeSelf) {
                 enablePainter();
                }
                else {
                    //Debug.Log("Cannot activate painter while photo camera is active.");
                    photocamera.SetActive(false);
                    isCameraActive = false;
                    enablePainter();
                }
            }
        }
    }
    void handleColorPickerUI() {
        if (Input.GetKeyUp(KeyCode.Tab)) {
            if (colorPickerCanvas.activeInHierarchy == false) {
                colorPickerCanvas.SetActive(true);
            }
            else {
                colorPickerCanvas.SetActive(false);
            }
        }   
    }

    void enableFreelook() {
        activePainter.SetActive(false);
        crosshair.SetActive(false);
        isPainterActive = false;

        if (mouseLook != null) {
            originalCamRotation = camara1.transform.rotation; // store current rotation
            mouseLook.enabled = true;
            isFreeLookActive = true;
        }
    }
    void enablePainter() {
        activePainter.SetActive(true);
        crosshair.SetActive(true);
        isPainterActive = true;

        if (mouseLook != null) {
            mouseLook.enabled = false;
            isFreeLookActive = false;
            camara1.transform.rotation = originalCamRotation; // reset rotation
        }
    }

    void HandlePhotoCameraEnable() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            //Debug.Log("is free look active: " + isFreeLookActive);

            if (!isFreeLookActive && isCameraActive == false) {
                enableFreelook();
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
