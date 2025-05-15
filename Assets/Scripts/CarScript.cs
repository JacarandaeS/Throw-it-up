using UnityEngine;

public class CarScript : MonoBehaviour {
    [SerializeField] bool isGoingAway;
    private float movementSpeed = .3f;
    private float backBorder = -71f;
    private float frontBorder = 160;

    void Update() {
        Vector3 pos = transform.position;
        if (isGoingAway) {
            if (pos.z > frontBorder) {
                Destroy(gameObject);
            }
            pos.z += movementSpeed;
            transform.position = pos;
        }

        if (!isGoingAway) {
            if (pos.z < backBorder) {
                Destroy(gameObject);
            }

            pos.z -= movementSpeed;
            transform.position = pos;
           // Debug.Log(pos);

        }
    }
}
