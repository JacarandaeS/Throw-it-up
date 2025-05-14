using UnityEngine;

public class JointManager : MonoBehaviour
{
    public Joint child;

    public Joint GetChild() {
        return child;
    }

    public void Rotate(float angle) {
        transform.Rotate(Vector3.up * angle);
    }
}
