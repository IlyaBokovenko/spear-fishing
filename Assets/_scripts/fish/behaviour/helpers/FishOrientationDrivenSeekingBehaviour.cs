using UnityEngine;
using System.Collections;

public class FishOrientationDrivenSeekingBehaviour : GenericSeekingBehaviour {    
    
    public override SteeringOutput GetSteering(){
        orientationMatcher.orientation = Direction();
        velocityMatcher.velocity = _transform.forward * maxSpeed;
        return velocityMatcher.GetSteering() + orientationMatcher.GetSteering();
    }
    
}
