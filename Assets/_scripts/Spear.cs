using UnityEngine;
using System.Collections;

public interface IHittable{
    void OnHit(Spear spear);
}

public class Spear : MonoBehaviour {
	public float lifetime = 10;
	
	private bool isFlying = true;
	private float flyStartTime;
	private bool neverDie = false;
	
	private Vector3 prevPosition;
	private Quaternion prevRotation;
	
	void Update () {
	    prevPosition = transform.position;
	    prevRotation = transform.rotation;
	    
	    if(!IsDead() && IsTimeToDie())
	        Die();
	    
	    if(isFlying)
	        CorrectHeadingDuringFly();
	}
	
    void OnCollisionEnter(Collision collision){
        if(isFlying)
            StickOnCollision(collision);    
    }
	
	//////////////////////////////////////////////////////////////////////////////
	public void FireWithForce(float force){
	    isFlying = true;
	    flyStartTime = Time.time;
	    rigidbody.AddForce(transform.forward * force);         
	}
	

	public void TemporaryStickOn(GameObject other){	
	    Destroy(rigidbody);
	    Destroy(collider);	    
	    
	    transform.parent = other.transform;	    
	    Vector3 parentScaleReversed = Utils.PowComponents(transform.parent.lossyScale, -1);
	    transform.localScale = Vector3.Scale(transform.localScale, parentScaleReversed);
    }

	public void DecorativeStickOn(GameObject other){
	    TemporaryStickOn(other);
	    neverDie = true;
	}	
    	
	public void RealisticStickOn(GameObject other){
        FixedJoint joint = (FixedJoint)other.AddComponent(typeof(FixedJoint));
        joint.connectedBody = rigidbody;     	    
        neverDie = true;
	}
	
	bool IsTimeToDie(){
	    return !neverDie && Time.time - flyStartTime > lifetime;
	}
	
	bool IsDead(){
	    return !gameObject.active;
	}
	
	void Die(){
	    gameObject.active = false;
	    Destroy(gameObject);    
	}	
	
    void StickOnCollision(Collision collision){        
        transform.position = prevPosition;
        transform.rotation = prevRotation;
        
        isFlying = false;        
        rigidbody.detectCollisions = false;        

        if(!CallHittable(collision.gameObject))
            TemporaryStickOn(collision.gameObject);
    }
	
	void CorrectHeadingDuringFly(){
	    Vector3 speed = rigidbody.velocity;
	    if(speed.magnitude > 1){
	        transform.LookAt(transform.position + speed);
	    }    
	}
	
	bool CallHittable(GameObject obj){
	    bool called = false;
	    foreach(Component c in obj.GetComponents(typeof(Component))){
	        if(c is IHittable){	            
	            ((IHittable)c).OnHit(this);
	            called = true;
	        }	            
	    }
	    
	    return called;
	}
}