using UnityEngine;
using System.Collections;

public class FishEscapeBehaviour : FishArbitratedBehaviour
{    	
    GameObject target;
    public float fleeSpeed  = 10;    
    public float safetyDistance  = 3;
    public float panicTime  = 4;    

    private bool isEscaping  = false;
    private float startEscapingTime;    

    private FishSeekingBehaviour seeking;

    FishEscapeBehaviour(){
        priority = 1;
    }

    void Start(){
        target = GameObject.FindWithTag("Player");

        seeking = (FishSeekingBehaviour)gameObject.AddComponent(typeof(FishSeekingBehaviour));
        seeking.target = target;
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
            if(Vector3.Distance(transform.position, target.transform.position) < safetyDistance ){
                if(!isEscaping){
                    startEscapingTime = Time.time;
                }
                isEscaping = true;
            }

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
