using UnityEngine;
using System.Collections;

public interface IBitable{
    void OnBite();
}

[RequireComponent(typeof(Nose))]
public class FishBiteBehaviour : FishBehaviour {    
	public float biteDistance = 0.3f;
	
	private Vector3 nose;
	
	private GameObject _target;
	private GameObject target
	{
	    get{return _target;}
	    set{_target = value;
	        targetTransform = _target.transform;
	        SetBitables(value);
	        targetAI = (FishAI)value.GetComponent(typeof(FishAI));}
	}
	private FishAI targetAI;
	private IBitable[] bitables;
	
	private Transform _transform;
	private Transform targetTransform;
	
	private bool _bited = false;
	public bool bited
	{
	    get{return _bited;}	    
	}
	
	public void StartBiting(GameObject obj){
	    target = obj;
	    enabled = true;	    
	}
	
	void OnEnable(){
	    _bited = false;
	}
	
	void Awake(){
	    nose = ((Nose)GetComponent(typeof(Nose))).position;	  
	    _transform = transform;  
	}	
	
	void Update(){
	    if( target == null || (targetAI != null && targetAI.isDead)){	            
                enabled = false;
                return;
        }
        
        float distanceToTarget = Vector3.Distance(_transform.TransformPoint(nose), targetTransform.position);			    
      	if(distanceToTarget < biteDistance){
      	    DoBite();
  	    }        
	}		    

	private void SetBitables(GameObject obj){	    
	    ArrayList _bitables = new ArrayList();
	    foreach(Component c in obj.GetComponents(typeof(Component))){
    	    if(c is IBitable)
    	        _bitables.Add(c);    	        
	    }	    
	    
	    bitables = (IBitable[])_bitables.ToArray(typeof(IBitable));
	}
	
	private void DoBite(){	
	    //AnimateAgony();
		_bited = true;    
	    foreach(IBitable b in bitables){
	        if(b != null)
	            b.OnBite();
	    }	        
	}	

	private void AnimateAgony() {
		foreach(Animation elem in GetComponentsInChildren(typeof(Animation))) {
			if(!elem.IsPlaying("Agony")) {
				if(elem.isPlaying) {
					elem.Stop();
				}
				elem.wrapMode = WrapMode.Once;
				elem.Play("Attack");
				//elem.PlayQueued("Swim");
				//elem.wrapMode = WrapMode.Loop;
			}
		}
	}
	private void AminateSwim() {
		foreach(Animation elem in GetComponentsInChildren(typeof(Animation))) {
			if(!elem.IsPlaying("Swim")) {
				if(elem.isPlaying) {
					elem.Stop();
				}
				elem.wrapMode = WrapMode.Loop;
				elem.Play("Swim");
			}
		}
	}
}
