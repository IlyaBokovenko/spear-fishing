using UnityEngine;
using System.Collections;

public class FishEscapeTargetBehaviour : FishArbitratedBehaviour
{    	
    private GameObject _target;
    public GameObject target
    {
        get{return seeking ? seeking.target : _target;}
        set{if(seeking) seeking.target = value; else _target = value;}
    }
    
    public float fleeSpeed  = 10;    
    public float panicTime  = 4;    

    public bool isEscaping  = true;
    private float startEscapingTime;    

    private FishSeekingBehaviour seeking;
    
    protected override ArrayList children
    {
        get {ArrayList ret = base.children; ret.Add(seeking); return ret; }
    }	
    
    public override string ToString(){
	    string ret = base.ToString();
	    if(!isEscaping)
	        ret += " (calm)";
        return ret;
	}	
	

    FishEscapeTargetBehaviour(){
        priority = 1;
    }

    void Start(){
        startEscapingTime = Time.time;
        seeking = (FishSeekingBehaviour)gameObject.AddComponent(typeof(FishSeekingBehaviour));
        seeking.target = _target;
        seeking.isFlee = true;
        seeking.maxSpeed = fleeSpeed;
    }

    public override void SelfDestroy(){
    	if(seeking)
        	seeking.SelfDestroy();

        base.SelfDestroy();
    }

    public override SteeringOutput GetSteering () {        
        Profiler.StartProfile(PT.Escape);
        
        SteeringOutput ret;

        if(!seeking || !target)
            ret = SteeringOutput.empty;
        else{
            if(Time.time - startEscapingTime > panicTime){
                isEscaping = false;
            }    

            if(isEscaping)
            {
                ret = seeking.GetSteering();
            }else{
                ret = SteeringOutput.empty;
            }            
        }
        
        Profiler.EndProfile(PT.Escape);
        
        return ret;
    }   
}
