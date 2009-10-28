using UnityEngine;
using System.Collections;

public class SunRays : MonoBehaviour {
    
    public GameMaster gameMaster;
    bool sunRaysEnabled = true;
    
    void Start(){
        int graphicsLevel = PlayerPrefs.GetInt("graphicsLevel", 1);
        sunRaysEnabled = graphicsLevel > 0;
        gameObject.SetActiveRecursively(sunRaysEnabled);
        
        gameMaster.isSurface.Subscribe(this.OnSurface);
    }

    void OnSurface(object isSurface){
        if(!sunRaysEnabled) return;
        gameObject.SetActiveRecursively(!(bool)isSurface);    
    }
    
}
