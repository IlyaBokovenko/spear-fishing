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
	        targetCollider = _target.collider;
	        SetBitables(value);
	        targetAI = (FishAI)value.GetComponent(typeof(FishAI));}
	}
	private FishAI targetAI;
	private IBitable[] bitables;
	
	private Transform _transform;
	private Transform targetTransform;
	private Collider targetCollider;
	private Animation _anim;
	
	private bool readyToBite = true;
	private bool _bited = false;
	public bool bited
	{
	    get{return _bited;}	    
	}
	
	public override string ToString(){
        string ret = base.ToString();
        if(!TargetIsDead())
            ret += " (distance to target: " + DistanceToTarget() + "); coll: " + targetCollider;
        return ret;
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
	
	void Start(){
	    _anim = (Animation)GetComponentsInChildren(typeof(Animation))[0];
	}
	
	void Update(){
	    if( TargetIsDead() ){
                enabled = false;
                return;
        }
        
      	if(DistanceToTarget() < biteDistance && readyToBite){
      	    StartCoroutine("DoBite");
  	    }        
	}
	
	float DistanceToTarget(){	    
	    Vector3 globalNose = _transform.TransformPoint(nose);
	    if(targetCollider != null){
	        Vector3 closestPoint = targetCollider.ClosestPointOnBounds(globalNose);
	        return Vector3.Distance(globalNose, closestPoint);
	    }else{	        
	        return Vector3.Distance(globalNose, targetTransform.position);			    
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
	
	bool TargetIsDead(){
	    return target == null || (targetAI != null && targetAI.isDead);
	}	
	
	IEnumerator DoBite(){
	    readyToBite = false;
	    
	    AnimationState attack = _anim["Attack"];
	    attack.speed = 0.5f;    
	    _anim.Blend("Attack");	
        yield return new WaitForSeconds(attack.length / attack.speed);
        _anim.Stop("Attack");
	    foreach(IBitable b in bitables){
	        if(b != null)
	            b.OnBite();
	    }	        
        _bited = true;
        readyToBite = true;	    
	}
}
