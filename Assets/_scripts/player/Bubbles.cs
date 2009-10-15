using UnityEngine;
using System.Collections;

public class Bubbles : MonoBehaviour {
    public ParticleEmitter bubbles;

	void Start () {
	    AnimationEvent startBubbles = new AnimationEvent();
        startBubbles.time = 0.01f; 
        startBubbles.functionName= "StartBubbles";
        animation.clip.AddEvent(startBubbles);        
        
        AnimationEvent stopBubbles = new AnimationEvent();
        stopBubbles.time = 0.45f; 
        stopBubbles.functionName= "StopBubbles";
        animation.clip.AddEvent(stopBubbles);        
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void StartBubbles(){	    
	    bubbles.emit = true; 
        // bubbles.Emit(10);
	}
	
	void StopBubbles(){
	    bubbles.emit = false; 
	}
	
}
