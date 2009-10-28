using UnityEngine;
using System.Collections;

public class AudioControl : MonoBehaviour {    
    GameMaster gameMaster;
    public AudioClip boostSound;
    public AudioClip scarySound;
    
    void Start(){        
        gameMaster = (GameMaster)GetComponent(typeof(GameMaster));
        gameMaster.isSurface.Subscribe(this.OnSurface);
        gameMaster.isGame.Subscribe(this.OnGame);   
        gameMaster.isOxygenLow.Subscribe(this.OnOxygenLow);        
        
        if(JukeBox.instance)
            JukeBox.AttachTo(this);
    }
    
    
    void Update(){
        
    }
    
    void setBoostButtonControl(HighlightableControlButton arg) {
        arg.AddPressedDelegate(new OnPressedDelegate(this.OnBoost));
    }


	void OnBoost(bool isBoost){
	    if(isBoost)
	        audio.PlayOneShot(boostSound);
	}
	
	void OnOxygenLow(object isLow){
	    SetOxygen();
	}
	
	void OnSurface(object isSurface){
	    SetBackgroundSound();
	}
	
	void OnGame(object isGame){
        SetBackgroundSound();        	    	    
        SetOxygen();
	}	
	
	void OnHunted(){
	    audio.PlayOneShot(scarySound);
	}
	
	void SetBackgroundSound(){
	    if(!JukeBox.instance)
	        return;
    
	    if(!gameMaster.isGame){
	        JukeBox.PlayMenu();	        
	    }else{
	        if(gameMaster.isSurface) 
	            JukeBox.PlaySurface();
	        else
	            JukeBox.PlayUnderwater();
	    }
	}
	
	void SetOxygen(){
	    if(gameMaster.isGame && gameMaster.isOxygenLow)
	        audio.Play();
	     else
    	    audio.Stop();  
	}	
}
