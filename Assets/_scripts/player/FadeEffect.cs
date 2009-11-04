using UnityEngine;
using System.Collections;

public class FadeEffect : MonoBehaviour {
	public Texture2D texture;
	public float alpha = 0.0f;
	public float maxAlpha = 1.0f;
	public float minAlpha = 0.0f;
	public float speed = 1.0f;
	
	public float duration
	{
	    get{return 1.0f/speed;}
	}
	
	private float direct = -1.0f;
	
	void Awake() {
		useGUILayout = false;
	}
	
	void Start () {
		if(alpha > 0.0f) {
			direct = 1.0f;
		}
	}
	
	public void fadeIn() {
		direct = 1.0f;
	}
	
	public void fadeOut() {
		direct = -1.0f;
	}
	
	public void setFadeAlpha(float arg) {
		alpha = arg;
	}
	
	// Update is called once per frame
	void OnGUI () {
		if(texture) {
			alpha += speed * direct * Time.deltaTime; 
    		if(alpha < minAlpha) {
				alpha = minAlpha;
			} else if (alpha > maxAlpha) {
				alpha = maxAlpha;
			}
			
			alpha = Mathf.Clamp01(alpha);   
			GUI.color = new Color(1.0f, 1.0f, 1.0f, alpha);
    		if(!Utils.Approximately(alpha, 0.0f)){
    		    GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texture);    
    		}
		}
	}
}
