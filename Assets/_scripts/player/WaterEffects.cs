using UnityEngine;
using System.Collections;

public class WaterEffects : MonoBehaviour {
	public float underwaterLevel = 0.0f;
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
	
	public static int SURFACE = 1;
	public static int UNDERWATER = 2;
	
	private int state = 0;

	void Awake() {
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
	
	void Update() {
		if(goTransform.position.y > underwaterLevel) {
			Surface();
		} else {
			Underwater();
		}
	}
	
	void Underwater() {
		if(state != UNDERWATER) {
			if(camera)
				camera.clearFlags = CameraClearFlags.SolidColor;
			RenderSettings.fog = true;
		    RenderSettings.fogColor = fogColor;
	    	RenderSettings.fogDensity = fogDensity;
			RenderSettings.skybox = null;
			camera.farClipPlane = farClipPlane;
			state = UNDERWATER;
		}
	}
	
	void Surface() {
		if(state != SURFACE) {	
			if(camera)
				camera.clearFlags = CameraClearFlags.Skybox;
			RenderSettings.fog = defaultFog;
		    RenderSettings.fogColor = defaultFogColor;
		    RenderSettings.fogDensity = defaultFogDensity;
		  	RenderSettings.skybox = skybox;
			camera.farClipPlane = defaultfarClipPlane;
			state = SURFACE;
		}
	}

	void setUnderwaterLevel(float arg) {
		underwaterLevel = arg;
	}
}

