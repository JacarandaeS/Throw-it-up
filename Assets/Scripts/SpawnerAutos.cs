using UnityEngine;

public class SpawnerAutos : MonoBehaviour
{
    private float countdown = 0;
    [SerializeField] private float timerLimit = 100f;
    [SerializeField] private GameObject auto;


    private void Start() {
        Instantiate(auto, transform.position, auto.transform.rotation);
    }

    void Update()
    {
        if (countdown < timerLimit + Random.Range(300, 1200)) {
            countdown += 1.0f;
        }
        else {
            Instantiate(auto, transform.position, auto.transform.rotation);
            countdown = 0;
        }
    }
}
