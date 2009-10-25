using UnityEngine;
using System.Collections;

public class Bubbles : MonoBehaviour {
    public ParticleEmitter bubbles;
    bool isBubblesEnabled = true;
    static bool isSubscribed = false;
    
    void Awake(){

        if(!isSubscribed){
          SubscribeToAnimation();
          isSubscribed = true;  
        } 
        
        isBubblesEnabled = PlayerPrefs.GetInt("graphicsLevel", 1) > 0;        
    }
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void StartBubbles(){
	    if(!isBubblesEnabled) return;
	    
	    print("emitting bubbles");
	    
	    bubbles.emit = true; 
        // bubbles.Emit(10);
	}
	
	void StopBubbles(){
	    bubbles.emit = false; 
	}
	
	void SubscribeToAnimation(){
	    AnimationEvent startBubbles = new AnimationEvent();
        startBubbles.time = 0.01f; 
        startBubbles.functionName= "StartBubbles";
        animation.clip.AddEvent(startBubbles);        

        AnimationEvent stopBubbles = new AnimationEvent();
        stopBubbles.time = 0.45f; 
        stopBubbles.functionName= "StopBubbles";
        animation.clip.AddEvent(stopBubbles);            	    
	}	
}
