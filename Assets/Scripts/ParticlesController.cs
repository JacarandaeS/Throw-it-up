using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Build;
using UnityEngine;

public class ParticlesController: MonoBehaviour{
    public Color paintColor;

    
    public float minRadius = 0.05f;
    public float maxRadius = 0.2f;
    public float strength = 1;
    public float hardness = 1;
    [Space]
    ParticleSystem part;
    List<ParticleCollisionEvent> collisionEvents;
   

    void Start(){
        part = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();    
    }

    void OnParticleCollision(GameObject other) {
        int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);

        Paintable p = other.GetComponent<Paintable>();
        if(p != null){
            for  (int i = 0; i< numCollisionEvents; i++){
                Vector3 pos = collisionEvents[i].intersection;
                float radius = Random.Range(minRadius, maxRadius);
                PaintManager.instance.paint(p, pos, radius, hardness, strength, paintColor);
            }
        }
    }
    void Update() {
        paintColor = ColorManager.instance.currentColor;
    }

    

    
}