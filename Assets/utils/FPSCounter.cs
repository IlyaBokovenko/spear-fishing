using UnityEngine;
using System.Collections;

public class FPSCounter : MonoBehaviour {
	public Rect fpsCounterRect = new Rect(1,1,64,24);
	public float updateTime = 0.25f;
	public bool isShow = true;
	private float currentTime = 0.0f;
	private float accFps = 0.0f;
	private int fps = 0;
	private int count = 0;
	
	void OnGUI () {
        if(currentTime < updateTime) {
                 currentTime += Time.deltaTime;
                 count ++;
                 accFps += 1.0f/Time.deltaTime;
        } else {
                 fps = (int)(accFps/count);
                 accFps = 0.0f;
                 count = 0;
                 currentTime = 0.0f;
        }
                
        if(isShow) {
           GUI.Box(fpsCounterRect, "FPS : " + fps.ToString());
        }
        
        //GUI.Box(fpsCounterRect, "FPS : " + 1/Time.deltaTime);
	}
	
	int getFPS() {
		return fps;
	}
}
