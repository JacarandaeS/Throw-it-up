using UnityEngine;

public class MouseHandlerParticleChanger : MonoBehaviour {
    [SerializeField] ColorManager colorManager; // <- Name was missing
    [SerializeField] GameObject pintura;
    [SerializeField] ParticleSystem sprayParticles;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip spraySound;


    private bool pinturaActiva = false;
    public float baseAmount = 0f;
    public float maxAmount = .3f;
    private float currentAmount;
    private float returnTimer = 0f;
    private bool isReturning = false;

    private void Start() {
        if (sprayParticles == null) {
            sprayParticles = GetComponentInChildren<ParticleSystem>();
        }

        currentAmount = baseAmount;
        SetSphericalDirectionAmount(currentAmount);
    }

    void Update() {
        HandleOnOff();
        HandleAmountChange();
    }

    void HandleOnOff() {
        // Set the particle start color to the current color from the ColorManager singleton
        var main = sprayParticles.main;
        main.startColor = ColorManager.instance.currentColor;

        if (Input.GetMouseButton(0) && !pinturaActiva) {
            pintura.SetActive(true);
            pinturaActiva = true;

            if (spraySound != null && audioSource != null) {
                audioSource.clip = spraySound;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        else if (!Input.GetMouseButton(0) && pinturaActiva) {
            pintura.SetActive(false);
            pinturaActiva = false;

            if (audioSource != null && audioSource.isPlaying) {
                audioSource.Stop();
            }
        }
    }

    void HandleAmountChange() {
        if (Input.GetKey(KeyCode.LeftControl)) {
            isReturning = false;
            returnTimer = 0f;

            currentAmount = Mathf.MoveTowards(currentAmount, maxAmount, Time.deltaTime * 2f);
            SetSphericalDirectionAmount(currentAmount);
        }
        else {
            if (currentAmount > baseAmount) {
                isReturning = true;
            }

            if (isReturning) {
                returnTimer += Time.deltaTime;
                float t = Mathf.Clamp01(returnTimer / 3f);
                currentAmount = Mathf.Lerp(currentAmount, baseAmount, t);
                SetSphericalDirectionAmount(currentAmount);

                if (Mathf.Abs(currentAmount - baseAmount) < 0.01f) {
                    currentAmount = baseAmount;
                    isReturning = false;
                    returnTimer = 0f;
                }
            }
        }
    }

    void SetSphericalDirectionAmount(float value) {
        var shape = sprayParticles.shape;
        shape.sphericalDirectionAmount = value;
    }
}
