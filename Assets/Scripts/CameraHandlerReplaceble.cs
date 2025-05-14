using UnityEngine;

public class CameraHandlerReplaceble : MonoBehaviour {
    [SerializeField] private float frontBorder = 2.5f;
    [SerializeField] private float backBorder = -16.0f;
    void Update() {

        float movementSpeed = GameManager.instance.GetCameraSpeed(); // ? Use camera speed from GameManager


        if (Input.GetKey(KeyCode.D)) {
            Vector3 pos = transform.position;
            pos.x += movementSpeed;
            transform.position = pos;
        }

        if (Input.GetKey(KeyCode.A)) {
            Vector3 pos = transform.position;
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
    }
}

