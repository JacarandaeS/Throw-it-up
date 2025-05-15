using UnityEngine;

public class CarScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private float movementSpeed = .3f;
    private float backBorder = -71f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        if (pos.z < backBorder) {
            Destroy(gameObject);
        }

        pos.z -= movementSpeed;
            transform.position = pos;
        Debug.Log(pos);
        
    }
}
