using UnityEngine;

public class CollisionPainter : MonoBehaviour {
    private Color paintColor = new Color(1, 0, 0, 1);

    public float radius = 1;
    public float strength = 1;
    public float hardness = 1;

    private void OnCollisionStay(Collision other) {
        Paintable p = other.collider.GetComponent<Paintable>();
        if (p != null) {
            Vector3 pos = other.contacts[0].point;
            PaintManager.instance.paint(p, pos, radius, hardness, strength, paintColor);
        }
    }
    void Update() {
        paintColor = ColorManager.instance.currentColor;
    }
}