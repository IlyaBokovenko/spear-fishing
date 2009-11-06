using UnityEngine;
using System.Collections;

public class GameMaster : MonoBehaviour {
    public PrefHolder isFreeVersion;
    public AudioClip arrghSound;
    public AudioClip biteSound;
    
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
	private const float healthMax = 100.0f;
	private float gameTimer = 600.0f;
	private float currentTimer = 0.0f;
	private int lives = 0;

	private float health = healthMax;
	
	private HUD hud = null;
	private PlayerControl playerControl;

	Tutorial tutorial;
	
	// oxygen
	private int minutesToBreath;
	private bool airLocked = false;
	private const float airLowTreshold = 0.33f;
	private float _airLeft = 1.0f;	
	public float airLeft
	{
	    get{return _airLeft;}
	    private set{
	        bool oxygenLowChanged = value != airLowTreshold && 
	                        (airLowTreshold - _airLeft) * (airLowTreshold - value) <= 0;
	        _airLeft = value;
	        if(oxygenLowChanged) isOxygenLow.value = _airLeft <= airLowTreshold;	        
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
	
	public static void ShowTutorialNextTime(){
	    PrefHolder.newBool("ShowTutorial", true).value = true;
	}
	private static void DontShowTutorialNextTime(){
	    PrefHolder.newBool("ShowTutorial", true).value = false;
	}
	public static bool IsNeedShowTutorial(){
	    return PrefHolder.newBool("ShowTutorial", true);
	}
			
	// depth
	private float _depthMeter = 0.0f;
	public float depth
	{
	    get{return _depthMeter;}
	    private set{_depthMeter = value; isSurface.value = value <= 0;}
	}
	
	public void SetUnderwaterLevel(float newUnderwaterLevel){
	    underwaterLevel = newUnderwaterLevel - 0.05f;
	}
	
	// is player on surface
	public ValueHolder isSurface;
	
	private bool isFailed = false;
	private bool isComplete = false;
	
	private bool isBeingBiting = false;
	
	//////////////////////////////////////////////////////////////////////
	
	public void AddHealth(){
	    health = Mathf.Min(health + 10, healthMax);
	}
	
	void Awake(){
	    isGame = PrefHolder.newBool("game", false);
	    isOxygenLow = new ValueHolder(false);
	    isSurface = new ValueHolder(false);	    
	    isFreeVersion = PrefHolder.newBool("IsFreeVersion", false);
	    
	    tutorial = (Tutorial)GetComponent(typeof(Tutorial));
	    if(IsNeedShowTutorial()){
	        tutorial.enabled = true;
	        DontShowTutorialNextTime();
	    }
	        
	}
	
	void Start () {
		playerTransform = gameObject.transform;
		hud = (HUD)gameObject.GetComponent(typeof(HUD));
		playerControl = (PlayerControl)gameObject.GetComponent(typeof(PlayerControl));
        // fade = (FadeEffect)gameObject.GetComponent(typeof(FadeEffect));
		
		minutesToBreath = PlayerPrefs.GetInt("minutesToBreath", 2);
		
		if(PlayerPrefs.HasKey("benchMark")) {
			benchMark = true;
		}
		
		if(benchMark) {
		    isGame.value = false;
		    			
			bmPointIndex = 0;
			bmMinFPS = 100;
			bmMaxFPS = 0;
			bmFPSCount = 0;
		} else {			
            Load();
            if(isFailed)
                SpendLife();
		}		
		
		Pause(false);
	}
	
	void Update () {
	    
	    if(isComplete)
	        return;
	    isComplete = currentTimer >= gameTimer;
	    if(isComplete){
	        Complete();
	        return;
	    }
	    
	    if(isFailed)
	        return;	        
	    isFailed = airLeft <= 0.0 || health < 1.0;
	    if(isFailed){
	        Fail();
	        return;
	    }
	        
		currentTimer += Time.deltaTime;
		
		depth = (underwaterLevel - playerTransform.position.y)/0.3f;		
		CalculateAir();		
		if(benchMark)
            Benchmark();		
	}
	
	void CalculateAir(){
		float airStep = Time.deltaTime/(minutesToBreath*60);
		if(!isSurface) {
		    if(!airLocked){    			
    			if(playerControl && playerControl.isBoost)
    				airLeft -= 4*airStep;
    			else
    			    airLeft -= airStep;		        
		    }
		} else {		    
		    float rate = 0.1f*Time.deltaTime;
			if(airLeft + rate <= 1.0f)
				airLeft += rate;
		}	    
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
		if(hud)
			hud.showFail();		
    	Pause(true);		
	}
	
	public void Continue() {
	    Pause(false);			
		SpendLife();
	}
	
	void SpendLife(){
	    if(lives > 0){
    		airLeft = 1.0f;
    		health = healthMax;
    		lives--;
    		isFailed = false;	    	        
	    }
	}
	
	public void DoBite() {
		health -= 10.0f;
		if(hud)
			hud.BloodFlash();
			
		if(!isBeingBiting)
		    StartCoroutine("PlayBiting");
	}
	
	IEnumerator PlayBiting(){
	    isBeingBiting = true;
	    audio.PlayOneShot(biteSound);
	    yield return new WaitForSeconds(0.5f);
	    audio.PlayOneShot(arrghSound);
	    yield return new WaitForSeconds(0.5f);
	    isBeingBiting = false;
	}
	
	void Save () {
	    if(tutorial.enabled == true) return;
	    
		PlayerPrefs.SetFloat("health", health);
		PlayerPrefs.SetString("transform", getPosition());
		PlayerPrefs.SetFloat("air", airLeft);
		PlayerPrefs.SetFloat("timer", currentTimer);
		PlayerPrefs.SetInt("lives", lives);
		
		HUD hud = (HUD)gameObject.GetComponent(typeof(HUD));
		if(hud)
			hud.saveFishes();
	}
	
	void Load() {
         // if(Application.platform == RuntimePlatform.OSXEditor)
         //      return;
	    
        health = PlayerPrefs.GetFloat("health", healthMax);
		airLeft = PlayerPrefs.GetFloat("air", 1.0f);
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
	
	public int getHealth() {
		return (int)health;
	}
	
	public void setHealth(float _health) {
		health = _health;
	}

	public void LockOxygenLow(){	    
	    airLocked = true;
	    airLeft = airLowTreshold - 0.1f;
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
