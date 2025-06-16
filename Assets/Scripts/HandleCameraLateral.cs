//using System.Diagnostics;
using Unity.Cinemachine;
using UnityEngine;

public class HandleCameraLateral : MonoBehaviour
{
    [SerializeField] private float leftTop;
    [SerializeField] private float rightTop;
    private CinemachineCamera virtualCamera;
    void Awake() {
        // Automatically gets the CinemachineVirtualCamera on the same GameObject
        virtualCamera = GetComponent<CinemachineCamera>();

        // Safety check
        if (virtualCamera == null) {
            Debug.LogError("No CinemachineVirtualCamera found on this GameObject!", this);
        }
    }
    void FixedUpdate()
    {
        float movementSpeed = GameManager.instance.GetCameraSpeed(); // ? Use camera speed from GameManager
        if (!IsActiveCamera()) return;

        if (Input.GetKey(KeyCode.D)) {
            Vector3 pos = transform.position;
            if(pos.x > rightTop) {
                return;
            }else {
                pos.x += movementSpeed;
                transform.position = pos;
            }
           
        }

        if (Input.GetKey(KeyCode.A)) {
            Vector3 pos = transform.position;
            if (pos.x < leftTop) {
                return;
            }
            else {
                pos.x -= movementSpeed;
                transform.position = pos;
            }

           
        }
        
    }
    private bool IsActiveCamera() {
        return virtualCamera != null && virtualCamera.Priority >= 10; // Same threshold as above
    }
}
