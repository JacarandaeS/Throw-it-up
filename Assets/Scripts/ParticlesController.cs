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



    private ParticleSystem part;
    private List<ParticleCollisionEvent> collisionEvents;

    void Start() {
        part = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    void OnParticleCollision(GameObject other) {
        int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents); Paintable p = other.GetComponent<Paintable>();
        p.MarkAsPainted();
        if (p != null) {
            for (int i = 0; i < numCollisionEvents; i++) {
                Vector3 pos = collisionEvents[i].intersection;


                PaintManager.instance.paint(
                    p,
                    pos,
                    radius: size,
                    hardness: hardness,
                    strength: strength,
                    color: paintColor

                );
            }
        }
    }

    void Update() {
        paintColor = ColorManager.instance.currentColor;

    }



}
