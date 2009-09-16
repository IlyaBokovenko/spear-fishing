using UnityEngine;
using System.Collections;

public class FishVelocityDrivenSeekingBehaviour : GenericSeekingBehaviour {

    public override Vector3 Direction(){
        return DirectionFromNose();
    }
    
    public override SteeringOutput GetSteering (){
        Profiler.StartProfile(PT.Seeking);
        
        SteeringOutput ret;
        
        if(!velocityMatcher || !target)
            ret = SteeringOutput.empty;      
        else{
            velocityMatcher.velocity = Direction() * maxSpeed;
            ret = velocityMatcher.GetSteering();          
        }
            
        Profiler.EndProfile(PT.Seeking);
        return ret;
    }
}