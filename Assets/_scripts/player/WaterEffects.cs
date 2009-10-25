using UnityEngine;
using System.Collections;

public class WaterEffects : MonoBehaviour {
	public Color fogColor = new Color(0.0f, 0.4f, 0.7f, 0.6f);
	public float fogDensity = 0.17f;
	public float farClipPlane = 10;
	public Material skybox;
	public bool lowLight = false;
	public Color ambientLight;
	
	//Default
	private bool defaultFog = RenderSettings.fog;
	private Color defaultFogColor = RenderSettings.fogColor;
	private float defaultFogDensity = RenderSettings.fogDensity;
	private Transform goTransform;
	private float defaultfarClipPlane;
	
	private GameMaster gameMaster;
	
    void Awake() {
	    gameMaster = (GameMaster)GetComponent(typeof(GameMaster));
	    gameMaster.AddSurfaceDelegate(new SurfaceDelegate(this.OnSurface));
		goTransform = transform;
		if(lowLight) {
			GameObject light = GameObject.Find("Light");
			if(light)
				light.active = false;
			RenderSettings.ambientLight = ambientLight;
		}
	}
	
	void Start() {
		defaultfarClipPlane = camera.farClipPlane;
	}	
	
	void OnSurface(bool isSurface){
		if(camera)
			camera.clearFlags = isSurface ? CameraClearFlags.Skybox : CameraClearFlags.SolidColor;
		RenderSettings.fog = isSurface ? defaultFog : true;
	    RenderSettings.fogColor = isSurface ? defaultFogColor : fogColor;
	    RenderSettings.fogDensity = isSurface ? defaultFogDensity : fogDensity;
	  	RenderSettings.skybox = isSurface ? skybox : null;
		camera.farClipPlane = isSurface ? defaultfarClipPlane : farClipPlane;
	}
}

