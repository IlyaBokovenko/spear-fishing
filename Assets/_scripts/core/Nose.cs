using UnityEngine;
using System.Collections;

public class Nose : MonoBehaviour {    
    public Vector3 position = Vector3.zero;    
    
    private Transform _transform;
    
    void Start(){
        _transform = transform;
    }
	
	void OnDrawGizmosSelected(){
	    if(!enabled)
	        return;
	        
	    Gizmos.color = Color.red;
	    Gizmos.DrawSphere(_transform.TransformPoint(position), 0.01f);	    
    }
    	
}
