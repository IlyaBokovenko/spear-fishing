using UnityEngine;
using System.Collections;

public class FishCollisionAvoidingBehaviour : FishArbitratedBehaviour {
    public float timeToThinkAhead = 3;
    public float minDistance = 2;
    
    
    private FishSeekingBehaviour seeking;
    private GameObject seekingTarget;
    
    FishCollisionAvoidingBehaviour(){
        priority = 2;
    }
	
	void Start () {
	    seeking = (FishSeekingBehaviour)gameObject.AddComponent(typeof(FishSeekingBehaviour));
	    seekingTarget = new GameObject("collision avoidance target");
	    seekingTarget.transform.parent = transform;
	    seeking.target = seekingTarget;
	}
	
	public override SteeringOutput GetSteering(){
	    if(!seeking)
	        return SteeringOutput.empty;
	    
	    RaycastHit hit;
	    float distance = rigidbody.velocity.magnitude * timeToThinkAhead;
	    int layerMask = 1 << gameObject.layer;
	    layerMask = ~layerMask;
	    bool collided = Physics.Raycast (transform.position, rigidbody.velocity, out hit, distance, layerMask);
	    if(collided){
	        seekingTarget.transform.position = hit.point + hit.normal * minDistance;
	        return seeking.GetSteering();
	    }else{
	        seekingTarget.transform.position = transform.position;
	        return SteeringOutput.empty;
	    }
	    
	}
	
	public override void SelfDestroy(){
	    seeking.SelfDestroy();
	    Destroy(seekingTarget);
	    base.SelfDestroy();
	}
}
