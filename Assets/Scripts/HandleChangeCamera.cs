using Unity.Cinemachine;
using UnityEngine;
using System.Collections;

public class HandleChangeCamera : MonoBehaviour {
    public GameObject canvasToEnable;
    [SerializeField] private CinemachineBrain brain;
    public CinemachineCamera vcam1;
    public CinemachineCamera vcam2;
    [SerializeField] private GameObject activeCan;

    private bool isInsideTrigger = false;
    private bool isCam1Active = true;

    void Start() {
        if (vcam1 != null && vcam2 != null) {
            vcam1.Priority = 10;
            vcam2.Priority = 0;
        }
    }

    void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("MainCamera")) return;
        isInsideTrigger = true;
        if (canvasToEnable != null)
            canvasToEnable.SetActive(true);
    }

    void OnTriggerExit(Collider other) {
        if (!other.CompareTag("MainCamera")) return;
        isInsideTrigger = false;
        if (canvasToEnable != null)
            canvasToEnable.SetActive(false);
    }

    void Update() {
        if (isInsideTrigger && Input.GetKeyDown(KeyCode.E)) {
            isCam1Active = !isCam1Active;

            // Deactivate spray
            if (SprayManager.instance != null && SprayManager.instance.currentSpray != null) {
                SprayManager.instance.currentSpray.SetActive(false);
            }

            // Change camera priority
            vcam1.Priority = isCam1Active ? 10 : 0;
            vcam2.Priority = isCam1Active ? 0 : 10;

            // Wait for camera to settle and reposition spray
            StartCoroutine(WaitForBlendAndRepositionSpray());
        }
    }

    IEnumerator WaitForBlendAndRepositionSpray() {
        if (SprayManager.instance != null)
            SprayManager.instance.raycastEnabled = false; // disable raycast right away

        // Wait until camera blending is done
        while (brain.IsBlending)
            yield return null;

        // Wait 2 more seconds after blending
        yield return new WaitForSeconds(2f);

        // Reposition spray
        if (SprayManager.instance != null && SprayManager.instance.currentSpray != null) {
            var cam = isCam1Active ? vcam1 : vcam2;
            Transform camTransform = cam.transform;

            SprayManager.instance.transform.position = camTransform.position + camTransform.forward * 2f;
            SprayManager.instance.targetWorldPosition = SprayManager.instance.transform.position;

            SprayManager.instance.currentSpray.SetActive(true);
            SprayManager.instance.raycastEnabled = true; // re-enable raycast AFTER full reposition
        }
    }
}

