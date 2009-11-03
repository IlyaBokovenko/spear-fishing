using UnityEngine;
using System.Collections;

public class FishOrientationDrivenSeekingBehaviour : GenericSeekingBehaviour {    
    
    public override Vector3 From(){
        return nose;
    }
    
    public override SteeringOutput GetSteering(){
        orientationMatcher.orientation = Direction();
        velocityMatcher.velocity = _transform.forward * maxSpeed;
        return velocityMatcher.GetSteering() + orientationMatcher.GetSteering();
    }
    
}
