using UnityEngine;
using System.Collections;

public class AudioControl : MonoBehaviour {    
    GameMaster gameMaster;
    public AudioClip boostSound;
    public AudioClip scarySound;
    
    void Awake(){        
        gameMaster = (GameMaster)GetComponent(typeof(GameMaster));
        gameMaster.AddSurfaceDelegate(new SurfaceDelegate(this.OnSurface));
        gameMaster.AddIsGameDelegate(new IsGameDelegate(this.OnGame));   
        gameMaster.AddOxygenLowDelegate(new OxygenLowDelegate(this.OnOxygenLow));        
        
        if(JukeBox.instance)
            JukeBox.AttachTo(this);
    }
    
    void setBoostButtonControl(HighlightableControlButton arg) {
        arg.AddPressedDelegate(new OnPressedDelegate(this.OnBoost));
    }


	void OnBoost(bool isBoost){
	    if(isBoost)
	        audio.PlayOneShot(boostSound);
	}
	
	void OnOxygenLow(bool isLow){
	    if(isLow) audio.Play();
	    else audio.Stop();
	}
	
	void OnSurface(bool isSurface){
	    SetClip();
	}
	
	void OnGame(bool isGame){
        SetClip();
	}	
	
	void OnHunted(){
	    audio.PlayOneShot(scarySound);
	}
	
	void SetClip(){
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
}
