using UnityEngine;

public class CamionArrancar : MonoBehaviour {
    private Animator Animator;
    private GameObject lights;
    private bool startTimer = false;
    private float startTimerOffset = 15f;
    private float timer; // countdown value

    void Start() {
        Animator = GetComponent<Animator>();

        // Find the child named "lights"
        Transform lightsTransform = transform.Find("lights");
        if (lightsTransform != null) {
            lights = lightsTransform.gameObject;
            lights.SetActive(false); // Ensure lights are off at start
        }
        else {
            Debug.LogWarning("No child named 'lights' was found.");
        }

        timer = startTimerOffset;
    }

    void Update() {
        if (startTimer) {
            timer -= Time.deltaTime;
            if (timer <= 0f) {
                startTimer = false;
                startTruck();
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        startTimer = true;
        timer = startTimerOffset; // Restart countdown from beginning
    }

    private void OnTriggerEnter(Collider other) {
        startTimer = false;       // Stop countdown
        timer = startTimerOffset; // Reset countdown time
    }

    private void startTruck() {
        Animator.SetBool("PlayerLeft", true);
        if (lights != null) {
            lights.SetActive(true);
        }
    }
}
