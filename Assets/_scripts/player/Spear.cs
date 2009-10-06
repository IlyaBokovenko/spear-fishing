using UnityEngine;
using System.Collections;

public interface IHittable{
    void OnHit(Spear spear);
}

public class Spear : MonoBehaviour {
	private GameObject fish = null;
	private GameObject gameMaster = null;
	private bool isActive = false;
	
	void Start() {
		gameMaster = GameObject.Find("GameMaster");
	}
	
	void OnTriggerEnter(Collider other) {
    	GameObject obj = other.gameObject;
    	
		if(obj.tag == "Fish" && isActive) {
			CallHittable(obj);
		}
		if(obj.tag == "Player" && fish != null) {
			obj.SendMessage("addFish", fish.name + ":" + fish.transform.localScale.x);
			((GenericScript)fish.GetComponent(typeof(FishAI))).DestroyGameObject();
		}
	}
	
	void OnTriggerExit(Collider other) {
		GameObject obj = other.gameObject;
		if(obj.tag == "Player") {
			isActive = true;
		}
	}
	
	void OnTriggerStay(Collider other) {
		GameObject obj = other.gameObject;
		if(obj.tag == "Player") {
			isActive = false;
		}
	}
	
	public void RealisticStickOn(GameObject other){
        FixedJoint joint = (FixedJoint)other.AddComponent(typeof(FixedJoint));
        joint.connectedBody = rigidbody;     	    
    	fish = other;
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
	
	public void DecorativeStickOn(GameObject other){
	    //TemporaryStickOn(other);
	    //neverDie = true;
	}
}
