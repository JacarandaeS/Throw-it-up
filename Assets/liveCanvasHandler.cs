using UnityEngine;
using UnityEngine.UI;

public class liveCanvasHandler : MonoBehaviour {
    [SerializeField] private Image image;

    void Update() {
        if (ColorManager.instance != null) {
            Color colorWithAlpha = ColorManager.instance.currentColor;
            colorWithAlpha.a = 0.3f; // Set desired alpha
            image.color = colorWithAlpha;
        }
    }
}
