using UnityEngine;
using System.Collections;
using System;

public class FishPreyBehaviour : FishArbitratedBehaviour {
    public FishEscapeTargetBehaviour escape;
    
    public float escapeAttemptPeriod = 1f;
    public float safetyDistance  = 3;
    
    private Transform _transform;
    public LayerMask escapeFrom;
    private int escapeeLayerMask;        
    
    enum State{
        Escaping,
        Calm
    } 
    
    private State state = State.Calm;
    
    public override string ToString(){
        return base.ToString() + ": " + Enum.GetName(typeof(State), state);
    }    
    
    FishPreyBehaviour(){
        priority = 1;
    }
    
    protected override ArrayList ActiveChildren(){
        if(state != State.Escaping)
            return new ArrayList();
            
        return base.ActiveChildren();
    }
    
    void Awake(){
        escapeeLayerMask = escapeFrom.value;
        children = new FishBehaviour[1] {escape};
    }
    
    void Start(){        
        _transform = transform;
        EnterCalm();
    }    
    
	void OnDrawGizmosSelected(){	           
        if(state == State.Calm){
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, safetyDistance);        
        }
    }
	
	public override SteeringOutput GetSteering(){
	    ChangeState();

	    if(state == State.Calm)
	        return SteeringOutput.empty;
	    else
            return escape.GetSteering();
	}		
	
	private void EscapeAttempt(){
	    Collider[] potentialPredators = Physics.OverlapSphere(_transform.position, safetyDistance, escapeeLayerMask);	    
	    if(potentialPredators.Length > 0){
	        ExitCalm();
	        EnterEscape(potentialPredators[0].gameObject);
	    }	        
	}
	
/////////////// state machine ///////////////	
	void ChangeState(){
	    if(state == State.Escaping){
	        if(!escape.enabled)
	            EnterCalm();
	    }
	}
	
	private void EnterEscape(GameObject obj){
	    state = State.Escaping;
	    escape.StartEscapingFrom(obj);
	}
	
	private void EnterCalm(){
	    state = State.Calm;
        InvokeRepeating("EscapeAttempt", 0, escapeAttemptPeriod);        
	}
	
	private void ExitCalm(){
	    CancelInvoke();
	}
}
