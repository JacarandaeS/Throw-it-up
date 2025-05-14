using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerScript : MonoBehaviour {
    [SerializeField] private float movementSpeed = .1f;
    void Start() {

    }

    // Update is called once per frame
    void Update() {
       
        if (Input.GetKey(KeyCode.D)) {
            Vector3 pos = transform.position;
            pos.x += movementSpeed; 
            Debug.Log("lad");
            transform.position = pos;
        }
      
        if(Input.GetKey(KeyCode.A)) {
            Vector3 pos = transform.position;
            pos.x -= movementSpeed; 
            transform.position = pos;
        }

        if (Input.GetKey(KeyCode.W)) {
            Vector3 pos = transform.position;
            pos.z += movementSpeed; 
            transform.position = pos;
        }

        if (Input.GetKey(KeyCode.S)) {
            Vector3 pos = transform.position;
            pos.z -= movementSpeed; 
            transform.position = pos;
        }
    }
}