using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Nose))]
public class FishPredatorBehaviour : FishArbitratedBehaviour, IHittable {
    
    public float huntPeriod = 1f;
    
    public float maxHuntDistance = 10;
    public float restTime = 20f;
    public float huntingSpeed = 12f;

    private SteeringOutput steering;
    
    private Transform _transform;
    private static readonly int preysLayerMask;    
    
    private enum State
    {
        Tracking,
        Hunting,
        Rest
    }
    private State state;
    
    private FishHuntTargetBehaviour hunting;
    
    private Vector3 nose;
    
    static FishPredatorBehaviour(){
        preysLayerMask = 1 << LayerMask.NameToLayer("Preys");
    }
    
    FishPredatorBehaviour(){
        priority = 1;
    }
    
    protected override ArrayList children
    {
        get {ArrayList ret = base.children; if(state == State.Hunting)ret.Add(hunting); return ret; }
    }
    
    public override string ToString(){
        return base.ToString() + ": " + Enum.GetName(typeof(State), state);
    }
    
    void Awake(){
        nose = ((Nose)GetComponent(typeof(Nose))).position;
    }
    
    void Start(){
        _transform = transform;
        EnterTracking();
    }    
    
    void OnDrawGizmosSelected(){    
        if(!enabled)
            return;
        
        if(state == State.Tracking){
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, maxHuntDistance);        
        }else if(state == State.Hunting){
            if(hunting.target){
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.TransformPoint(nose), hunting.target.transform.position);                
            }
        }
    }

     public void OnHit(Spear spear){  
        ExitCurrentState();
        GameObject player = GameObject.FindWithTag("Player");
        EnterHunting(player);	     
	}
	
	public override SteeringOutput GetSteering(){
	    ChangeState();
	    ProcessState();
	    
	    return steering;
	}	
	
	private void ChangeState(){
	    if(state == State.Hunting){
    	    if(hunting.state == FishHuntTargetBehaviour.State.Calm){
    	        ExitCurrentState();
    	        if(hunting.succeed){        	            
    	            EnterRest();
    	        }else{
    	            EnterTracking();
    	        }    	        
    	    }
	    }
	}
	
	private void ProcessState(){
	    if(state == State.Hunting){
	        steering = hunting.GetSteering();	        
	    }else{
	        steering = SteeringOutput.empty;
	    }
	}
	
	private void EnterTracking(){
	   state = State.Tracking;
	   InvokeRepeating("HuntAttempt", 0, huntPeriod);
	}
	
	private void ExitTracking(){
	    CancelInvoke();
	}
	
	private void EnterHunting(GameObject prey){	    
	    hunting = (FishHuntTargetBehaviour)gameObject.AddComponent(typeof(FishHuntTargetBehaviour));
	    hunting.target = prey; 
	    hunting.pursueSpeed = huntingSpeed;
	    state = State.Hunting;	    
	}
		
	private void ExitHunting(){
        hunting.SelfDestroy();        
	}
	
	private void EnterRest(){
	    state = State.Rest;
	    Invoke("EnterTracking", restTime);
	}
	
	private void ExitRest(){
	    CancelInvoke();
	}
	
	private void ExitCurrentState(){
	    switch(state){
	        case State.Rest:
	            ExitRest();
	            break;
	        case State.Hunting:
	            ExitHunting();
	            break;
	        case State.Tracking:
	            ExitTracking();
	            break;
	    }
	}
	
	private void HuntAttempt(){
	    Collider[] potentialPreys = Physics.OverlapSphere(_transform.position, maxHuntDistance, preysLayerMask);	    
	    if(potentialPreys.Length > 0){
	        ExitCurrentState();
	        EnterHunting(potentialPreys[0].gameObject);
	    }	        
	}
	

}
