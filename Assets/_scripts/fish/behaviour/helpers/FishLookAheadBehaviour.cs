using UnityEngine;
using System.Collections;

public class FishLookAheadBehaviour : FishBehaviour
{
    public OrientationMatching orientationMatcher;
    
    private Transform _transform;

    void Awake(){
        children = new FishBehaviour[1]{orientationMatcher};        
        
        _transform = transform;
    }

    public override SteeringOutput GetSteering(){        
        Profiler.StartProfile(PT.LookAhead);
        
        if(!orientationMatcher)
            return SteeringOutput.empty;

        if(!Utils.Approximately(0, rigidbody.velocity.magnitude))
            orientationMatcher.orientation = rigidbody.velocity;
        else
            orientationMatcher.orientation = _transform.forward;

        SteeringOutput ret = orientationMatcher.GetSteering();
        
        Profiler.EndProfile(PT.LookAhead);
        
        return ret;
    }
}