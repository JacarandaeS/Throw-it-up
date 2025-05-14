using UnityEngine;

public class IKManager : MonoBehaviour {
    public Transform root;
    public Transform end;

    public GameObject target;
    public float threshold = 0.05f;

    float CalculateSlope(Transform joint) {
        float deltaTheta = 0.01f;
        float distance1 = GetDistance(end.transform.position, target.transform.position);

        joint.Rotate(Vector3.up * deltaTheta); // o cualquier otro eje

        float distance2 = GetDistance(end.transform.position, target.transform.position);
        
        joint.Rotate(Vector3.up *- deltaTheta);

        // Ojo: deberías devolver la diferencia aquí
        return (distance2 - distance1) / deltaTheta;
    }

    float GetDistance(Vector3 point1, Vector3 point2) {
        return Vector3.Distance(point1, point2);
    }
    private void Update() {

        if (GetDistance(end.transform.position, target.transform.position) > threshold) {
            float slope = CalculateSlope(root);
            root.Rotate(Vector3.up * -slope * 1200);
        }
        
    }
}
