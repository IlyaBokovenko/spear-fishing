using UnityEngine;
using System.Collections;

public class GameMaster : MonoBehaviour {
    public bool isFreeVersion = false;
    public AudioClip arrghSound;
    
	private bool benchMark = false;
	public GameObject[] bmPoints;
	private float benchSpeed = 4.0f; 
	
	private Transform playerTransform;
	
	private int bmPointIndex;
	private int bmFPS;
	private int bmFPSCount;
	private int bmMinFPS;
	private int bmMaxFPS;
	
	public float underwaterLevel = 3.0f;
	private float airMax;
	private float healthMax = 100.0f;
	private float gameTimer = 600.0f;
	private float currentTimer = 0.0f;
	private int lives = 0;

	private float health = 0.0f;
	
	private HUD hud = null;
	private PlayerControl playerControl;
	private FadeEffect fade;
	private int state;
	
	// oxygen
	private bool airLocked = false;
	private const float airTreshold = 0.33f;
	private float _airTimer = 0.0f;	
	private float airTimer
	{
	    get{return _airTimer;}
	    set{
	        float treshold = airMax*airTreshold;
	        bool oxygenLowChanged = value != treshold && 
	                        (treshold - _airTimer) * (treshold - value) <= 0;
	        _airTimer = value;
	        if(oxygenLowChanged) isOxygenLow.value = _airTimer <= treshold;	        
	    }
	}
	public ValueHolder isOxygenLow;	
	
	// is player in game or in menu
	public PrefHolder isGame;
	
	// only use if no GameMaster component in scene
	public static void SetGame(bool value){
	    PrefHolder.newBool("game", false).value = value;
	}
	public static bool IsGame(){
	    return PrefHolder.newBool("game", false);
	}	


		
	// depth
	private float _depthMeter = 0.0f;
	public float depth
	{
	    get{return _depthMeter;}
	    private set{_depthMeter = value; isSurface.value = value <= 0;}
	}
	
	// is player on surface
	public ValueHolder isSurface;
	
	//////////////////////////////////////////////////////////////////////
	
	public void AddHealth(){
	    health = Mathf.Min(health + 10, healthMax);
	}
	
	void Awake(){
	    isGame = PrefHolder.newBool("game", false);
	    isOxygenLow = new ValueHolder(false);
	    isSurface = new ValueHolder(false);
	}
	
	void Start () {	    
	    PlayerPrefs.SetInt("free_version", isFreeVersion ? 1 : 0);
		playerTransform = gameObject.transform;
		hud = (HUD)gameObject.GetComponent(typeof(HUD));
		playerControl = (PlayerControl)gameObject.GetComponent(typeof(PlayerControl));
		fade = (FadeEffect)gameObject.GetComponent(typeof(FadeEffect));
		
		int minutesToBreath = PlayerPrefs.GetInt("minutesToBreath", 2);
        airMax = minutesToBreath * 60;
        airTimer = airMax;
        airTimer = 20;
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
            Load();
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
		depth = (underwaterLevel - playerTransform.position.y)/0.3f;
		
		if(!isSurface) {
		    if(!airLocked){
    			float airStep = Time.deltaTime;
    			if(playerControl && playerControl.isBoost)
    				airStep *= 4.0f;
    			airTimer -= airStep;		        
		    }
		} else {
			if(airTimer < airMax)
				airTimer += 1.0f;
		}
		
		if(benchMark)
            Benchmark();		
	}
	
	void Benchmark(){
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
	
	public void Complete() {
	    Pause(true);
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
			
		audio.PlayOneShot(arrghSound);
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
        // if(Application.platform == RuntimePlatform.OSXEditor)
        //     return;
	    
        health = PlayerPrefs.GetFloat("health", healthMax);
		airTimer = PlayerPrefs.GetFloat("air", airMax);
		currentTimer = PlayerPrefs.GetFloat("timer", 0.0f);
		lives = PlayerPrefs.GetInt("lives", 3);
		
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

	public float getAir() {
		return (airTimer/airMax);
	}
	
	public void LockOxygenLow(){	    
	    airLocked = true;
	    airTimer = airTreshold*airMax - 0.1f;
	}
	
	public void UnlockOxygen(){
	    airLocked = false;
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
