using System;
using UnityEngine;

public class MouseHandlerParticleChanger : MonoBehaviour {
    [SerializeField] ParticleSystem sprayParticles;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip spraySound;
   // [SerializeField] private ParticlesController particlesController;

    private bool pinturaActiva = false;
    private float returnTimer = 0f;
    private bool isReturning = false;
    private float angle;
    public bool changeAngle = true;
    private ParticlesController particlesController;

    private void Start() {
        if (sprayParticles == null) {
            sprayParticles = GetComponentInChildren<ParticleSystem>();
        }

        if (particlesController == null && sprayParticles != null) {
            particlesController = sprayParticles.GetComponent<ParticlesController>();
        }

        angle = sprayParticles.shape.angle;
        sprayParticles.gameObject.SetActive(false);
       // PaintManager.instance.currentBrushMode = PaintManager.BrushMode.Circle;
    }


    private void FixedUpdate() {
        if (changeAngle) {
            handleAngleChange();
        }
    }

    void Update() {
        HandleOnOff();
        HandleScroll();
    }
    private void OnEnable() {
        PaintManager.instance.currentBrushMode = PaintManager.BrushMode.Circle;
    }
    void HandleOnOff() {
        if (ColorManager.instance != null && sprayParticles != null) {
            var main = sprayParticles.main;
            main.startColor = ColorManager.instance.currentColor;
        }

        if (Input.GetMouseButton(0) && !pinturaActiva) {
            sprayParticles.gameObject.SetActive(true);
            pinturaActiva = true;

            if (spraySound != null && audioSource != null) {
                audioSource.clip = spraySound;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        else if (!Input.GetMouseButton(0) && pinturaActiva) {
            sprayParticles.gameObject.SetActive(false);
            pinturaActiva = false;

            if (audioSource != null && audioSource.isPlaying) {
                audioSource.Stop();
            }
        }
    }

    void HandleScroll() {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f) {
            particlesController?.SizeUp();
        }
        else if (scroll < 0f) {
            particlesController?.SizeDown();
        }
    }

    void handleAngleChange() {
        if (Input.GetKey(KeyCode.LeftControl)) {
            angle += 3f;
        }
        else {
            angle -= 3f;
        }

        angle = Mathf.Clamp(angle, 5f, 65f);
        var shape = sprayParticles.shape;
        shape.angle = angle;
        Debug.Log("Updated angle: " + angle);
    }

    void SetSphericalDirectionAmount(float value) {
        var shape = sprayParticles.shape;
        shape.sphericalDirectionAmount = value;
    }
}
