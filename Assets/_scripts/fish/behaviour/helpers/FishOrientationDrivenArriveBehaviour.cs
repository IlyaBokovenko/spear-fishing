using UnityEngine;
using System.Collections;

public class FishOrientationDrivenArriveBehaviour : FishOrientationDrivenSeekingBehaviour {
    public float satisfactionRadius;
    
    float originalMaxSpeed;
    
    void Awake(){
        originalMaxSpeed = maxSpeed;
    }
    
    public override SteeringOutput GetSteering(){
        float distance = Distance();
        maxSpeed = distance >= satisfactionRadius ? originalMaxSpeed : distance / satisfactionRadius;
        return base.GetSteering();
    }
     
 }
