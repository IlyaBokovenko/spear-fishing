using UnityEngine;
using System.Collections;

public class FishVelocityDrivenSeekingBehaviour : GenericSeekingBehaviour {

    public override Vector3 From(){
        return nose;
    }
    
    public override SteeringOutput GetSteering (){
        Profiler.StartProfile(PT.Seeking);        
                
        velocityMatcher.velocity = Direction() * maxSpeed;
        orientationMatcher.orientation = rigidbody.velocity;
        SteeringOutput ret = velocityMatcher.GetSteering() + orientationMatcher.GetSteering();                      
            
        Profiler.EndProfile(PT.Seeking);
        return ret;
    }
}