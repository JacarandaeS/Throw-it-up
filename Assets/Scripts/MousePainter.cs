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

    private float originalRadius; // Store the original radius
    private Vector3? lastPaintPos = null; // Nullable Vector3

    void Start() {
        originalRadius = radius; // Initialize the original radius
        
    }
    private void OnEnable() {
        PaintManager.instance.currentBrushMode = PaintManager.BrushMode.Texture;
    }

    void Update() {
        paintColor = ColorManager.instance.currentColor;
        bool click = mouseSingleClick ? Input.GetMouseButtonDown(0) : Input.GetMouseButton(0);


        if (Input.GetKey(KeyCode.LeftControl)) {
            radius += 0.1f;
            radius = Mathf.Min(radius, 2f); // Set a maximum limit (adjust as needed)
        }
        else if (radius > originalRadius)  // Only decrease if above original
        {
            radius -= 0.1f;
            radius = Mathf.Max(radius, originalRadius); // Don't go below original
        }

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
                    float step = Mathf.Max(0.05f, radius * 0.3f); // Clamp minimum step size
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