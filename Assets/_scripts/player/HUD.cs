using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour {
	public bool hideHud = false;
    public Texture2D fade;
    Fader fader;
	//Textures
	//GUI
	
    public GUITexture crosshairGUI;
    public Texture2D[] airTank;
	public GUITexture airTankGUI;
	public GUITexture fireButtonGUI;
	public Texture2D fireButtonOn;
	public Texture2D fireButtonOff;
	public Texture2D fireButtonHighlight;
	public GUITexture boostButtonGUI;
	public Texture2D boostButtonOn;
	public Texture2D boostButtonOff;
	public Texture2D boostButtonHighlight;
	public GUITexture aimButtonGUI;
	public Texture2D aimButtonOn;
	public Texture2D aimButtonOff;
	public Texture2D aimButtonHighlight;
    public Texture2D menuButton;
    public GUITexture menuButtonGUI; // for game mode only
	public GUITexture statusGUI;
	public Texture2D labelFail;
	public Texture2D labelComplete;
	
	//Menu
	public Texture2D bgMenu;
	public Texture2D bgGallery;
	
	//GUIStyle
	//GUI
	public GUIStyle healthText;
	public GUIStyle depthText;
	public GUIStyle statusText;
	
	//Menu
	public GUIStyle galleryText;
	public GUIStyle menuStyle;
	public GUIStyle buttonResume;
	public GUIStyle buttonGallery;
	public GUIStyle buttonMainMenu;
	public GUIStyle buttonTryAgain;
	public GUIStyle buttonContinue;
	public GUIStyle buttonNull;
	public GUIStyle buttonObjectives;
    public GUIStyle buttonNext;
	public GUIStyle buttonStart;
	public GUIStyle buttonGameOver;
	public GUIStyle buttonBuy;
	public GUIStyle buttonCancel;
    
	public GameObject tasksText;

	public GUIStyle objectivesStyle;
	public GUIStyle objectivesTitleStyle;
	
	//Menu
	private Vector2 btSize = new Vector2(238,32);
	private Rect btPesume;
	private Rect btGallery;
	private Rect btMainMenu;
	private Rect btFail;
   	private Rect btObjectives;
	
	//GUI
	private HighlightableControlButton buttonFire;	
	private HighlightableControlButton buttonAim;
	private HighlightableControlButton buttonBoost;
	
	private Rect btAim;	
	private Rect btMenu;
	private Rect btBoost;
	private Rect btFire;
	
	private Rect rcAirTank;
	private Rect rcCrosshair;
	private Rect rcWatch;
	
	private Rect rcMask;
	private Rect rcDepth;
	private Rect rcHealth;
	private Rect rcCount; 
	private Rect rcWeight;
	private Rect rcLives;	
	
	private bool isComplete = false;
	private bool IsFreeVersion = false;
	
	public static string upgradeText = "You have completed all levels of the free version. Buy full version now and enjoy more fun challenges.";
	
	private int lives = 0;
	private string benchString = "";
    private FishesInfo fishesInfo;
 	private Tasks tasks;
	private int level;

	private GameMaster gameMaster = null;
	private float weight = 0.0f;
	
	private ValueHolder airTankLevel;
	
	public enum GameState
	{
    	GAME,
    	PAUSE,
    	FAIL,
    	GALLERY,
    	BENCHMARK,
    	HIDEHUD,
	    OBJECTIVES,
		UPGRADE
	}
	
	private GameState _state;
	public GameState state
	{
	    get{return _state;}
	}
	
	public bool isGame
	{
	    get{return _state == GameState.GAME;}
	}
	
	public void BloodFlash(){
	    fader.alpha = 0.6f;
	}	
	
	void Awake() {	    
		useGUILayout = false;
		IsFreeVersion = PlayerPrefs.HasKey("IsFreeVersion") ? PlayerPrefs.GetInt("IsFreeVersion") > 0 ? true : false : false;
		fader = new Fader(fade);
		buttonFire = new HighlightableControlButton(this, fireButtonGUI, fireButtonOn, fireButtonOff, fireButtonHighlight);
		buttonBoost = new HighlightableControlButton(this, boostButtonGUI, boostButtonOn, boostButtonOff, boostButtonHighlight);
		buttonAim =  new HighlightableControlButton(this, aimButtonGUI, aimButtonOn, aimButtonOff, aimButtonHighlight);		
		fishesInfo = new FishesInfo();
		airTankLevel = new ValueHolder(airTank.Length - 1);
		airTankLevel.Subscribe(OnAirTankLevelChanged);
	}	
	
	public void InitTasks() {
		if(tasks == null) {
			level =  PlayerPrefs.HasKey("level") ? PlayerPrefs.GetInt("level") : 0;
			tasks = new Tasks(level, tasksText);
			tasks.Show();
		}
	}
	
	public void showAimButton(bool arg) {
		aimButtonGUI.active = arg;
	}
	
	public void InitTasksWithStart() {
		if(tasks == null) {
			level =  PlayerPrefs.HasKey("level") ? PlayerPrefs.GetInt("level") : 0;
			tasks = new Tasks(level, tasksText);
			tasks.Show();
			
			if(gameMaster) {
				gameMaster.Pause(false);
			}
			tasks.StartTask();
			_state = GameState.OBJECTIVES;
		}
	}
	
	void OnAirTankLevelChanged(object value){
	   if(airTankGUI != null) 
	        airTankGUI.texture = airTank[(int)value];
	}
	
	void Start () {
	    gameMaster = (GameMaster)gameObject.GetComponent(typeof(GameMaster));
		GUIInit();
		InitControlButton();
		_state = hideHud ? GameState.HIDEHUD : GameState.GAME;
	}	
	
	void GUIInit() {	    
		btMenu = new Rect(0, 0, 48, 48);
        // print("btMenu : " + qqq(btMenu));
		
		rcCrosshair = new Rect(Screen.width / 2 - 8, Screen.height / 2 - 8, 16, 16);
		rcAirTank = new Rect(Screen.width / 2 - 120, Screen.height - 150, 100,150);
		rcWatch = new Rect(Screen.width / 2 - 64, Screen.height - 130, 128 , 128);
		
		int yLayout = 124;
		btPesume = new Rect((Screen.width - btSize.x) / 2, yLayout, btSize.x, btSize.y);
		yLayout += (int)btSize.y + 4;
		btGallery = new Rect((Screen.width - btSize.x) / 2, yLayout, btSize.x, btSize.y);
		yLayout += (int)btSize.y + 4;
		btObjectives = new Rect((Screen.width - btSize.x) / 2, yLayout, btSize.x, btSize.y); 
		yLayout += (int)btSize.y + 4;
		btMainMenu = new Rect((Screen.width - btSize.x) / 2, yLayout, btSize.x, btSize.y);
		
		btFail = new Rect((Screen.width - btSize.x) / 2, (Screen.height - btSize.y)/2, btSize.x, btSize.y);
		
		rcMask = new Rect(0,0,Screen.width, Screen.height);
		rcDepth = new Rect(216, 230, 48, 26);
		rcHealth = new Rect(216, 264, 48, 26);
		
		// status		
		Rect rcStatus = statusGUI != null ? statusGUI.GetScreenRect() : new Rect(0,0,0,0);
		rcCount = new Rect(rcStatus.x + 49, - 2, 29, rcStatus.height);
		rcWeight = new Rect(rcStatus.x + 118, - 2, 84, rcStatus.height);
		rcLives = new Rect(rcStatus.x + 215, - 2, 32, rcStatus.height);		
	}

	void OnGUI() {	    
	    bool isGame = false;
	    GUI.color = Color.white;
	    fader.FadeOut();
	    
		switch(_state) {
			case GameState.GAME :			
			    GameMaster.ShowAd(false);
				isGame = true;
				int depth = 0;
                int health = 0;                

                int airLevel = (int)Mathf.Round(gameMaster.airLeft * (airTank.Length - 1));
                airTankLevel.value = (int)Mathf.Clamp(airLevel, 0, airTank.Length - 1);
                depth = (int)gameMaster.depth;
                health = gameMaster.getHealth();
                lives = gameMaster.getLives();
                
                if(menuButtonGUI){                    
                    if(Application.platform == RuntimePlatform.OSXPlayer){
                        foreach(iPhoneTouch touch in iPhoneInput.touches){
                	        Vector2 touchCoords = new Vector2(touch.position.x, Screen.height - touch.position.y);
                	        if(touch.phase != iPhoneTouchPhase.Ended && menuButtonGUI.HitTest(touchCoords)){
                	            JukeBox.Tap();
                                gameObject.SendMessage("Pause", true);
                                _state = GameState.PAUSE;
								break;
                	        }   
                	    }
                    }else{
                        if(Input.GetMouseButtonDown(0) &&                        
                            menuButtonGUI.HitTest(Input.mousePosition)){                                
                            JukeBox.Tap();
                            if(gameObject != null) {
								gameObject.SendMessage("Pause", true);
                            }
							if(tasks != null) {
								tasks.Hide();
							}
							_state = GameState.PAUSE;
						}                        
                    }
                }
				
                GUI.Label(rcDepth, "" + depth + " ft", depthText);
                GUI.Label(rcHealth, health + "", healthText);
                
                //Status
                GUI.Label(rcCount, "" + fishesInfo.FishesCount, statusText);
                GUI.Label(rcWeight, fishesInfo.getWeightToString() + " lbs.", statusText);
                GUI.Label(rcLives, "" + lives, statusText);
                fader.OnGUI();
				
				if(tasks != null) {
					switch(tasks.Update(Time.deltaTime)) {
						case Tasks.TaskStates.IN_PROCESS :
							break;
						case Tasks.TaskStates.FAIL :
							gameObject.SendMessage("Pause", true);
	                        _state = GameState.OBJECTIVES;
							break;
						case Tasks.TaskStates.COMPLETE :
							gameObject.SendMessage("Pause", true);
	                        _state = GameState.OBJECTIVES;
							break;
					}
				}
				break;
			case GameState.PAUSE :
				GameMaster.ShowAd(false);
				if(bgMenu) {
					GUI.DrawTexture(new Rect(0,0,Screen.width, Screen.height), bgMenu);
				}
				if(GUI.Button(btPesume, "", buttonResume)) {
				    JukeBox.Tap();
					if(gameMaster) {
						gameMaster.Pause(false);
					}
					if(tasks != null) {
						tasks.Show();
					}
					_state = hideHud ? GameState.HIDEHUD : GameState.GAME;
				}
				if(GUI.Button(btGallery, "", buttonGallery)) {
				    JukeBox.Tap();
					_state = GameState.GALLERY;
				}
				if(tasks != null) {
					if(GUI.Button(btObjectives, "", buttonObjectives)) {
						JukeBox.Tap();
						_state = GameState.OBJECTIVES;
					}
				}
				if(GUI.Button(tasks != null ? btMainMenu : btObjectives, "", buttonMainMenu)) {
				    JukeBox.Tap();
					if(gameMaster)
						gameMaster.goMainMenu();
				}
				break;
			case GameState.FAIL:
				GameMaster.ShowAd(false);
				fader.FadeIn();
				fader.OnGUI();				
				if(lives > 1) {
					if(GUI.Button(btFail, "", buttonContinue)) {
					    JukeBox.Tap();
						if(gameMaster) {
							gameMaster.Continue();
						}
						_state = hideHud ? GameState.HIDEHUD : GameState.GAME;
					}
				} else {
					//tasks.Hide();
					if(GUI.Button(btFail, "", buttonGameOver)) {
					    JukeBox.Tap();
						showComplete();
						//MainMenu.CleanPlayerPrefs();
						//Application.LoadLevel(Application.loadedLevel);
					}
				}				
				break;
			case GameState.GALLERY :			    
				GameMaster.ShowAd(false);
				if(bgGallery) {
					GUI.DrawTexture(new Rect(0,0,Screen.width, Screen.height), bgGallery);
				}
				GUI.Label(new Rect(292,125,64,24), "" + fishesInfo.getCountByType(FishesInfo.YELLOWFINTUNA), galleryText);
				GUI.Label(new Rect(340,125,80,24), "" + fishesInfo.getWeightByTypeToString(FishesInfo.YELLOWFINTUNA) + " lbs.", galleryText);
				
				GUI.Label(new Rect(292,172,64,24), "" + fishesInfo.getCountByType(FishesInfo.REDSNAPPER), galleryText);
				GUI.Label(new Rect(340,172,80,24), "" + fishesInfo.getWeightByTypeToString(FishesInfo.REDSNAPPER) + " lbs.", galleryText);
				
				GUI.Label(new Rect(292,219,64,24), "" + fishesInfo.getCountByType(FishesInfo.GROUPER), galleryText);
				GUI.Label(new Rect(340,219,80,24), "" + fishesInfo.getWeightByTypeToString(FishesInfo.GROUPER) + " lbs.", galleryText);
				
				GUI.Label(new Rect(136,262,64,24), "" + fishesInfo.FishesCount, galleryText);				
				GUI.Label(new Rect(375,262,88,24), "" + fishesInfo.getWeightToString() + " lbs.", galleryText);
				
				if(GUI.Button(new Rect(100,292,237, 88), "", buttonNull)){
				    JukeBox.Tap();
				    PlayerPrefs.SetInt("totalFishes", fishesInfo.FishesCount);
				    PlayerPrefs.SetFloat("totalWeight", fishesInfo.getWeight());				    
				    PlayerPrefs.SetInt("upload", 1);
				}
				
				if(GUI.Button(btMenu, menuButton, menuStyle)) {
				    JukeBox.Tap();
					if(isComplete) {
						if(Application.levelCount > 2) {
							PlayerPrefs.SetString("player", "Player:" + fishesInfo.FishesCount + ":" + fishesInfo.getWeight());
							Application.LoadLevel(Application.levelCount - 1);
						} else  
							Application.LoadLevel(0);
					} else
						_state = GameState.PAUSE;
				}
				break;
			case GameState.BENCHMARK :			
				if(benchString != "") {
					GUI.Label(new Rect(1, 64, Screen.width,26), benchString,  galleryText);
				}
				if(menuButton && GUI.Button(btMenu, menuButton, menuStyle)) {
					Application.LoadLevel(0);
				}
				break;
			case GameState.UPGRADE :
				GameMaster.ShowAd(false);
				IsFreeVersion = PlayerPrefs.HasKey("IsFreeVersion") ? PlayerPrefs.GetInt("IsFreeVersion") > 0 ? true : false : false;
				if(IsFreeVersion) {
					if(bgMenu) {
						GUI.DrawTexture(new Rect(0,0,Screen.width, Screen.height), bgMenu);
					}
					GUI.Label(new Rect((Screen.width - 180)/ 2,100, 180, 140),"Congratulations!", objectivesTitleStyle);
	                GUI.Label(new Rect(50,140,Screen.width - 100, Screen.height - 200), upgradeText, objectivesStyle);
					if(GUI.Button(new Rect(1, Screen.height - btSize.y - 20, btSize.x, btSize.y), "", buttonBuy)) {
						JukeBox.Tap();
						PlayerPrefs.SetInt("upgrade", 1);
					}
					if(GUI.Button(new Rect((Screen.width - btSize.x - 1), Screen.height - btSize.y - 20, btSize.x, btSize.y), "", buttonCancel)) {
						JukeBox.Tap();
						showComplete();
					}
				} else {
					_state = GameState.OBJECTIVES;
					if(MainMenu.OpenNewLevel(level)) {
						tasks.StartTask(level);
						PlayerPrefs.SetInt("level", level);
					} else {
						showComplete();
					}
				}
				break;
			case GameState.HIDEHUD :
				break;
			case GameState.OBJECTIVES :
				GameMaster.ShowAd(true);
				if(bgMenu) {
					GUI.DrawTexture(new Rect(0,0,Screen.width, Screen.height), bgMenu);
				}
				GUI.Label(new Rect(30,100, Screen.width - 60, 140),"objectives :", objectivesTitleStyle);
                GUI.Label(new Rect(40,140,Screen.width - 80, Screen.height - 200), MainMenu.objectives[level], objectivesStyle);
				
				switch(tasks.State) {
					case Tasks.TaskStates.COMPLETE :
						if(labelComplete) {
							GUI.DrawTexture(new Rect((Screen.width - 256)/2, (Screen.height - 256)/2, 256, 256), labelComplete);
						}
						if(GUI.Button(new Rect((Screen.width - btSize.x)/2, Screen.height - btSize.y - 20  - (IsFreeVersion ? 50 : 0), btSize.x, btSize.y), "", buttonNext)) {
							JukeBox.Tap();
							level++;
							if(MainMenu.OpenNewLevel(level)) {
								tasks.StartTask(level);
								PlayerPrefs.SetInt("level", level);
							} else {
								if(IsFreeVersion) {
									showUpgrade();
								} else {
								   	showComplete();
								}
							}
						}
						break;
					case Tasks.TaskStates.FAIL :
						if(labelFail) {
							GUI.DrawTexture(new Rect((Screen.width - 256)/2, (Screen.height - 256)/2, 256, 256), labelFail);
						}
						if(GUI.Button(new Rect((Screen.width - btSize.x)/2, Screen.height - btSize.y - 20 - (IsFreeVersion ? 50 : 0), btSize.x, btSize.y), "", buttonTryAgain)) {
							JukeBox.Tap();
							tasks.StartTask(level);
							//Application.LoadLevel(Application.loadedLevel);
						}
						break;
					case Tasks.TaskStates.IN_PROCESS :
						if(GUI.Button(btMenu, menuButton, menuStyle)) {
						    JukeBox.Tap();
							_state = GameState.PAUSE;
						}
						break;
					case Tasks.TaskStates.NEW :
						if(GUI.Button(new Rect((Screen.width - btSize.x)/2, Screen.height - btSize.y - 20  - (IsFreeVersion ? 50 : 0), btSize.x, btSize.y), "", buttonStart)) {
							JukeBox.Tap();
							if(gameMaster) {
								gameMaster.Pause(false);
							}
							tasks.setStarted();
							_state = hideHud ? GameState.HIDEHUD : GameState.GAME;
						}
						break;
				}
				break;
		}		
		gameMaster.isGame.value = isGame;
	}
	
	public void InitControlButton() {
		gameObject.SendMessage("setAimButtonControl", buttonAim);
		gameObject.SendMessage("setBoostButtonControl", buttonBoost);
		gameObject.SendMessage("setFireButtonControl", buttonFire);
		gameObject.SendMessage("setDepthTextStyle", depthText);
		gameObject.SendMessage("setHealthTextStyle", healthText);
	}
	
	
	public void showFail() { _state = GameState.FAIL;}
	
	public void showGame() { _state = hideHud ? GameState.HIDEHUD : GameState.GAME; }
	
	public void showComplete() {
		isComplete = true;
		_state = GameState.GALLERY;		
	}
	
	public void showUpgrade() {
		_state = GameState.UPGRADE;
	}
	
	public void showBenchResult(string arg) { benchString = arg; _state = GameState.BENCHMARK; }
	
	public void setCrosshair(Vector2 arg) {
        // rcCrosshair.x = arg.x - (rcCrosshair.width / 2);
        // rcCrosshair.y = (Screen.height - arg.y) - (rcCrosshair.height / 2);
        Vector3 pos = crosshairGUI.transform.position;
        pos.x = arg.x/Screen.width;
        pos.y = arg.y/Screen.height;        
        crosshairGUI.transform.position = pos;
	}
	
	
	public void addFish(string arg) { 
		fishesInfo.addFish(arg);
		if(tasks != null) {
			tasks.addFish(arg);
		}
	}

	public void Save() {
		if(fishesInfo != null) {
			fishesInfo.Save();
		}
		if(tasks != null) {
			tasks.Save();
		}
	}
	
	public void Load() {
		if(gameMaster != null) {
			lives = gameMaster.getLives();
		}
		if(fishesInfo != null) {
			fishesInfo.Load();
		}
		if(tasks != null) {
			tasks.Load();
		}
	}
	
}

class Fader {
    bool isFade = false;
    public float alpha = 0f;
    float maxAlpha = 0.8f;
    float speed = 0.2f;
    Rect rect;
    float lastDeltaTime;
    
    Texture2D fade;
    public Fader(Texture2D _fade){
        fade = _fade;
        rect = new Rect(0, 0, Screen.width, Screen.height);
    }
    
    public void FadeIn(){
        isFade = true;
    }
    public void FadeOut(){
        isFade = false;
    }
    
    public void OnGUI(){
        if(!Utils.Approximately(Time.deltaTime, 0f)) { 
			lastDeltaTime = Time.deltaTime;
		}
        float delta = (isFade ? 1f : -1f) * speed * lastDeltaTime;
        alpha += delta;        
        alpha = Mathf.Clamp(alpha, 0f, maxAlpha);
        if(!Utils.Approximately(alpha, 0.0f)) {
            Color old = GUI.color;
            GUI.color = new Color(1.0f, 1.0f, 1.0f, alpha);
		    GUI.DrawTexture(rect, fade);    
		    GUI.color = old;
		}		
    }
}