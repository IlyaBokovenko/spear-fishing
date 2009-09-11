using UnityEngine;
using System.Collections;
using System;

public interface IBitable{
    void OnBite(FishHuntingTargetBehaviour hunter);
}

[RequireComponent(typeof(Nose))]
public class FishHuntingTargetBehaviour : FishBehaviour {
	
	public enum State{		
		Pursue,
		Biting,
		Calm
	}
	
	private GameObject _target;
	private IBitable[] bitables;
	public GameObject target
	{
	    get{return seeking ? seeking.target : _target;}
	    set{if(seeking)
	            seeking.target = value;	            
	        else 
	            _target = value;
	        SetBitable(value);
	    }
	}	
	
	public float pursueSpeed = 12;
	public float pursueTime = 10f;
	public float biteDistance = 0.3f;
	public float biteTime = 3f;
	
	public State state = State.Pursue;
	private float biteStartTime;
	private float pursueStartTime;
	
	private bool _succeed = false;
	public bool succeed
	{
	    get{return _succeed;}
	}
	
	private FishSeekingBehaviour seeking;
	
	private Vector3 nose;
	
	public override string ToString(){
	    return base.ToString() + ": " + Enum.GetName(typeof(State), state);
	}	
	
	void Awake(){
	    nose = ((Nose)GetComponent(typeof(Nose))).position;	    
	}
	
	void Start(){	    	    
	    seeking = (FishSeekingBehaviour)gameObject.AddComponent(typeof(FishSeekingBehaviour));		
	    this.target = _target;	    
		
		pursueStartTime = Time.time;	
        seeking.maxSpeed = pursueSpeed;
	}

	public override SteeringOutput GetSteering(){
		if(!seeking || !target)
		    return SteeringOutput.empty;
		    
		ChangeState();
		return ProcessState();		
	}
	
	private void SetBitable(GameObject obj){	    
	    ArrayList _bitables = new ArrayList();
	    foreach(Component c in obj.GetComponents(typeof(Component))){
    	    if(c is IBitable)
    	        _bitables.Add(c);    	        
	    }	    
	    
	    bitables = (IBitable[])_bitables.ToArray(typeof(IBitable));
	}
	
	private void NotifyBitables(){	    
	    foreach(IBitable b in bitables)
	        b.OnBite(this);
	}
	
	private void ChangeState(){
       	switch(state){
			case State.Pursue:
				if(Time.time - pursueStartTime > pursueTime)
					state = State.Calm;       			
			      
			    float distanceToTarget = Vector3.Distance(transform.TransformPoint(nose), target.transform.position);			    
		      	if(distanceToTarget < biteDistance){
		       	 	biteStartTime = Time.time;
		       	 	state = State.Biting;
		       	}
		       	break;
	     	case State.Biting:	     	        		     	    
		     	if(Time.time - biteStartTime > biteTime){
                    NotifyBitables();
		     	    _succeed = true;    			
		     		state = State.Calm;	     	    
		     	}
		     	    
      			break;
		}
	}
	
	private SteeringOutput ProcessState(){
	    switch(state){
			case State.Calm:
				return SteeringOutput.empty;
			case State.Pursue:
			case State.Biting:
				return seeking.GetSteering();
			default:
				return SteeringOutput.empty;
		}		
	}
}