using UnityEngine;
using System.Collections;

public class FishLookAheadBehaviour : FishBehaviour
{
    private OrientationMatching orientationMatcher;

    void Start(){
        orientationMatcher = (OrientationMatching)gameObject.AddComponent(typeof(OrientationMatching));
    }

    public override void SelfDestroy(){
        orientationMatcher.SelfDestroy();
        base.SelfDestroy();
    }

    public override SteeringOutput GetSteering(){
        if(!orientationMatcher)
            return SteeringOutput.empty;

        if(!Utils.Approximately(0, rigidbody.velocity.magnitude))
            orientationMatcher.orientation = rigidbody.velocity;
        else
            orientationMatcher.orientation = transform.forward;

        return orientationMatcher.GetSteering();
    }
}