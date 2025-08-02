using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    [SerializeField] private GameObject optionScreen;
    [SerializeField] private Slider smoothSpeedSlider;
    [SerializeField] private Slider cameraSpeedSlider;
    [SerializeField] private float smoothSpeed = 8f;
    [SerializeField] private float cameraSpeed = 0.1f;
    [SerializeField] private GameObject activeCans;
    [SerializeField] private GameObject optionObject;
    //[SerializeField] private GameObject colorPickerCanvas;
   // private bool TopLayerPainting = true;

    private bool isTurnedOn = false;

    public static GameManager instance { get; private set; }

    void Awake() {
        if (instance == null) {
            instance = this;
            //Cursor.visible = false;
        }
        else {
            Destroy(gameObject);
            return;
        }

        // Set initial slider values and subscribe to changes
        if (smoothSpeedSlider != null) {
            smoothSpeedSlider.value = smoothSpeed;
            smoothSpeedSlider.onValueChanged.AddListener(SetSmoothSpeed);
        }

        if (cameraSpeedSlider != null) {
            cameraSpeedSlider.value = cameraSpeed;
            cameraSpeedSlider.onValueChanged.AddListener(SetCameraSpeed);
        }
    }

    void Update() {
        if (Input.GetKeyUp(KeyCode.Escape)) {
            isTurnedOn = !isTurnedOn;
            optionScreen.SetActive(isTurnedOn);
            activeCans.SetActive(!isTurnedOn);
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

    public float GetCameraSpeed() {
        return cameraSpeed;
    }

    public void SetCameraSpeed(float value) {
        cameraSpeed = value;
        Debug.Log($"Camera speed set to: {cameraSpeed}");
    }

    public void EnableOptions() {

    }
}
