using UnityEngine;
using System.Collections;

public class PointToPoint : MonoBehaviour {
    bool pointLeft = false;
    Vector3 vector;
	Transform _transform;

	// Use this for initialization
	void Start () {
	    _transform = transform;
	}
	
	// Update is called once per frame
	void Update () {
	    UpdateDirection();
	}
	
	public void setVector(Vector3 arg){
	    vector = arg;
	}
	
	void UpdateDirection(){
	    _transform.forward = vector;
	}
}
