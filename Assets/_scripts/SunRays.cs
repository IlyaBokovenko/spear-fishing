using UnityEngine;
using System.Collections;

public class SunRays : MonoBehaviour {
    
    public WaterEffects waterEffects;
    bool sunRaysEnabled = true;
    
    void Start(){
        int graphicsLevel = PlayerPrefs.GetInt("graphicsLevel", 1);
        sunRaysEnabled = graphicsLevel > 0;
        gameObject.SetActiveRecursively(sunRaysEnabled);
        
        waterEffects.AddSurfaceDelegate(new SurfaceDelegate(this.OnSurface));
    }

    void OnSurface(bool isSurface){
        if(!sunRaysEnabled) return;
        gameObject.SetActiveRecursively(!isSurface);    
    }
    
}
