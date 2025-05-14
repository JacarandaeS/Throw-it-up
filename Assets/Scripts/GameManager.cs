using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    [SerializeField] private GameObject optionScreen;
    [SerializeField] private Slider smoothSpeedSlider; // Existing slider
    [SerializeField] private Slider cameraSpeedSlider; // ? New slider for camera speed
    [SerializeField] private float smoothSpeed = 8f;
    [SerializeField] private float cameraSpeed = 0.1f;  // ? New value
    [SerializeField] private GameObject activeCans;

    private bool isTurnedOn = false;

    public static GameManager instance { get; private set; }

    void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
            return;
        }

        // Set initial values from sliders
        if (smoothSpeedSlider != null) {
            SetSmoothSpeed(smoothSpeedSlider.value);
        }

        if (cameraSpeedSlider != null) {
            SetCameraSpeed(cameraSpeedSlider.value);
        }
    }

    void Update() {
        if (Input.GetKeyUp(KeyCode.Escape)) {
            isTurnedOn = !isTurnedOn;
            optionScreen.SetActive(isTurnedOn);
            activeCans.SetActive(!isTurnedOn);
        }

        // Update both speeds continuously
        if (smoothSpeedSlider != null) {
            SetSmoothSpeed(smoothSpeedSlider.value);
        }

        if (cameraSpeedSlider != null) {
            SetCameraSpeed(cameraSpeedSlider.value);
        }
    }

    public void Quit() {
        Debug.Log("Quitting the game...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    public void RestartScene() {
        Debug.Log("Restarting scene...");
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public float GetSmoothSpeed() {
        return smoothSpeed;
    }

    public void SetSmoothSpeed(float value) {
        smoothSpeed = value;
        Debug.Log($"Smooth speed set to: {smoothSpeed}");
    }

    // ? New camera speed getter/setter
    public float GetCameraSpeed() {
        return cameraSpeed;
    }

    public void SetCameraSpeed(float value) {
        cameraSpeed = value;
    }
}
