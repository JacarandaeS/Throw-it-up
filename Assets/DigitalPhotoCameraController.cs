using System;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class DigitalPhotoCameraController : MonoBehaviour {
    [Header("Zoom Settings")]
    [SerializeField] private float fieldOfViewDifference = 5f;
    [SerializeField] private float minFOV = 15f;
    [SerializeField] private float maxFOV = 90f;

    [Header("Photo Settings")]
    [SerializeField] private RenderTexture renderTexture;
    [Tooltip("Resolution to save as PNG (should match renderTexture).")]
    [SerializeField] private Vector2Int outputResolution = new Vector2Int(1920, 1080);

    private Camera _camera;

    void Start() {
        _camera = GetComponent<Camera>();

        if (renderTexture == null) {
            Debug.LogError("RenderTexture not assigned.");
        }
        else {
            _camera.targetTexture = renderTexture;
        }
    }

    private void LateUpdate() {
        HandleZoom();

        if (Mouse.current.leftButton.wasPressedThisFrame) {
            StartCoroutine(CapturePhotoNextFrame());
        }
    }

    private IEnumerator CapturePhotoNextFrame() {
        yield return new WaitForEndOfFrame();
        TakePhoto();
    }


    private void HandleZoom() {
        float scroll = Mouse.current.scroll.ReadValue().y;
        if (Mathf.Abs(scroll) > 0.01f) {
            _camera.fieldOfView -= scroll * fieldOfViewDifference;
            _camera.fieldOfView = Mathf.Clamp(_camera.fieldOfView, minFOV, maxFOV);
        }
    }

    private void TakePhoto() {
        if (renderTexture == null) {
            Debug.LogError("RenderTexture is missing.");
            return;
        }

        // Temporarily set this camera to render manually
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = renderTexture;

        // Force render
        _camera.enabled = true;
        _camera.Render();

        // Create texture to copy from renderTexture
        Texture2D photo = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
        photo.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        photo.Apply();
        RenderTexture.active = currentRT;

        // Save PNG
        byte[] bytes = photo.EncodeToPNG();
        Destroy(photo);

        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string fileName = $"Photo_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png";
        string fullPath = Path.Combine(desktopPath, fileName);
        File.WriteAllBytes(fullPath, bytes);

        Debug.Log($"?? Photo saved to: {fullPath}");
    }

}
