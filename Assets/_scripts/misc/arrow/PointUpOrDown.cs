using UnityEngine;
using System.Collections;

public class PointUpOrDown : MonoBehaviour {
    bool pointUp = true;
    Transform _transform;

	// Use this for initialization
	void Start () {
	    _transform = transform;
	}
	
	// Update is called once per frame
	void Update () {
	    UpdateDirection();
	}
	
	public void PointDown(){
	    pointUp = false;
	}
	
	void UpdateDirection(){
	    _transform.forward = pointUp ? Vector3.up : Vector3.down;
	}
}
