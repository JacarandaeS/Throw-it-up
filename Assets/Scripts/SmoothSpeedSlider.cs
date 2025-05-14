using UnityEngine;
using UnityEngine.UI;

public class SmoothSpeedSlider : MonoBehaviour {
    [SerializeField] private Slider slider;
    [SerializeField] private GameManager gameManager;

    void Start() {
        if (gameManager != null && slider != null) {
            slider.value = gameManager.GetSmoothSpeed();
        }
        else {
            Debug.LogWarning("Slider or GameManager reference is missing.");
        }
    }
}
