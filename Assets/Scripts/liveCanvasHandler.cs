using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class liveCanvasHandler : MonoBehaviour {
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI layerText;
    [SerializeField] private TextMeshProUGUI CapText;

    private void Start() {
        PlayerStateController playerStateController = FindAnyObjectByType<PlayerStateController>();
        playerStateController.OnCameraOpen += PlayerStateController_OnCameraOpen;
    }

    private void PlayerStateController_OnCameraOpen(object sender, System.EventArgs e) {
        //Debug.Log("funciono y lo llamamos desde aca live camera handler");
        //if(layerText.IsActive() && CapText.IsActive()) {
        //    Debug.Log("funciono y lo llamamos desde aca live camera handler");
        //    layerText.enabled = false;
        //    CapText.enabled = false;
        //}
        //else {
        //    Debug.Log("lo llamamos cuando esta apagado"); 
        //    layerText.enabled = true;
        //    CapText.enabled = true;
        //}
    }

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
