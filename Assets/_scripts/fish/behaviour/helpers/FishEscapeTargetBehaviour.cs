using UnityEngine;
using System.Collections;

public class FishEscapeTargetBehaviour : FishArbitratedBehaviour
{    	
    public FishFleeBehaviour flee;
    
    public GameObject target
    {
        get{return flee.target;}
        set{flee.target = value;}
    }    

    public float panicTime  = 4;    
    private float startEscapingTime;    

    protected override ArrayList ActiveChildren(){
        if(enabled)
            return base.ActiveChildren();
        else
            return new ArrayList();
    }	
    
    public override string ToString(){
	    string ret = base.ToString();
	    if(!enabled)
	        ret += " (calm)";
        return ret;
	}	
	

    FishEscapeTargetBehaviour(){
        priority = 1;
    }    

    void Awake(){
        children = new FishBehaviour[1]{flee};        
    }
    
    public void StartEscapingFrom(GameObject obj){
        target = obj;
        enabled = true;
    }
    
    void OnEnable(){
         startEscapingTime = Time.time;       
    }    

    public override SteeringOutput GetSteering () {        
        Profiler.StartProfile(PT.Escape);        
        SteeringOutput ret = _GetSteering();        
        Profiler.EndProfile(PT.Escape);        
        return ret;
    }
    
    private SteeringOutput _GetSteering(){
        if(!flee)
            return SteeringOutput.empty;
        if(!target)
            return SteeringOutput.empty;

        if(Time.time - startEscapingTime > panicTime)
            enabled = false;
            
        if(enabled)
            return flee.GetSteering();
        else
            return SteeringOutput.empty;
    }
}
