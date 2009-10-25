using UnityEngine;
using System.Collections;

public class FishOrientationDrivenArriveBehaviour : FishOrientationDrivenSeekingBehaviour {
    public float satisfactionRadius = 2;
    
    protected override float ComputeMaxSpeed(){
        float distance = Distance();
        float speed = base.ComputeMaxSpeed();
        if(distance < satisfactionRadius) speed *= distance / satisfactionRadius;
        return speed;
    }
    
    public override SteeringOutput GetSteering(){
        maxSpeed = ComputeMaxSpeed();
        return base.GetSteering();
    }    
     
 }
