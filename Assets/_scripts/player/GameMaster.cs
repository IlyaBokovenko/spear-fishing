using UnityEngine;
using System.Collections;

public class GameMaster : MonoBehaviour {
	private bool benchMark = false;
	public GameObject[] bmPoints;
	private float benchSpeed = 4.0f; 
	
	private Transform playerTransform;
	
	private int bmPointIndex;
	private int bmFPS;
	private int bmFPSCount;
	private int bmMinFPS;
	private int bmMaxFPS;
	
	private float underwaterLevel = 3.0f;
	private float airMax = 240.0f;
	private float healthMax = 100.0f;
	private float gameTimer = 600.0f;
	private float currentTimer = 0.0f;
	private int lives = 0;

	private float health = 0.0f;
	private float airTimer = 0.0f;
	private float depthMeter = 0.0f;
	private HUD hud = null;
	private PlayerControl playerControl;
	private FadeEffect fade;
	private int state;
	
	public bool isSurface
	{
	    get{return depthMeter <= 0;}
	}
	
	public void AddHealth(){
	    health = Mathf.Min(health + 10, healthMax);
	}
	
	void Start () {
		playerTransform = gameObject.transform;
		hud = (HUD)gameObject.GetComponent(typeof(HUD));
		playerControl = (PlayerControl)gameObject.GetComponent(typeof(PlayerControl));
		fade = (FadeEffect)gameObject.GetComponent(typeof(FadeEffect));
		airTimer = airMax;
		health = healthMax;
		currentTimer = 0.0f;
		
		if(PlayerPrefs.HasKey("benchMark")) {
			benchMark = true;
		}
		
		if(benchMark) {
			PlayerControl pc = (PlayerControl)gameObject.GetComponent(typeof(PlayerControl));
			if(pc != null)
				pc.setEnableControl(false);
			
			bmPointIndex = 0;
			bmMinFPS = 100;
			bmMaxFPS = 0;
			bmFPSCount = 0;
		} else {
			lives = 3;
            // Load();
		}
		Pause(false);
	}
	
	void Update () {
		if(currentTimer >= gameTimer) {
			Complete();
		} else {
			currentTimer += Time.deltaTime;
		}
		if(airTimer <= 0.0 || health < 1.0) {
			Fail();
		}
		depthMeter = (int)((underwaterLevel - playerTransform.position.y)/0.3);
		
		if(depthMeter > 0.0) {
			float airStep = Time.deltaTime;
			if(playerControl && playerControl.isBoost)
				airStep *= 4.0f;
			airTimer -= airStep;
		} else {
			if(airTimer < airMax)
				airTimer += 1.0f;
		}
		
		if(benchMark) {
			if(bmPointIndex < bmPoints.Length) {
				if(bmPoints[bmPointIndex]) {
					float distance = Vector3.Distance(playerTransform.position, bmPoints[bmPointIndex].transform.position);
					if(distance > 0.0f) {
						Vector3 step = ( bmPoints[bmPointIndex].transform.position - playerTransform.position).normalized * benchSpeed * Time.deltaTime;
						if(step.magnitude > distance) {
							playerTransform.position = bmPoints[bmPointIndex].transform.position;
						} else {
							playerTransform.rotation = Quaternion.Slerp(playerTransform.rotation, bmPoints[bmPointIndex].transform.rotation, Time.deltaTime * 1 / distance);
							playerTransform.position += step;
						}
					} else {
						bmPointIndex ++;
						Debug.Log("Distance : " + distance + " Go to point " + bmPointIndex);
						Debug.Break();
					}
				} 
				bmFPS = (int)(1 / Time.deltaTime);
				if(bmMaxFPS < bmFPS && bmFPS < 101)
					bmMaxFPS = bmFPS;
				if(bmMinFPS > bmFPS)
					bmMinFPS = bmFPS;
				bmFPSCount ++;
			} else {
				Debug.Log("Show Result");
				bmFPS = (int)(bmFPSCount / currentTimer);
				if(hud)
					hud.showBenchResult("FPS : " + bmFPS + " (" + bmMinFPS + "/" + bmMaxFPS + ")");
				Pause(true);
			}
		}
	}
	
	void setUnderwaterLevel(float arg) {
		underwaterLevel = arg;
	}
	
	public void Complete() {
		if(hud)
			hud.showComplete();
	}
	
	public void Fail() {
		FadeEffect fade = (FadeEffect)gameObject.GetComponent(typeof(FadeEffect));
		if(fade)
			fade.fadeIn();
		if(playerControl)
			playerControl.setEnableControl(false);
		if(hud)
			hud.showFail();
	}
	
	public void DoBite() {
		health -= 1.0f;
		if(fade)
			fade.setFadeAlpha(0.6f);
	}
	
	void Save () {
		PlayerPrefs.SetFloat("health", health);
		PlayerPrefs.SetString("transform", getPosition());
		PlayerPrefs.SetFloat("air", airTimer);
		PlayerPrefs.SetFloat("timer", currentTimer);
		PlayerPrefs.SetInt("lives", lives);
		
		HUD hud = (HUD)gameObject.GetComponent(typeof(HUD));
		if(hud)
			hud.saveFishes();
	}
	
	void Load() {
		health = PlayerPrefs.HasKey("health") ? PlayerPrefs.GetFloat("health") : health;
		airTimer = PlayerPrefs.HasKey("air") ? PlayerPrefs.GetFloat("air") : airTimer;
		currentTimer = PlayerPrefs.HasKey("timer") ? PlayerPrefs.GetFloat("timer") : currentTimer;
		lives = PlayerPrefs.HasKey("lives") ? PlayerPrefs.GetInt("lives") : lives;
		
		if(PlayerPrefs.HasKey("transform"))
			setPosition(PlayerPrefs.GetString("transform"));
		
		if(hud)
			hud.loadFishes();
	}
	
	public void Pause(bool arg) {
		if(arg) {
			Time.timeScale = 0.0f;
		} else {
			Time.timeScale = 1.0f;
		}
	}
	
	public void goMainMenu() {
		Save();
		Application.LoadLevel(0);
	}
	
	public void Continue() {
		if(fade)
			fade.fadeOut();
		if(playerControl)
			playerControl.setEnableControl(true);
		
		airTimer = airMax;
		health = healthMax;
		lives--;
	}
	
	public int getHealth() {
		return (int)health;
	}
	
	public void setHealth(float _health) {
		health = _health;
	}

	public int getDepth() {
		return (int)depthMeter;
	}
	
	public float getAir() {
		return (airTimer/airMax);
	}
	
	public void setAir(float air){
	    airTimer = air*airMax;
	}
	
	public int getLives() {
		return lives;
	}
	
	string getPosition() {
		string result = "";
		result += playerTransform.position.x + "&" + playerTransform.position.y + "&" + playerTransform.position.z + "&";
		result += playerTransform.rotation.x + "&";
		result += playerTransform.rotation.y + "&";
		result += playerTransform.rotation.z + "&";
		result += playerTransform.rotation.w + "&";
		return result;
	}
	
	void setPosition(string arg) {
		string[] param = arg.Split("&"[0]);
		int index = 0;
		playerTransform.position = new Vector3 (float.Parse(param[index++]), float.Parse(param[index++]), float.Parse(param[index++]));
		playerTransform.rotation = new Quaternion(float.Parse(param[index++]),float.Parse(param[index++]),float.Parse(param[index++]),float.Parse(param[index++]));
	}
	
	void OnApplicationQuit() {
		if(!benchMark) {
			Save();
		}
	}
}
