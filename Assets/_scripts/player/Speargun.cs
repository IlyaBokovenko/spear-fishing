using UnityEngine;
using System.Collections;

public class Speargun : MonoBehaviour {
    public ParticleEmitter bubbles;
    bool isBubblesEnabled = true;
    static bool isSubscribed = false;
    
    public AudioClip fireSound;
    public AudioClip bubblesSound;
    public Spear spear;
    
	private bool ready;

    void Awake(){
        if(!isSubscribed || Application.platform != RuntimePlatform.OSXEditor){
          SubscribeToAnimation();
          isSubscribed = true;  
        } 
        ready = true;
        isBubblesEnabled = PlayerPrefs.GetInt("graphicsLevel", 1) > 0;        
    }
	
	public void Fire(){
	    audio.PlayOneShot(fireSound);
        audio.PlayOneShot(bubblesSound);
	    animation.Play("Shot");
		if(spear != null) {
			spear.Fire();
		}
		ready = false;
	}
	
	public bool isReady {
		get {return ready && !animation.isPlaying;}
	} 
	
	public void Reload() {
		animation.Play("Reload");
		ready = true;
	}
	
	public void StartBubbles(){
	    if(!isBubblesEnabled) return;	    
	    bubbles.emit = true; 
        // bubbles.Emit(10);
	}
	
	public void StopBubbles(){
	    bubbles.emit = false; 
	}
	
	void SubscribeToAnimation(){
	    AnimationEvent startBubbles = new AnimationEvent();
        startBubbles.time = 0.01f; 
        startBubbles.functionName= "StartBubbles";
        animation["Shot"].clip.AddEvent(startBubbles);        

        AnimationEvent stopBubbles = new AnimationEvent();
        stopBubbles.time = 0.45f; 
        stopBubbles.functionName= "StopBubbles";
        animation["Shot"].clip.AddEvent(stopBubbles);            	    
		
		AnimationEvent stopBubblesNow = new AnimationEvent();
        stopBubblesNow.time = 0.01f; 
        stopBubblesNow.functionName= "StopBubbles";
		animation["Reload"].clip.AddEvent(stopBubblesNow);            	    
	}	
}
