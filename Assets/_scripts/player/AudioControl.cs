using UnityEngine;
using System.Collections;

public class AudioControl : MonoBehaviour {
    
    public AudioClip underwater;
    public AudioClip surface;
    public AudioClip menu;
    
    AudioSource _audio;
    WaterEffects waterEffects;
    
    
    
    StateMachine sm;
    
    void Awake(){        
        _audio = audio;
        waterEffects = (WaterEffects)GetComponent(typeof(WaterEffects));
        sm = new StateMachine();
        
    }

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	    
	}
}
