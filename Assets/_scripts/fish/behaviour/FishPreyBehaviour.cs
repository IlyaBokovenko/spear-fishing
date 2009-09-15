using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Nose))]
public class FishPreyBehaviour : FishArbitratedBehaviour {
    
    public float escapeAttemptPeriod = 1f;
    public float safetyDistance  = 3;
    public float fleeSpeed  = 10;    
    public float panicTime  = 4;    
    
    private Transform _transform;
    private static readonly int predatorsLayerMask;    
    
    private FishEscapeTargetBehaviour escape;
    
    private Vector3 nose;
    
    enum State{
        Escaping,
        Calm
    } 
    
    private State state
    {
        get{return escape ? State.Escaping : State.Calm;}
    }
    
    public override string ToString(){
        return base.ToString() + ": " + Enum.GetName(typeof(State), state);
    }    
    
    static FishPreyBehaviour(){
        predatorsLayerMask = 1 << LayerMask.NameToLayer("Predators");
    }
    
    FishPreyBehaviour(){
        priority = 1;
    }
    
    protected override ArrayList children
    {
        get {ArrayList ret = base.children; if(state == State.Escaping)ret.Add(escape); return ret; }
    }
    
    void Awake(){
        nose = ((Nose)GetComponent(typeof(Nose))).position;
    }
    
    void Start(){
        _transform = transform;
        InvokeRepeating("EscapeAttempt", 0, escapeAttemptPeriod);
    }    
    
    void OnDrawGizmosSelected(){    
        if(!enabled)
            return;
        
        if(state == State.Calm){
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, safetyDistance);        
        }else if(state == State.Escaping){
            if(escape.target){
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, escape.target.transform.position);                
            }
        }
    }
	
	public override SteeringOutput GetSteering(){
	    if(state == State.Calm)
	        return SteeringOutput.empty;	        
	    
        if(!escape.isEscaping){
            escape.SelfDestroy();	            
            InvokeRepeating("EscapeAttempt", 0, escapeAttemptPeriod);
        }
        
        return escape.GetSteering();
	}		
	
	private void EscapeAttempt(){
	    Collider[] potentialPredators = Physics.OverlapSphere(_transform.position, safetyDistance, predatorsLayerMask);	    
	    if(potentialPredators.Length > 0){
	        CancelInvoke();
	        escape = (FishEscapeTargetBehaviour)gameObject.AddComponent(typeof(FishEscapeTargetBehaviour));
	        escape.target = potentialPredators[0].gameObject;
	        escape.fleeSpeed = fleeSpeed;
	        escape.panicTime = panicTime;
	    }	        
	}
	

}
