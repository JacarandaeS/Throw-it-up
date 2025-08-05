using UnityEngine;

public class RollerPainter : MonoBehaviour {
    public Camera cam;
    [Space]
    public bool mouseSingleClick;
    [Space]
    private Color paintColor = new Color(1, 0, 0, 1);

    public float radius = 1;
    public float strength = 1;
    public float hardness = 1;

    private float originalRadius;
    private float originalStrength;

    private Vector3? lastPaintPos = null;

    private bool restoringStrength = false;
    private float strengthRestoreTimer = 0f;
    private float strengthRestoreDuration = 1f;

    void Start() {
        originalRadius = radius;
        originalStrength = strength;
    }

    private void OnEnable() {
        if (PaintManager.instance != null) {
            PaintManager.instance.currentBrushMode = PaintManager.BrushMode.Texture;
        }
    }

    void Update() {
        paintColor = ColorManager.instance.currentColor;
        bool click = mouseSingleClick ? Input.GetMouseButtonDown(0) : Input.GetMouseButton(0);

        radius = Mathf.Lerp(0.4f, originalRadius, strength / originalStrength);


        if (click) {
            // Decrease strength while holding mouse
            strength -= Time.deltaTime * 0.3f;
            strength = Mathf.Max(strength, 0.1f);

            restoringStrength = false;
            strengthRestoreTimer = 0f;

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
                    float step = Mathf.Max(0.05f, radius * 0.3f);
                    int steps = Mathf.Clamp(Mathf.CeilToInt(dist / step), 1, 100);

                    for (int i = 0; i <= steps; i++) {
                        Vector3 lerpPos = Vector3.Lerp(lastPaintPos.Value, currentPos, (float)i / steps);
                        PaintManager.instance.paint(p, lerpPos, radius, hardness, strength, paintColor);
                    }

                    lastPaintPos = currentPos;
                }
            }
        }
        else {
            lastPaintPos = null;

            // Start restoring strength after releasing click
            if (!restoringStrength && strength < originalStrength) {
                restoringStrength = true;
                strengthRestoreTimer = 0f;
            }
        }

        // Smoothly restore strength over time
        if (restoringStrength) {
            strengthRestoreTimer += Time.deltaTime;
            float t = strengthRestoreTimer / strengthRestoreDuration;
            strength = Mathf.Lerp(strength, originalStrength, t);

            if (t >= 1f) {
                strength = originalStrength;
                restoringStrength = false;
                strengthRestoreTimer = 0f;
            }
        }
    }
}
