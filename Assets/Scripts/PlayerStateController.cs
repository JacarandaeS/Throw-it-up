using UnityEngine;
using Unity.Cinemachine;

public class PlayerStateController : MonoBehaviour {
    [SerializeField] private GameObject activePainter;
    [SerializeField] private CinemachineCamera camara1;
    [SerializeField] private GameObject photocamera;

    private bool isPainterActive = true;
    private MouseLook mouseLook;
    private Quaternion originalCamRotation;

   
    void Start() {
        if (camara1 != null) {
            mouseLook = camara1.GetComponent<MouseLook>();
            originalCamRotation = camara1.transform.rotation;
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.X)) {
            if (isPainterActive) {
                activePainter.SetActive(false);
                isPainterActive = false;

                if (mouseLook != null) {
                    originalCamRotation = camara1.transform.rotation; // store current rotation
                    mouseLook.enabled = true;
                    photocamera.SetActive(true);
                }
            }
            else {
                photocamera.SetActive(false);
                activePainter.SetActive(true);
                isPainterActive = true;

                if (mouseLook != null) {
                    mouseLook.enabled = false;
                    camara1.transform.rotation = originalCamRotation; // reset rotation
                }
            }
        }
    }
}
