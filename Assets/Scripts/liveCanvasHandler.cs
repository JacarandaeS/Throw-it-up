using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class liveCanvasHandler : MonoBehaviour {
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI textMeshPro;

    void Update() {
        if (ColorManager.instance != null) {
            Color colorWithAlpha = ColorManager.instance.currentColor;
            colorWithAlpha.a = 0.3f; // Set desired alpha
            image.color = colorWithAlpha;
        }
        var pm = PaintManager.instance;
        if (pm != null) {
            if (pm.layer2) {
                textMeshPro.text = "Top layer";
            }
            else {
                textMeshPro.text = "Base layer";
            }
        }
    }
}
