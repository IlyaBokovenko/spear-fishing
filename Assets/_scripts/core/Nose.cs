using UnityEngine;
using System.Collections;

public class Nose : MonoBehaviour {    
    public Vector3 position = Vector3.zero;    
	
	void OnDrawGizmosSelected(){
	    if(!enabled)
	        return;
	        
	    Gizmos.color = Color.red;
	    Gizmos.DrawSphere(transform.TransformPoint(position), 0.01f);	    
    }
    	
}
