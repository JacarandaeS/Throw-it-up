using UnityEngine;

public class MousePainter : MonoBehaviour {
    public Camera cam;
    [Space]
    public bool mouseSingleClick;
    [Space]
    private Color paintColor = new Color(1, 0, 0, 1);

    public float radius = 1;
    public float strength = 1;
    public float hardness = 1;

    private Vector3? lastPaintPos = null; // Nullable Vector3

    void Update() {
        paintColor = ColorManager.instance.currentColor;
        bool click = mouseSingleClick ? Input.GetMouseButtonDown(0) : Input.GetMouseButton(0);

        if (click) {
            Vector3 position = Input.mousePosition;
            Ray ray = cam.ScreenPointToRay(position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 300.0f)) {
                Paintable p = hit.collider.GetComponent<Paintable>();
                if (p == null) return;

                Vector3 currentPos = hit.point;

                if (lastPaintPos == null || mouseSingleClick) {
                    PaintManager.instance.paint(p, currentPos, radius, hardness, strength, paintColor);
                    lastPaintPos = currentPos;
                }
                else {
                    float dist = Vector3.Distance(lastPaintPos.Value, currentPos);
                    float step = Mathf.Max(0.05f, radius * 0.5f); // Clamp minimum step size
                    int steps = Mathf.Clamp(Mathf.CeilToInt(dist / step), 1, 100); // Limit to avoid too many calls

                    for (int i = 0; i <= steps; i++) {
                        Vector3 lerpPos = Vector3.Lerp(lastPaintPos.Value, currentPos, (float)i / steps);
                        PaintManager.instance.paint(p, lerpPos, radius, hardness, strength, paintColor);
                    }

                    lastPaintPos = currentPos;
                }
            }
        }
        else {
            lastPaintPos = null; // Reset when mouse isn't held down
        }
    }
}
