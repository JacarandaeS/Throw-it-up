using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesController : MonoBehaviour {
    [Header("Paint Settings")]
    public Color paintColor;
    public float minRadius = 0.05f;
    public float maxRadius = 0.2f;
    public float strength = 1;
    public float hardness = 1;
    public float size = 1;

    [Header("Layer Settings")]
    public bool paintOnSuperiorLayer = false;
    public KeyCode layerToggleKey = KeyCode.L;
    public float superiorLayerHardness = 0.9f;

    private ParticleSystem part;
    private List<ParticleCollisionEvent> collisionEvents;

    void Start() {
        part = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    void OnParticleCollision(GameObject other) {
        int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);
        Paintable p = other.GetComponent<Paintable>();

        if (p != null) {
            for (int i = 0; i < numCollisionEvents; i++) {
                Vector3 pos = collisionEvents[i].intersection;
                float currentHardness = paintOnSuperiorLayer ? superiorLayerHardness : hardness;
                Debug.Log(pos);
                PaintManager.instance.paint(
                    p,
                    pos,
                    radius: size,
                    hardness: currentHardness,
                    strength: strength,
                    color: paintColor,
                    paintOnSuperior: paintOnSuperiorLayer
                );
            }
        }
    }

    void Update() {
        paintColor = ColorManager.instance.currentColor;

        if (Input.GetKeyDown(layerToggleKey)) {
            Debug.Log("se toco la tecla");
            paintOnSuperiorLayer = !paintOnSuperiorLayer;
            Debug.Log($"Now painting on: {(paintOnSuperiorLayer ? "SUPERIOR" : "MAIN")} layer");
        }
    }
}