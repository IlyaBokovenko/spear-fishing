using UnityEngine;
using System.Collections;

public class OrientationMatching : FishBehaviour {
	
    public Vector3 orientation;
    public float maxRotationSpeed  = 360f;
    public float slowAngle = 30f;
    public float maxRotationAcceleration = 500f;       
    public float timeToMatchOrientation = 0.1f;
    
    public override SteeringOutput GetSteering (){
        Profiler.StartProfile(PT.OrientationMatching);
        
        Vector3 fromHeading = transform.forward;
        Vector3 toHeading = orientation;
        Vector3 delta = Quaternion.FromToRotation(fromHeading, toHeading).eulerAngles;        
        delta = Utils.DegToShifted(delta);
        
        Vector3 rotation = calcRotation(delta);
        float angle = Mathf.Abs(Vector3.Angle(fromHeading, toHeading));
        if(angle < slowAngle)
            rotation *= angle / slowAngle;

        Vector3 deltaVelocity = rotation - rigidbody.angularVelocity * Mathf.Rad2Deg;        
        Vector3 acceleration = deltaVelocity / timeToMatchOrientation;
        
        acceleration = Utils.ClampComponents(acceleration, -maxRotationAcceleration, maxRotationAcceleration);
        
        Profiler.EndProfile(PT.OrientationMatching);
        
        return SteeringOutput.WithTorque(acceleration * Mathf.Deg2Rad);   
    }
    
    
////////////////////////////////////////////////////////////////////////////////////////


    private Vector3 calcRotation(Vector3  delta){
        float max = maxComponent(delta); 
        if(Utils.Approximately(max, 0.0f))
            return delta;
            
        Vector3 rotation = delta / max;
        rotation *= maxRotationSpeed;
        return rotation;
    }
    
    private float maxComponent(Vector3 v) {
        return Mathf.Max(
            Mathf.Max(  Mathf.Abs(v.x), Mathf.Abs(v.y)  ),
            Mathf.Abs(v.z)
            ); 
    }
    
    
}

