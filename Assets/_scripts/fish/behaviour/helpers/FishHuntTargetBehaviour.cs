using UnityEngine;
using System.Collections;
using System;

public class FishHuntTargetBehaviour : FishBehaviour {
    
    public GenericSeekingBehaviour seeking;
    
	public GameObject target
	{
	    get{return seeking.target;}
	    set{seeking.target = value;}
	}
	
	public float pursueTime = 10f;
	private float pursueStartTime;	

	protected override ArrayList ActiveChildren(){
	    if(enabled)
	        return base.ActiveChildren();
	    else
	        return new ArrayList();
	}
	
	void Awake(){
	    children = new FishBehaviour[1]{seeking};	
	}
	
	public void StartHunting(GameObject obj){
	    target = obj;
	    enabled = true;
	}
	
	void OnEnable(){
	    pursueStartTime = Time.time;	    
	}

	public override SteeringOutput GetSteering(){
		if(!seeking || !target || !enabled)
		    return SteeringOutput.empty;
		    
	    if(Time.time - pursueStartTime > pursueTime){	        
	        enabled = false;
	        return SteeringOutput.empty;   
	    }
	    
	    return seeking.GetSteering();		    
	}	
}