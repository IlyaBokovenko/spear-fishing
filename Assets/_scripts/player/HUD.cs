using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour {
	public bool hideHud = false;
	public bool isFreeVersion = false;
	//Textures
	//GUI
	public Texture2D crosshair;
	public Texture2D mask;
	public Texture2D[] airTank;
	public Texture2D fireButtonOn;
	public Texture2D fireButtonOff;
	public Texture2D fireButtonHighlight;
	public Texture2D boostButtonOn;
	public Texture2D boostButtonOff;
	public Texture2D boostButtonHighlight;
	public Texture2D aimButtonOn;
	public Texture2D aimButtonOff;
	public Texture2D aimButtonHighlight;
	public Texture2D menuButton;
	public Texture2D watch;
	public Texture2D status;
	
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

    public AudioClip menuTap;
	
	//Menu
	private Vector2 btSize = new Vector2(238,32);
	private Rect btPesume;
	private Rect btGallery;
	private Rect btMainMenu;
	private Rect btFail;
	
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
	private Rect rcAirTankBar;
	private Rect rcWatch;
	private Rect rcStatus;
	
	private bool isComplete = false;
	
	private int lives = 0;
	private string benchString = "";
	private ArrayList fishes;
	private FishInfo fishInfo;
	private GameMaster gameMaster = null;
	private int weight = 0;
	
	public enum GameState
	{
    	GAME,
    	PAUSE,
    	FAIL,
    	GALLERY,
    	BENCHMARK,
    	HIDEHUD	    
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
	
	void Awake() {
		useGUILayout = false;
		 buttonFire = new HighlightableControlButton(this, new Rect(Screen.width - 76, Screen.height - 90, 68, 68), fireButtonOn, fireButtonOff, fireButtonHighlight);
		buttonBoost = new HighlightableControlButton(this, new Rect(Screen.width - 150, Screen.height - 72, 68, 68), boostButtonOn, boostButtonOff, boostButtonHighlight);
		buttonAim =  new HighlightableControlButton(this, new Rect(0, Screen.height - 128, 128, 128), aimButtonOn, aimButtonOff, aimButtonHighlight);		
		fishes = new ArrayList();
	}	
	
	void Start () {
		GUIInit();
		InitControlButton();
		_state = hideHud ? GameState.HIDEHUD : GameState.GAME;
		gameMaster = (GameMaster)gameObject.GetComponent(typeof(GameMaster));
		
		PlayerPrefs.SetInt("free_version", isFreeVersion ? 1 : 0);
	}	

	void GUIInit() {
		btMenu = new Rect(0, 0, 48, 48);
		
		rcCrosshair = new Rect(Screen.width / 2 - 8, Screen.height / 2 - 8, 16, 16);
		rcAirTank = new Rect(Screen.width / 2 - 120, Screen.height - 150, 100,150);
		rcWatch = new Rect(Screen.width / 2 - 64, Screen.height - 130, 128 , 128);
		
		float statusHeight = isFreeVersion ? 50 : 0;
		rcStatus = new Rect(Screen.width / 2 - 115, statusHeight, 229, 26);
		
		int yLayout = 134;
		btPesume = new Rect((Screen.width - btSize.x) / 2, yLayout, btSize.x, btSize.y);
		yLayout += (int)btSize.y + 4;
		btGallery = new Rect((Screen.width - btSize.x) / 2, yLayout, btSize.x, btSize.y);
		yLayout += (int)btSize.y + 4;
		btMainMenu = new Rect((Screen.width - btSize.x) / 2, yLayout, btSize.x, btSize.y);
		
		btFail = new Rect((Screen.width - btSize.x) / 2, (Screen.height - btSize.y)/2, btSize.x, btSize.y);
	}

	void OnGUI() {	    
	    bool isGame = false;
	    
		switch(_state) {
			case GameState.GAME :
			    isGame = true;                
			    
				if(crosshair)
					GUI.DrawTexture(rcCrosshair, crosshair);
				if(mask)
					GUI.DrawTexture(new Rect(0,0,Screen.width, Screen.height), mask);
				
				int index = 0;
				int depth = 0;
				int health = 0;
				
				if(gameMaster) {
					index = (int)((gameMaster.getAir() * airTank.Length));
					depth = (int)gameMaster.depth;
					health = gameMaster.getHealth();
					lives = gameMaster.getLives();
				}
				if(index >= airTank.Length) 
					index = airTank.Length - 1;
				if(airTank[index])
					GUI.DrawTexture(rcAirTank, airTank[index]);
				
				if(menuButton && GUI.Button(btMenu, menuButton, menuStyle)) {
					gameObject.SendMessage("Pause", true);
					_state = GameState.PAUSE;
				}
				if(buttonFire != null)
					buttonFire.Draw();
				if(buttonAim != null)
					buttonAim.Draw();
				if(buttonBoost != null)
					buttonBoost.Draw();
				
				if(watch)
					GUI.DrawTexture(rcWatch, watch);
				if(status)
					GUI.DrawTexture(rcStatus, status);
				
				GUI.Label(new Rect(216, 230, 48, 26), "" + depth + " ft", depthText);
				GUI.Label(new Rect(216, 264, 48, 26), health + "", healthText);
				//Status
				GUI.Label(new Rect(rcStatus.x + 50, rcStatus.y, 32, rcStatus.height), "" +fishes.Count, statusText);
				GUI.Label(new Rect(rcStatus.x + 120, rcStatus.y, 48, rcStatus.height), "" + weight, statusText);
				GUI.Label(new Rect(rcStatus.x + 200, rcStatus.y, 32, rcStatus.height), "" + lives, statusText);
				break;
			case GameState.PAUSE :
				if(bgMenu)
					GUI.DrawTexture(new Rect(0,0,Screen.width, Screen.height), bgMenu);
				if(GUI.Button(btPesume, "", buttonResume)) {
				    JukeBox.Tap();
					if(gameMaster)
						gameMaster.Pause(false);
					_state = hideHud ? GameState.HIDEHUD : GameState.GAME;
				}
				if(GUI.Button(btGallery, "", buttonGallery)) {
				    JukeBox.Tap();
					fishInfo = new FishInfo(fishes);
					_state = GameState.GALLERY;
				}
				if(GUI.Button(btMainMenu, "", buttonMainMenu)) {
				    JukeBox.Tap();
					if(gameMaster)
						gameMaster.goMainMenu();
				}
				break;
			case GameState.FAIL :
				if(lives > 0) {
					if(GUI.Button(btFail, "", buttonContinue)) {
					    JukeBox.Tap();
						if(gameMaster)
							gameMaster.Continue();
						_state = hideHud ? GameState.HIDEHUD : GameState.GAME;
					}
				} else {
					if(GUI.Button(btFail, "", buttonTryAgain)) {
					    JukeBox.Tap();
						MainMenu.CleanPlayerPrefs();
						Application.LoadLevel(Application.loadedLevel);
					}
				}
				break;
			case GameState.GALLERY :
				if(bgGallery)
					GUI.DrawTexture(new Rect(0,0,Screen.width, Screen.height), bgGallery);
				
				GUI.Label(new Rect(292,125,64,24), "" + fishInfo.getCount(FishInfo.YELLOWFINTUNA), galleryText);
				GUI.Label(new Rect(348,125,64,24), "" + fishInfo.getWeightString(FishInfo.YELLOWFINTUNA), galleryText);
				
				GUI.Label(new Rect(292,172,64,24), "" + fishInfo.getCount(FishInfo.REDSNAPPER), galleryText);
				GUI.Label(new Rect(348,172,64,24), "" + fishInfo.getWeightString(FishInfo.REDSNAPPER), galleryText);
				
				GUI.Label(new Rect(292,219,64,24), "" + fishInfo.getCount(FishInfo.GROUPER), galleryText);
				GUI.Label(new Rect(348,219,64,24), "" + fishInfo.getWeightString(FishInfo.GROUPER), galleryText);
				
				GUI.Label(new Rect(136,262,64,24), "" + fishes.Count, galleryText);
				GUI.Label(new Rect(386,262,64,24), "" + (int)weight, galleryText);
				
				if(GUI.Button(new Rect(100,292,237, 88), "", buttonNull)){
				    JukeBox.Tap();
				    PlayerPrefs.SetInt("totalFishes", fishes.Count);
				    PlayerPrefs.SetInt("totalWeight", weight);				    
				    PlayerPrefs.SetInt("upload", 1);
				}
				
				if(GUI.Button(btMenu, menuButton, menuStyle)) {
				    JukeBox.Tap();
					if(isComplete) {
						if(Application.levelCount > 2) {
							PlayerPrefs.SetString("player", "Player:" + fishes.Count + ":" + weight);
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
			case GameState.HIDEHUD :
				break;
		}
		
		gameMaster.isGame = isGame;
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
		fishInfo = new FishInfo(fishes);
		isComplete = true;
		_state = GameState.GALLERY;		
	}
	
	public void showBenchResult(string arg) { benchString = arg; _state = GameState.BENCHMARK; }
	
	public void setCrosshair(Vector2 arg) {
		rcCrosshair.x = arg.x - (rcCrosshair.width / 2);
		rcCrosshair.y = (Screen.height - arg.y) - (rcCrosshair.height / 2);
	}
	
	public void addFish(string arg) {
		fishes.Add(arg);
		string[] param = arg.Split(":"[0]);
		weight += FishInfo.getInfo(param[0], float.Parse(param[1]));
	}

	public void saveFishes() {
		int index = 0;
		foreach(string fish in fishes) {
			PlayerPrefs.SetString("fish" + (index++), fish);
		}
	}
	
	public void loadFishes() {
		int index = 0;
		string key = "";
		while(true) {
			key = "fish" + (index++);
			if(PlayerPrefs.HasKey(key)) {
				addFish(PlayerPrefs.GetString(key));
			} else {
				break;
			}
		}
	}
}
