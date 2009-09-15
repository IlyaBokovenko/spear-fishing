using UnityEngine;
using System.Collections;

public class FishLookAheadBehaviour : FishBehaviour
{
    public float maxRotationSpeed  = 360f;
    public float slowAngle = 30f;
    public float maxRotationAcceleration = 500f;       
    public float timeToLookWhereGoing = 0.1f;
    
    private OrientationMatching orientationMatcher;
    
    protected override ArrayList children
    {
        get {ArrayList ret = base.children; ret.Add(orientationMatcher); return ret; }
    }    

    void Start(){
        orientationMatcher = (OrientationMatching)gameObject.AddComponent(typeof(OrientationMatching));
        orientationMatcher.maxRotationSpeed = maxRotationSpeed;
        orientationMatcher.slowAngle = slowAngle;
        orientationMatcher.maxRotationAcceleration = maxRotationAcceleration;
        orientationMatcher.timeToMatchOrientation = timeToLookWhereGoing;
    }

    public override void SelfDestroy(){
        orientationMatcher.SelfDestroy();
        base.SelfDestroy();
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