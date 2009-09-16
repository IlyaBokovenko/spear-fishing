using UnityEngine;
using System.Collections;

public class FishLookAheadBehaviour : FishBehaviour
{
    public OrientationMatching orientationMatcher;

    void Awake(){
        children = new FishBehaviour[1]{orientationMatcher};        
    }

    public override SteeringOutput GetSteering(){        
        Profiler.StartProfile(PT.LookAhead);
        
        if(!orientationMatcher)
            return SteeringOutput.empty;

        if(!Utils.Approximately(0, rigidbody.velocity.magnitude))
            orientationMatcher.orientation = rigidbody.velocity;
        else
            orientationMatcher.orientation = transform.forward;

        SteeringOutput ret = orientationMatcher.GetSteering();
        
        Profiler.EndProfile(PT.LookAhead);
        
        return ret;
    }
}