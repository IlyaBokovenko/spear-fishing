using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Nose))]
public class FishSeekingBehaviour : FishBehaviour {
    public GameObject target;
    public float maxSpeed = 3;
    public bool isFlee = false;
    
    private VelocityMatching velocityMatcher;
    private Vector3 nose;
    
    protected override ArrayList children
    {
        get {ArrayList ret = base.children; ret.Add(velocityMatcher); return ret; }
    }
    
    void Awake(){
        nose = ((Nose)GetComponent(typeof(Nose))).position;
    }

    
    void Start(){
        velocityMatcher = (VelocityMatching)gameObject.AddComponent(typeof(VelocityMatching));        
    }
    
    public override void SelfDestroy(){
        Destroy(velocityMatcher);
        base.SelfDestroy();
    }    

    public override SteeringOutput GetSteering (){
        Profiler.StartProfile(PT.Seeking);
        
        SteeringOutput ret;
        
        if(!velocityMatcher || !target)
            ret = SteeringOutput.empty;        
        else{
            Vector3 from = transform.TransformPoint(nose);
            Vector3 to = target.transform.position;
            Vector3 direction = (to - from).normalized;
            if(isFlee)
                direction = Vector3.zero - direction;     

            velocityMatcher.velocity = direction * maxSpeed;
            ret = velocityMatcher.GetSteering();            
        }
            
        Profiler.EndProfile(PT.Seeking);
        return ret;
    }

}