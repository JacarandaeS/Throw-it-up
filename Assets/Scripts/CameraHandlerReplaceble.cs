using Unity.Cinemachine;
using UnityEngine;

public class CameraHandlerReplaceble : MonoBehaviour {

    //esto se tiene que llamar de otra forma, seria el desplazamiento en el modo sidescroll

    [SerializeField] private float frontBorder = 2.5f;
    [SerializeField] private float backBorder = -16.0f;
    [SerializeField] private float leftBorder = 0;
    [SerializeField] private float rightBorder = 0;
    [SerializeField] private CinemachineCamera playerCamera;
    public float movementSpeed = 1f;
    private bool isCrouch = false;
    private bool crouchKeyPressedLastFrame = false;
    void FixedUpdate() {
        if (GameManager.instance != null) {
            float movementSpeed = GameManager.instance.GetCameraSpeed(); // ? Use camera speed from GameManager
        }
        if (!IsActiveCamera()) return;

        if (Input.GetKey(KeyCode.D)) {
            Vector3 pos = transform.position;
            if (pos.x > rightBorder) {
                return;
            }
            pos.x += movementSpeed;
            transform.position = pos;
        }

        if (Input.GetKey(KeyCode.A)) {
            Vector3 pos = transform.position;
            if (pos.x < leftBorder) {
                return;
            }
            pos.x -= movementSpeed;
            transform.position = pos;
        }

        if (Input.GetKey(KeyCode.S)) {
            Vector3 pos = transform.position;
            if (pos.z < backBorder) {
                return;
            }
            else {
                pos.z -= movementSpeed;
                transform.position = pos;
            }
        }

        if (Input.GetKey(KeyCode.W)) {
            Vector3 pos = transform.position;
            if (pos.z > frontBorder) {
                return;
            }
            else {
                pos.z += movementSpeed;
                transform.position = pos;
            }

        }
       bool crouchKeyPressed = Input.GetKey(KeyCode.C);
        
        if (crouchKeyPressed && !crouchKeyPressedLastFrame)
        {
            Vector3 pos = transform.position;
            if (!isCrouch)
            {
                pos.y -= 2f;
                isCrouch = true;
            }
            else
            {
                pos.y += 2f;
                isCrouch = false;
            }
            transform.position = pos;
        }
        
        crouchKeyPressedLastFrame = crouchKeyPressed;
    }
    private bool IsActiveCamera() {
        return playerCamera != null && playerCamera.Priority >= 9;
    }
}

    

