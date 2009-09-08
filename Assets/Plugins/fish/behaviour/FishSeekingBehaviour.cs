using UnityEngine;
using System.Collections;

public class FishSeekingBehaviour : FishBehaviour {
    public GameObject target;
    public float maxSpeed = 3;
    public bool isFlee= false;
    
    private VelocityMatching velocityMatcher;

    
    void Start(){
        velocityMatcher = (VelocityMatching)gameObject.AddComponent(typeof(VelocityMatching));        
    }
    
    public override void SelfDestroy(){
        Destroy(velocityMatcher);
        base.SelfDestroy();
    }


    public override SteeringOutput GetSteering (){
        Profiler.StartProfile(PT.Seeking);
        
        if(!velocityMatcher || !target)
            return new SteeringOutput();        
            
        Vector3 direction = (target.transform.position - transform.position).normalized;
        if(isFlee)
            direction = Vector3.zero - direction;     

        velocityMatcher.velocity = direction * maxSpeed;
        
        Profiler.EndProfile(PT.Seeking);
        return velocityMatcher.GetSteering();
    }

}