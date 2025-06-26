using UnityEngine;

public class BrushController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private bool useCircleBrush = true;

    private Material painterMaterial;
    private static readonly int BrushShape = Shader.PropertyToID("_BrushShape");

    private void Start() {
        painterMaterial = GetComponent<Renderer>().material;
        UpdateBrushShape();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.B)) // Press B to toggle brush shape
        {
            useCircleBrush = !useCircleBrush;
            UpdateBrushShape();
            Debug.Log($"Switched to {(useCircleBrush ? "Circle" : "Square")} brush");
        }
    }

    private void UpdateBrushShape() {
        painterMaterial.SetInt(BrushShape, useCircleBrush ? 0 : 1);
    }

    // Public method to change brush shape from other scripts
    public void SetBrushShape(bool circleBrush) {
        useCircleBrush = circleBrush;
        UpdateBrushShape();
    }
}
