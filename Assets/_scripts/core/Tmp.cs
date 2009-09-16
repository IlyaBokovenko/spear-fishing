using UnityEngine;
using System.Collections;

public class Tmp : MonoBehaviour {

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	    print(  Vector3.Angle(transform.forward, Vector3.forward));
	}
}
