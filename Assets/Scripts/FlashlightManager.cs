using UnityEngine;

public class FlashlightManager : MonoBehaviour {
    [SerializeField] private GameObject flashlight;
    private bool isTurnedOn = false;

    void Update() {
        if (Input.GetKeyDown(KeyCode.F)) {
            isTurnedOn = !isTurnedOn;
            flashlight.SetActive(isTurnedOn);
        }
    }
}
