using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class liveCanvasHandler : MonoBehaviour {
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI layerText;
    [SerializeField] private TextMeshProUGUI CapText;

    void Update() {
        // Set preview color with reduced alpha
        if (ColorManager.instance != null) {
            Color colorWithAlpha = ColorManager.instance.currentColor;
            colorWithAlpha.a = 0.3f;
            image.color = colorWithAlpha;
        }

        // Show which paint layer is active
        var pm = PaintManager.instance;
        if (pm != null) {
            layerText.text = pm.layer2 ? "Top layer" : "Base layer";
        }

        // Show the name of the current spray can
        var sprayManager = SprayManager.instance;
        if (sprayManager != null && sprayManager.currentSpray != null) {
            CapText.text = sprayManager.currentSpray.name;
        }
    }
}
