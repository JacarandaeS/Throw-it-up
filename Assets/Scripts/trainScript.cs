using UnityEngine;

public class trainScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private float movementSpeed = .3f;
    private float leftBorder = -450f;
    

    void Update() {
      
        Vector3 pos = transform.position;
        if (pos.x < leftBorder) {
           Destroy(gameObject);
        }
        pos.x -= movementSpeed;
        transform.position = pos;

    }
    }
