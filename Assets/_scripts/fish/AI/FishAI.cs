using UnityEngine;
using System.Collections;
using System;

public class FishAI : MonoBehaviour, IHittable
{
    public float updatePeriod = 0.1f;
    private float lastUpdateTime = 0.0f;
    
	private bool isDead = false;
	private Vector3 originalScale;
	private float size = 0.0f;

    private ArrayList activeBehaviours;
	private FishBehaviour[] rootNonArbitratedBehaviours;
	private FishArbitratedBehaviour[][] rootArbitratedBehaviours;	
	
	private float deltaTime{
	    get{return Time.time - lastUpdateTime;}
	}
	
	void Awake(){
	    GatherRootBehaviours(); 
		// PrintBehaviours();
	}

	void Start(){
       originalScale = transform.localScale;
       if(!Utils.Approximately(size, 0.0f)) {
            updateScale();    
        }
	}
	   
	void FixedUpdate () {
       if(!isDead)
           TryExecuteBehaviours();
	}
	
	///// IHittable
	
	public virtual void OnHit(Spear spear){
	    spear.RealisticStickOn(gameObject);
	}
	
	///////////////
	
	public string BehavioursDescription(){
	    if(activeBehaviours == null)
	        return "";
	        
        string msg = "";        
        foreach(FishBehaviour beh in activeBehaviours){
            if(!beh)
                continue;
            if(beh.enabled)
                msg += beh.ToStringWithChildren() + "\n";            
        }
        
        return msg;	    
	}
    

	public void setSize(float sz){    
	    size = sz;
	    if(originalScale != Vector3.zero)
	        updateScale();    
	}

	private void updateScale(){
	    transform.localScale = originalScale * size;
	}

	public bool IsDead(){
	    return isDead;    
	}

	public void Die(){
		ArrayList behs = new ArrayList(GetComponents(typeof(FishBehaviour)));		
	    foreach(GenericScript elem in behs)
	        elem.SelfDestroy();
    
	    foreach(GenericScript elem in GetComponentsInChildren(typeof(FishAnimation)))
	        elem.SelfDestroy();
	        

	    foreach(Animation elem in GetComponentsInChildren(typeof(Animation)))
	        Destroy(elem);

	    rigidbody.useGravity = true;
	    rigidbody.drag = 10;
        
	    isDead = true;
	}
	
    // static SteeringOutput steering = SteeringOutput.empty;

	private void ExecuteRootBehaviours(){
	    Profiler.StartProfile(PT.ExecBehs);
	    
	    activeBehaviours = new ArrayList();
	    
        // execute non-arbitrated behaviours
        foreach(FishBehaviour beh in rootNonArbitratedBehaviours){
                 if(!beh.enabled)
                     continue;
         
         SteeringOutput steering = beh.GetSteering();
         if(steering.Significant())
             steering.ApplyTo(gameObject, deltaTime);             
             activeBehaviours.Add(beh);
        }
	
		// execute arbitrated behaviours
		bool isAlreadyApplied = false;

	    foreach(FishArbitratedBehaviour[] behs in rootArbitratedBehaviours){
	
			if(isAlreadyApplied)
				break;
	
			foreach(FishBehaviour beh in behs){
                if(!beh.enabled)
                    continue;
		
                SteeringOutput steering  =  beh.GetSteering();        
                
                if(!steering.Significant())
                    continue;
		
                steering.ApplyTo(gameObject, deltaTime);
                activeBehaviours.Add(beh);
				isAlreadyApplied = true;		
			}
	    }
	    
	    Profiler.EndProfile(PT.ExecBehs);    
	}
	
	private void TryExecuteBehaviours(){
	    if(Time.time - lastUpdateTime > updatePeriod){
	        ExecuteRootBehaviours();
	        lastUpdateTime = Time.time;
	    }
	}

	private void GatherRootBehaviours(){
	
		// separate arbitrated and non arbitrated behaviours
		ArrayList allRootBehaviours =  new ArrayList(GetComponents(typeof(FishBehaviour)));
		
		ArrayList tmpRootArbitratedBehaviours =  new ArrayList();
		ArrayList tmpRootNonArbitratedBehaviours =  new ArrayList();
		foreach(System.Object beh in allRootBehaviours){
			if(typeof(FishArbitratedBehaviour).IsInstanceOfType(beh))
				tmpRootArbitratedBehaviours.Add(beh);
			else
				tmpRootNonArbitratedBehaviours.Add(beh);
		}
		
		rootNonArbitratedBehaviours = (FishBehaviour[])tmpRootNonArbitratedBehaviours.ToArray(typeof(FishBehaviour));
		
		// group arbitrated behaviours by priorities
		rootArbitratedBehaviours = FishArbitratedBehaviour.GroupByPriorities(tmpRootArbitratedBehaviours);
		
        // PrintBehaviours();
	}
	
	private void PrintBehaviours(){
		print("========= non arbitrated behaviours =========");
		foreach(FishBehaviour beh in rootNonArbitratedBehaviours){
			print(beh);
		}
	
		print("========= arbitrated behaviours =========");
		foreach(FishArbitratedBehaviour[] behs in rootArbitratedBehaviours){
			print("--------");
			foreach(FishBehaviour beh in behs){
				print(beh);
			}
		}
	
	}
}