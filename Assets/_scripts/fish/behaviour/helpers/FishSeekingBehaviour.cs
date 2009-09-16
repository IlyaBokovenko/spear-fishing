using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Nose))]
public class FishSeekingBehaviour : FishBehaviour {
    public VelocityMatching velocityMatcher;
    
    public GameObject target;
    public float maxSpeed = 3;
    public bool isFlee = false;    
    
    private Vector3 nose;

    void Awake(){
        nose = ((Nose)GetComponent(typeof(Nose))).position;
        children = new FishBehaviour[1]{velocityMatcher};    
    }
    
    protected virtual Vector3 direction(){
        Vector3 from = transform.TransformPoint(nose);
        Vector3 to = target.transform.position;
        Vector3 direction = (to - from).normalized;
        return direction;        
    }   

    public override SteeringOutput GetSteering (){
        Profiler.StartProfile(PT.Seeking);
        
        SteeringOutput ret;
        
        if(!velocityMatcher || !target)
            ret = SteeringOutput.empty;        
        else{
            velocityMatcher.velocity = direction() * maxSpeed;
            ret = velocityMatcher.GetSteering();          
        }
            
        Profiler.EndProfile(PT.Seeking);
        return ret;
    }

}