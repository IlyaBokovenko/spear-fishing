using UnityEngine;
using System.Collections;

public class VelocityMatching : FishBehaviour {
    public Vector3 velocity;
    public float timeToMatch = 0.1f;
    public float maxAcceleration = 10;
    
    public override string ToString(){
        return base.ToString() + " " + velocity.magnitude + "(" + rigidbody.velocity.magnitude + ")";
    }
    
    void Start(){}
    
    public override SteeringOutput GetSteering(){
       Profiler.StartProfile(PT.Velocity);
        
       Vector3 fromVelocity = rigidbody.velocity;
       Vector3 toVelocity = velocity;
       Vector3 delta = toVelocity - fromVelocity;        
       Vector3 acceleration = delta / timeToMatch;
        
        acceleration = Utils.ClampMagnitude(acceleration, 0, maxAcceleration);        
        
        SteeringOutput ret = SteeringOutput.WithForce(acceleration);
        
        Profiler.EndProfile(PT.Velocity);
        
        return ret;
    }
}