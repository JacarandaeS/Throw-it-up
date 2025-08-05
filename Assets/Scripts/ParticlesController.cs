using System.Collections.Generic;
using UnityEngine;

public class ParticlesController : MonoBehaviour {
    [Header("Paint Settings")]
    public Color paintColor;
    public float strength = 1;
    public float hardness = 1;
    public float size = 1;

    [SerializeField] private float MaxSize;
    [SerializeField] private float MinSize;



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
    public void SizeUp() {
        if(size < MaxSize) {
         size += 0.02f;

        }
        return;
    }
    public void SizeDown() {
        if (size > MinSize) {
            size -= 0.02f;

        }
        return;
    }
}



