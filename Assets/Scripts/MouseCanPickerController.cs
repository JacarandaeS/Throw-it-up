using UnityEngine;

public class MouseCanPickerController : MonoBehaviour {
    public LayerMask targetLayer;
    public Transform tableCansParent;      // drag your TableCansInList GO here\

    public float hoverOffset = 0.3f;
    public float hoverSpeed = 5f;

    Vector3 lastHoveredOriginalPosition;
    Vector3 lastHoveredTargetPosition;
    Transform lastHoveredObject = null;

    void Update() {
        HandleObjectHover();
        UpdateMousePosition();

        if (lastHoveredObject != null) {
            lastHoveredObject.position = Vector3.Lerp(
                lastHoveredObject.position,
                lastHoveredTargetPosition,
                Time.deltaTime * hoverSpeed
            );
        }

        if (Input.GetMouseButtonDown(0) && lastHoveredObject != null) {
            // pick the color
            var colorHolder = lastHoveredObject.GetComponent<CanColorHolder>();
            if (colorHolder != null) {
                var picked = colorHolder.color;
                Debug.Log($"Picked can color = {picked}");
                if (ColorManager.instance != null) {
                    ColorManager.instance.HandPickedpalette.Add(picked);
                    ColorManager.instance.currentColor = picked;
                }
            }

            string canName = lastHoveredObject.gameObject.name;
            lastHoveredObject.gameObject.SetActive(false);

            var tableCopy = tableCansParent.Find(canName);
            if (tableCopy != null) {
                tableCopy.gameObject.SetActive(true);
            }
            else {
                Debug.LogWarning($"Could not find a copy named '{canName}' under {tableCansParent.name}");
            }
        }
    }


    // … rest of your hover & OnDisable logic …




    void UpdateMousePosition() {
        Vector3 screenPos = Input.mousePosition;
        screenPos.z = Camera.main.WorldToScreenPoint(transform.position).z;
        transform.position = Camera.main.ScreenToWorldPoint(screenPos);
    }

    void HandleObjectHover() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, targetLayer)) {
            Transform canTransform = hit.collider.transform.parent;

            // on left?click, read its color then destroy
            if (Input.GetMouseButtonDown(0)) {
                var colorHolder = canTransform.GetComponent<CanColorHolder>();
                if (colorHolder != null) {
                    Color picked = colorHolder.color;
                    Debug.Log($"Picked can color = {picked}");

                    
                    if (ColorManager.instance != null) {
                        ColorManager.instance.HandPickedpalette.Add(picked);
                        // (optional) immediately make it the current color:
                        ColorManager.instance.currentColor = picked;
                    }
                    else {
                        Debug.LogWarning("No ColorManager instance found!");
                    }
                }
                Debug.Log(canTransform.gameObject.name);
                canTransform.gameObject.SetActive(false);
               //Destroy(canTransform.gameObject);
            }

            // hover?up logic (unchanged)…
            if (canTransform != lastHoveredObject) {
                if (lastHoveredObject != null)
                    lastHoveredObject.position = lastHoveredOriginalPosition;

                lastHoveredOriginalPosition = canTransform.position;
                lastHoveredTargetPosition = lastHoveredOriginalPosition + Vector3.up * hoverOffset;
                lastHoveredObject = canTransform;
            }
        }
        else {
            if (lastHoveredObject != null) {
                lastHoveredTargetPosition = lastHoveredOriginalPosition;
                if (Vector3.Distance(lastHoveredObject.position, lastHoveredOriginalPosition) < 0.01f) {
                    lastHoveredObject.position = lastHoveredOriginalPosition;
                    lastHoveredObject = null;
                }
            }
        }
    }

    void OnDisable() {
        if (lastHoveredObject != null)
            lastHoveredObject.position = lastHoveredOriginalPosition;
    }
}
