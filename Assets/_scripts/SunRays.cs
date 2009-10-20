using UnityEngine;
using System.Collections;

public class SunRays : MonoBehaviour {
    
    public WaterEffects waterEffects;
    
    void Start(){
        waterEffects.AddSurfaceDelegate(new SurfaceDelegate(this.OnSurface));
    }

    void OnSurface(bool isSurface){
        gameObject.SetActiveRecursively(!isSurface);    
    }
    
}
