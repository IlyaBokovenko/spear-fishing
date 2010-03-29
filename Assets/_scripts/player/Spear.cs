using UnityEngine;
using System.Collections;

public interface IHittable{
    void OnHit(Spear spear);
}

public class Spear : MonoBehaviour {
  	public GameObject ropeEnd;
	public GameObject ropeBegin;
	public HUD hud;
	public Speargun gun;
	
	public GameObject[] ignoreObjects;
	public enum States {
		RETURN,
		FLY,
		NONE
	}

	private GameObject fish = null;

	private float segmentDrag = 0.5f;
	private float segmentMass = 0.01f;
	private bool useGravity = false;
	
	private ArrayList segments;
	private GameObject lastSegment;
    private LineRenderer line;
	private Transform goTransform; 
	private float returnTime = 2.0f;
	private float timer = 0.0f;
	private Vector3 defaultPosition;
	private Quaternion defaultRotation;
	private Transform parent;
	private float distance = 0.1f;  
	private States state;
	private int segmentsCountPerTick = 3;
	
	void Start () {
		segments = new ArrayList();
		line = (LineRenderer)gameObject.GetComponent(typeof(LineRenderer));
		goTransform = transform;
		if(ropeEnd != null) {
			addSegment(ropeEnd);
		}
		state = States.NONE;
		foreach(GameObject element in ignoreObjects) {
			Physics.IgnoreCollision(element.collider, collider);
		}
		defaultPosition = goTransform.localPosition;
		defaultRotation = goTransform.localRotation;
		parent = goTransform.parent;
	}
	
	void Update() {
		switch(state) {
			case States.FLY:
				if(Vector3.Distance(ropeEnd.transform.position, goTransform.position) > 10.0f || segments.Count > 25) {
					ReturnSpear();
				} else {
					addSegment(createNewSegment(goTransform.position)); 
				}
				break;
			case States.RETURN :
				Vector3 returnVector = ropeEnd.transform.position - goTransform.position;
			    goTransform.position += returnVector * Time.deltaTime * 2.0f;
				goTransform.LookAt(ropeEnd.transform.position);
				if(fish != null) {
					fish.transform.position = goTransform.position;
				}
				if(returnVector.magnitude < 0.3f) {
				 	ReloadGun();
				}
				break;
		}
	}
	
	void LateUpdate () {
		switch(state) {
			case States.FLY :
				line.SetVertexCount(segments.Count);
				for(int index = 0 ; index < segments.Count; index++) {
					if(index == 0) {
						line.SetPosition(index, ropeEnd.transform.position);
					} else {
						if(index == segments.Count - 1) {
							line.SetPosition(index, ropeBegin.transform.position);   
						} else {
							GameObject segment = (GameObject)segments[index];
							line.SetPosition(index, segment.transform.position);
						}
					}
				}
				break;
			case States.RETURN :
				line.SetVertexCount(2);
				line.SetPosition(0, ropeEnd.transform.position);
				line.SetPosition(1, ropeBegin.transform.position);      
				break;
		}
	}
	
	private void ReloadGun() {
		goTransform.parent = parent;
		goTransform.localPosition = defaultPosition;
		goTransform.localRotation = defaultRotation;
		line.SetVertexCount(0);
		if(ropeEnd != null) {
			addSegment(ropeEnd);
		}
		if(fish != null) {
			float fishWeight = FishesInfo.getFishWeight(fish.name) * Mathf.Pow(fish.transform.localScale.x, 3);
			if(hud != null) {
				hud.addFish(fish.name + ":" + fishWeight);
			}
			((GenericScript)fish.GetComponent(typeof(FishAI))).DestroyGameObject();
			fish = null;
		}
		state = States.NONE;
		gun.Reload();
	}
	
	public void show(bool arg) {
		gameObject.SetActiveRecursively(arg);
	}
	
	private GameObject createNewSegment(Vector3 position) {
		GameObject segment = new GameObject("Segment_" + segments.Count);
		segment.transform.parent = transform;
		segment.transform.position = position;
		Rigidbody rigidBody = (Rigidbody)segment.AddComponent(typeof(Rigidbody));
		rigidBody.useGravity = useGravity;
		rigidBody.drag = segmentDrag;
		rigidBody.mass = segmentMass;
		segment.AddComponent("CharacterJoint");
		return segment;
	}
	
	private void addSegment(GameObject arg) {
		CharacterJoint joint = (CharacterJoint)arg.GetComponent(typeof(CharacterJoint));
		if(lastSegment != null) {
			CharacterJoint lastJoint = (CharacterJoint)lastSegment.GetComponent(typeof(CharacterJoint));
			lastJoint.connectedBody = arg.rigidbody;
		}
		if(segments.Count > 0) {      
			joint.connectedBody = rigidbody;
		}
		segments.Add(arg);
		lastSegment = arg;
	}
	
	public void Fire() {
		Vector3 vector = transform.TransformDirection(-Vector3.forward);
		state = States.FLY;
		rigidbody.useGravity = true; 
		rigidbody.isKinematic = false;
		rigidbody.AddForce(vector * 400.0f);
		transform.parent = null;
	}
	
	void ReturnSpear() {
		foreach(GameObject segment in segments) {
			if(segment != ropeEnd) {
				Destroy(segment);
			}
		}
		segments.Clear();
		rigidbody.isKinematic = true;
		state = States.RETURN;
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
	
	public void RealisticStickOn(GameObject other){
        CapsuleCollider capsuleCollider = (CapsuleCollider)other.GetComponent(typeof(CapsuleCollider));
		capsuleCollider.isTrigger = true;
		fish = other;
	}
	
	void OnCollisionEnter(Collision collision) {
		GameObject obj = collision.gameObject;
		if(obj.tag == "Fish") {
			CallHittable(obj);
		}
		ReturnSpear();
	}
}
