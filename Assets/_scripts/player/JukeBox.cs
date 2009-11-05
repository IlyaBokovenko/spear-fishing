using UnityEngine;
using System.Collections;

public class JukeBox : MonoBehaviour {    
    
    public AudioClip menu;
    public AudioClip underwater;
    public AudioClip surface;
    public AudioClip menuTap;
    
    private static JukeBox _instance;
    public static JukeBox instance
    {
        get{return _instance; }
    }

    void Awake(){
        if(!_instance){
            GameObject.DontDestroyOnLoad(gameObject);    
            _instance = this;
            AudioListener.volume = PlayerPrefs.GetFloat("sound", 0.5f);
        }else{
            Destroy(gameObject);
        }
        
    }
    
    void OnDisable(){
        transform.parent = null;
    }
    
    public static void AttachTo(MonoBehaviour comp){
        instance.transform.parent = comp.transform;
        instance.transform.localPosition = Vector3.zero;
    }    
    
    public static void PlayMenu(){instance.Play(instance.menu);}
    public static void PlayUnderwater(){instance.Play(instance.underwater);}
    public static void PlaySurface(){instance.Play(instance.surface);}
    public static void Tap(){instance._Tap();}
    
    void _Tap(){     
        print("tap");   
        audio.PlayOneShot(menuTap);
    }
    
    void Play(AudioClip clip){
        if(audio.clip == clip) return;
        audio.clip = clip;
        audio.loop = true;
        audio.Play();        
       }    
       

}
