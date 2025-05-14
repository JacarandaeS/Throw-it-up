//using System.Diagnostics;
using UnityEngine;

public class HandleCameraLateral : MonoBehaviour
{
    [SerializeField] private float leftTop;
    [SerializeField] private float rightTop;
    void Update()
    {
        float movementSpeed = GameManager.instance.GetCameraSpeed(); // ? Use camera speed from GameManager


        if (Input.GetKey(KeyCode.D)) {
            Vector3 pos = transform.position;
            if(pos.x > rightTop) {
                return;
            }else {
                pos.x += movementSpeed;
               // Debug.Log(pos.x);
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
                Debug.Log(pos.x);
                transform.position = pos;
            }
           
        }
    }
}
