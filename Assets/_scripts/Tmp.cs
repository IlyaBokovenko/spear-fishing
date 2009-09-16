using UnityEngine;
using System.Collections;

public class Tmp : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if(Time.time > 1){
	        Debug.Break();
	        print(transform.position.z);
	    }
	        
	}
}
