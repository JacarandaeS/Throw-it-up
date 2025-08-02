using System;
using UnityEngine;

public class MouseHandlerParticleChanger : MonoBehaviour {
    [SerializeField] ParticleSystem sprayParticles;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip spraySound;
    

    private bool pinturaActiva = false;
    public float baseAmount = 0f;
    public float maxAmount = 0.3f;
    private float currentAmount;
    private float returnTimer = 0f;
    private bool isReturning = false;
    private float angle;

    private void Start() {
        if (sprayParticles == null) {
            sprayParticles = GetComponentInChildren<ParticleSystem>();
        }
        angle = sprayParticles.shape.angle;
        currentAmount = baseAmount;
        SetSphericalDirectionAmount(currentAmount);

        // Asegurarse de que arranca desactivado
        sprayParticles.gameObject.SetActive(false);
    }

    private void FixedUpdate() {
        //HandleAmountChange();
        handleAngleChange();


    }

    void Update() {
        HandleOnOff();
        
    }

    void HandleOnOff() {
        // Actualizar color del spray si hay un ColorManager
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
    void handleAngleChange() {
        if (Input.GetKey(KeyCode.LeftControl)) {
            angle += 3f;
        }
        else {
            angle -= 3f;
        }

        // Limitar entre 5 y 60 grados
        angle = Mathf.Clamp(angle, 5f, 65f);

        var shape = sprayParticles.shape;
        shape.angle = angle;


    }

    //void HandleAmountChange() {
    //    if (Input.GetKey(KeyCode.LeftControl)) {
    //        isReturning = false;
    //        returnTimer = 0f;

    //        currentAmount = Mathf.MoveTowards(currentAmount, maxAmount, Time.deltaTime * 2f);
    //        SetSphericalDirectionAmount(currentAmount);
    //    }
    //    else {
    //        if (currentAmount > baseAmount) {
    //            isReturning = true;
    //        }

    //        if (isReturning) {
    //            returnTimer += Time.deltaTime;
    //            float t = Mathf.Clamp01(returnTimer / 3f);
    //            currentAmount = Mathf.Lerp(currentAmount, baseAmount, t);
    //            SetSphericalDirectionAmount(currentAmount);

    //            if (Mathf.Abs(currentAmount - baseAmount) < 0.01f) {
    //                currentAmount = baseAmount;
    //                isReturning = false;
    //                returnTimer = 0f;
    //            }
    //        }
    //    }
    //}

    void SetSphericalDirectionAmount(float value) {
        var shape = sprayParticles.shape;
        shape.sphericalDirectionAmount = value;
    }
}
