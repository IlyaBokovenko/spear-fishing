using UnityEngine;
using System.Collections;
using System;

public class FishAI : GenericScript, IHittable
{
    public float updatePeriod = 0.1f;
    private float lastUpdateTime = 0.0f;
    
	private bool _isDead = false;
	public bool isDead
	{
	    get{return _isDead;}
	}
	
	private Vector3 originalScale;
	private float size = 0.0f;

    private FishBehaviour[] rawActiveBehaviours;
    private int activeBehaviourCount;
	private FishBehaviour[] rootNonArbitratedBehaviours;
	private FishArbitratedBehaviour[][] rootArbitratedBehaviours;	
	
	private float deltaTime{
	    get{return Time.time - lastUpdateTime;}
	}
	
	private void AllocateActiveBehaviours(int num){
        // print("resetting behaviours to " + num);
	    rawActiveBehaviours = new FishBehaviour[num];
	    ResetActiveBehaviors();
	}
	
	private void ResetActiveBehaviors(){
	    activeBehaviourCount = 0;
	}
	
	private void AddActiveBehaviour(FishBehaviour beh){
        // print("behaviours: " + rawActiveBehaviours.Length + "; trying to push: " + activeBehaviourCount + 1);
	    rawActiveBehaviours[activeBehaviourCount++] = beh;
	}
	
	private IEnumerable ActiveBehaviours(){
	    for(int i = 0; i < activeBehaviourCount; i++){
	        yield return rawActiveBehaviours[i];
	    }
	}
	

	void Start(){
        lastUpdateTime = Time.time;
		GatherRootBehaviours(); 
        //PrintBehaviours();
		
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
        if(activeBehaviourCount == 0)
            return "";
            
        string msg = "";
        foreach(FishBehaviour beh in ActiveBehaviours()){
            if(!beh)
                continue;
            if(!beh.enabled)
                continue;
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

	public void Die(){
		ArrayList behs = new ArrayList(GetComponents(typeof(FishBehaviour)));		

	    foreach(GenericScript elem in behs)
	        elem.SelfDestroy();
		
		foreach(GenericScript elem in GetComponentsInChildren(typeof(FishAnimation)))
	        elem.SelfDestroy();
	    
		foreach(Animation elem in GetComponentsInChildren(typeof(Animation))) {
			if(elem.isPlaying) {
				elem.Stop();
			}
			elem.Play("Agony");
		}
		
		rigidbody.useGravity = true;
	    rigidbody.drag = 10;
        
	    _isDead = true;
	}
	
    // static SteeringOutput steering = SteeringOutput.empty;

	private void ExecuteRootBehaviours(){
	    Profiler.StartProfile(PT.ExecBehs);
	    
	    ResetActiveBehaviors();
	    
        // execute non-arbitrated behaviours
        foreach(FishBehaviour beh in rootNonArbitratedBehaviours){
                 if(!beh.enabled)
                     continue;
         
         SteeringOutput steering = beh.GetSteering();
         if(steering.Significant())
             steering.ApplyTo(gameObject, deltaTime);             
             AddActiveBehaviour(beh);
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
                AddActiveBehaviour(beh);
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

	    // find roots
	    ArrayList allBehaviours = new ArrayList(GetComponents(typeof(FishBehaviour)));
		ArrayList allRootBehaviours = (ArrayList)allBehaviours.Clone();
		AllocateActiveBehaviours(allBehaviours.Count);
		
		foreach(FishBehaviour beh in allBehaviours){
		    beh.RemoveChildrenFrom(allRootBehaviours);
		}
		
		// separate arbitrated and non arbitrated behaviours
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
	
	void OnDrawGizmosSelected(){
	    if(activeBehaviourCount == 0)
	        return;
	        
	    foreach(FishBehaviour beh in ActiveBehaviours()){
	        beh.DrawGizmosSelectedWithChildren();
	    }
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