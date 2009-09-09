using UnityEngine;
using System.Collections;

public class FishPredatorBehaviour : FishArbitratedBehaviour, IHittable {
	
	private enum State{
		Calm,
		Pursue,
		Biting
	}
	
	public GameObject target;
	public float pursueSpeed = 12;
	public float pursueTime = 10;
	public float biteDistance = 0.2f;
	public float biteTime = 3;
	
	private State state = State.Calm;
	private float biteStartTime;
	private float pursueStartTime;
	
	private FishSeekingBehaviour seeking;
	
	void Start(){
		target = GameObject.FindWithTag("Player");
		
        seeking = (FishSeekingBehaviour)gameObject.AddComponent(typeof(FishSeekingBehaviour));
        seeking.target = target;
        seeking.maxSpeed = pursueSpeed;
	}

	 public void OnHit(Spear spear){  
		pursueStartTime = Time.time;
		state = State.Pursue;			
	}	
	
	public override SteeringOutput GetSteering(){
		if(!seeking || !target)
		    return new SteeringOutput();
		    
		ChangeState();
		    
		
		switch(state){
			case State.Calm:
				return new SteeringOutput();
			case State.Pursue:
			case State.Biting:
				return seeking.GetSteering();
			default:
				return SteeringOutput.empty;
		}
	}
	
	private void ChangeState(){
       	switch(state){
			case State.Pursue:
				if(Time.time - pursueStartTime > pursueTime)
					state = State.Calm;       			
			      		
		      	if(Vector3.Distance(transform.position, target.transform.position) < biteDistance){
		       	 	biteStartTime = Time.time;
		       	 	state = State.Biting;
		       	}
		       	break;
	     	case State.Biting:
		     	if(Time.time - biteStartTime > biteTime)
		     		state = State.Calm;      			
      			break;
		}
	}
}