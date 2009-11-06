using UnityEngine;
using System.Collections;

public class PointToHealth : MonoBehaviour {
    Transform _transform;
    Transform _heartTransform;
    GameObject heart;

	void Start () {
	    _transform = transform;	    
	    _heartTransform = ClosestHeart().transform;	
	}
	
	void FixedUpdate () {	    
	    _transform.LookAt(_heartTransform);
	}
	
	
	GameObject ClosestHeart(){
	    GameObject[] hearts = GameObject.FindGameObjectsWithTag("Health");
	    
	    float distance = Mathf.Infinity;
	    GameObject closestHeart = null;
	    foreach(GameObject heart in hearts){
	        if(!heart.active) continue;
	        
	        float newDistance = Vector3.Distance(_transform.position, heart.transform.position);
	        if(newDistance < distance){
	            distance = newDistance;
	            closestHeart = heart;
	        }	            
	    }
	    
	    return closestHeart;
	}
}
