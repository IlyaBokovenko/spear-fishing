using UnityEngine;
using System.Collections;

public class FishOrientationDrivenArriveBehaviour : FishOrientationDrivenSeekingBehaviour {
    public float satisfactionRadius = 2;
    
    float originalMaxSpeed;
    
    protected override void Awake(){
        base.Awake();
        originalMaxSpeed = maxSpeed;
    }
    
    public override SteeringOutput GetSteering(){
        float distance = Distance();
        maxSpeed = distance >= satisfactionRadius ? originalMaxSpeed : distance / satisfactionRadius;
        return base.GetSteering();        
    }
     
 }
