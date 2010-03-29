using UnityEngine;
using System.Collections;

// [ExecuteInEditMode]
public class MainMenu : MonoBehaviour {
	public static int MAX_LEVEL = 10;
	public static int MAX_LEVEL_TRIAL = 3;
	
	public Texture2D bgTexture;
	public Texture2D bgSettings;
	public Texture2D labelLoading;
	//Buttons
	public GUIStyle btResume;
	public GUIStyle btStart;
	public GUIStyle btSettings;
	public GUIStyle btHelp;
	public GUIStyle btNull;
	public GUIStyle btBack;
	public GUIStyle btBenchMark;
	public GUIStyle lnMoreGames;
	public GUIStyle btHighScores;
	public GUIStyle btStartLevel;
	public GUIStyle btFullVersion;
	public GUIStyle btLevel;
	public GUIStyle btNoActiveLevel;	
	
	public GUIStyle controlStyle;
	public GUIStyle labelStyle;
	public GUIStyle objectivesStyle;
	public GUIStyle objectivesTitleStyle;
	public AudioClip menuTap;
	
	private PrefHolder control;
	private PrefHolder minutesToBreath;	
	private PrefHolder difficulty;	
	private PrefHolder sound;
	private PrefHolder graphicsLevel;
	private int openLevel;
	private bool IsFreeVersion;
	
	//Rects
	private static Vector2 btSize = new Vector2(238,32);
	private static Vector2 btLevelSize = new Vector2(140, 32);
	private Rect rectResume;
	private Rect rectStart;
	private Rect rectHighScores;
	private Rect rectSettings;	
	private Rect rectBenchMark;
	private Rect rectHelp;
	private Rect rectFullVersion;
	private Rect rectMoreGames;
	private int selectLevel;
	
	enum State
	{
        MENU,
        SETTINGS,
        LOADING,
	    LEVELS,
		OBJECTIVES
	}
	
	private State state;
	//Test
	private bool isResume;
	private int count;
	
	public static string[] objectives = {	
		"You need to catch 3 fish.",
		"You need to catch fish which their total weight is over 15 lbs.",
		"You need to catch 2 different types of fish.",
		"You need to catch 3 different types of fish.",
		"You need to catch 2 Grouper fish & 2 Red Snapper fish.",
		"You need to catch 2 Grouper fish & 2 Red Snapper fish & 1 Yellowfin tuna.",
		"You need to catch 3 Grouper fish & 3 Red Snapper fish & 3 Yellowfin tuna.",
		"You need to catch fish which their total weight is over 50 lbs & 3 different types of fish.",
		"You need to catch fish which their total weight is over 60 lbs & 3 Grouper fish & 3 Red Snapper fish & 3 Yellowfin tuna.",
		"You need to catch fish which their total weight is over 90 lbs."
	};
	
	// Use this for initialization
	
	void Awake(){
	    minutesToBreath = PrefHolder.newInt("minutesToBreath", 3);
	    difficulty = PrefHolder.newInt("difficulty", 0);
	    sound = PrefHolder.newFloat("sound", 0.5f);
	    graphicsLevel = PrefHolder.newInt("graphicsLevel", 1);
		control = PrefHolder.newInt("control", 0);
    }
	
	void Start () {
		state = State.MENU;
		selectLevel = 0;
		isResume = PlayerPrefs.HasKey("health")? true : false;
		IsFreeVersion = PlayerPrefs.HasKey("IsFreeVersion") ? PlayerPrefs.GetInt("IsFreeVersion") > 0 ? true : false : false;
		openLevel = PlayerPrefs.HasKey("openLevel") ? PlayerPrefs.GetInt("openLevel") : 0;
		
		if(IsFreeVersion && openLevel >= MAX_LEVEL_TRIAL) {
			openLevel = MAX_LEVEL_TRIAL - 1;
		}
		
		InitGUI();
		
		GameMaster.SetGame(false);
        JukeBox.AttachTo(this);
		JukeBox.PlayMenu();
		
		minutesToBreath.Subscribe(OnTap, false);
	    difficulty.Subscribe(OnTap, false);
	    sound.Subscribe(OnTap, false);
	    graphicsLevel.Subscribe(OnTap, false);	    
	}
	
	void InitGUI() {
		int yLayout = IsFreeVersion ? 2 : 4;
		int yOffset = IsFreeVersion ? 84 :102;
		
		if(isResume) {
			rectResume = new Rect((Screen.width - btSize.x) / 2, yOffset, btSize.x, btSize.y);
			yOffset += (int)btSize.y + yLayout;
		} else {
			yOffset += 10;
		}
		
		rectStart = new Rect((Screen.width - btSize.x) / 2, yOffset, btSize.x, btSize.y);
		yOffset += (int)btSize.y + yLayout;		
		rectHighScores = new Rect((Screen.width - btSize.x) / 2, yOffset, btSize.x, btSize.y);
		yOffset += (int)btSize.y + yLayout;
		rectSettings = new Rect((Screen.width - btSize.x) / 2, yOffset, btSize.x, btSize.y);
		yOffset += (int)btSize.y + yLayout;
		rectHelp = new Rect((Screen.width - btSize.x) / 2, yOffset, btSize.x, btSize.y);
		yOffset += (int)btSize.y + yLayout;
		
		if(IsFreeVersion) {
			rectFullVersion = new Rect((Screen.width - btSize.x) / 2, yOffset, btSize.x, btSize.y);
		}
		rectMoreGames = new Rect((Screen.width - btSize.x) / 2, Screen.height - 6 - btSize.y, btSize.x, btSize.y);
	}
	
	void OnTap(object value){
	    JukeBox.Tap();
	}	
	
	void CheckVersion() {
		bool tmp = PlayerPrefs.HasKey("IsFreeVersion") ? PlayerPrefs.GetInt("IsFreeVersion") > 0 ? true : false : false;
		if(IsFreeVersion != tmp) {
			InitGUI();
			IsFreeVersion = tmp;
		}
	}
	
	// Update is called once per frame
	void OnGUI () {
		CheckVersion(); 
		switch(state) {
			case State.MENU :
				if(bgTexture != null) {
					GUI.DrawTexture(new Rect(0,0,Screen.width, Screen.height), bgTexture);
                }

				if(isResume) {				    
					if(GUI.Button(rectResume,"",btResume)) {
					    JukeBox.Tap();
					    StartGame(true);
					}
				}
				if(GUI.Button(rectStart,"",btStart)) {
				    JukeBox.Tap();
                    state = State.LEVELS;
					//StartGame(false);
                }
				if(GUI.Button(rectHighScores,"",btHighScores)) {
				    JukeBox.Tap();
					if(Application.levelCount > 2) {
						state = State.LOADING;
						Application.LoadLevel(Application.levelCount - 1);
					}
				}				
				if(GUI.Button(rectSettings,"",btSettings)) {
				    JukeBox.Tap();
					state = State.SETTINGS;
				}
				if(GUI.Button(rectHelp,"",btHelp)) {
				    JukeBox.Tap();
				    GameMaster.ShowTutorialNextTime();
				    StartGame(false);
				}
				if(IsFreeVersion) {
					if(GUI.Button(rectFullVersion,"",btFullVersion)) {
	                    JukeBox.Tap();                    
	                    PlayerPrefs.SetInt("upgrade", 1);
					}
				}
				if(GUI.Button(rectMoreGames,"",lnMoreGames)) {
                    JukeBox.Tap();                    
                    Application.OpenURL("http://phobos.apple.com/WebObjects/MZSearch.woa/wa/search?submit=seeAllLockups&media=software&entity=software&term=yossi+malki");
				}				
				GameMaster.ShowAd(false); 
				break;				
			case State.SETTINGS :
                GUI.DrawTexture(new Rect(0,0,Screen.width, Screen.height), bgSettings);
			    
    			if(GUI.Button(new Rect(0, 0, 48, 48), "", btBack)) {
    			    JukeBox.Tap();
    				state = State.MENU;
    			}
			    
			    GUI.BeginGroup(new Rect(40, 76, 400, 240));
			
			    GUI.Label(new Rect(0, 0, 380, 24), "BREATH DURATION", labelStyle);
			    minutesToBreath.value = GUI.SelectionGrid(new Rect(0, 24, 400, 24), minutesToBreath - 1, new string[3]{"1 min", "2 min", "3 min"}, 3, controlStyle) + 1;			    

			    GUI.Label(new Rect(0, 48, 380, 24), "DIFFICULTY", labelStyle);
			    difficulty.value = GUI.SelectionGrid(new Rect(0,72, 400, 24), difficulty, new string[3]{"easy", "normal", "hard"}, 3, controlStyle);
			    
				GUI.Label(new Rect(0, 96, 380, 24), "CONTROL", labelStyle);
				control.value = GUI.SelectionGrid(new Rect(0,120, 400, 24), control, new string[2]{"Joystick", "Accelerometer"}, 2, controlStyle);
			
			    GUI.Label(new Rect(0, 144, 380, 24), "VOLUME", labelStyle);
			    sound.value = GUI.HorizontalSlider(new Rect(0,168, 400, 24), sound, 0.0f, 1.0f);
			    AudioListener.volume = sound;
			    
			    GUI.Label(new Rect(0, 192, 380, 24), "GRAPHICS LEVEL", labelStyle);
			    graphicsLevel.value = GUI.SelectionGrid(new Rect(0,216, 400, 24), graphicsLevel, new string[2]{"low", "high"}, 2, controlStyle);
			    
			    GUI.EndGroup();
				GameMaster.ShowAd(false);
			    break;
			case State.LOADING :
				if(bgTexture != null) { 
					GUI.DrawTexture(new Rect(0,0,Screen.width, Screen.height), bgTexture);
				}
				if(labelLoading != null) {
					GUI.DrawTexture(new Rect((Screen.width - 256)/2,(Screen.height - 256)/2, 256, 256), labelLoading);
				}
				GameMaster.SetGame(false);
				break;
			case State.LEVELS :
				if(bgTexture != null) {
					GUI.DrawTexture(new Rect(0,0,Screen.width, Screen.height), bgTexture);
                }
				
				if(GUI.Button(new Rect(0, 0, 48, 48), "", btBack)) {
    			    JukeBox.Tap();
    				state = State.MENU;
    			}
				int level = 0;
				int btLayout = 8;
				for(int cols = 0; cols < 3; cols++) {
					for(int rows = 0; rows < 3; rows++, level++) {
						if(level <= openLevel) {
							if(GUI.Button(new Rect(20 + cols * (btLevelSize.x + btLayout), 100 + rows * (btLevelSize.y + btLayout), btLevelSize.x, btLevelSize.y), "Level " + (level + 1), btLevel)) {
								JukeBox.Tap();
								state = State.OBJECTIVES;
								selectLevel = level; 
							}
						} else {
							GUI.Label(new Rect(20 + cols * (btLevelSize.x + btLayout), 100 + rows * (btLevelSize.y + btLayout), btLevelSize.x, btLevelSize.y), "Level " + (level + 1), btNoActiveLevel);
						}
					}
				}
				if(IsFreeVersion) {
					if(GUI.Button(new Rect((Screen.width - btSize.x)/2, Screen.height - btSize.y - 20 - 50, btSize.x, btSize.y),"",btFullVersion)) {
	                    JukeBox.Tap();                    
	                    PlayerPrefs.SetInt("upgrade", 1);
					}
				} else {
					if(level <= openLevel) {
						if(GUI.Button(new Rect(20 + btLevelSize.x + btLayout, 100 + 3 * (btLevelSize.y + btLayout), btLevelSize.x, btLevelSize.y), "Level " + (level + 1), btLevel)) {
							JukeBox.Tap();
							state = State.OBJECTIVES;
							selectLevel = level; 
						}
					} else {
						GUI.Label(new Rect(20 + btLevelSize.x + btLayout, 100 + 3 * (btLevelSize.y + btLayout), btLevelSize.x, btLevelSize.y), "Level " + (level + 1), btNoActiveLevel);
					}
				}
				GameMaster.ShowAd(true);
				break;
			case State.OBJECTIVES :
				if(bgTexture != null) {
					GUI.DrawTexture(new Rect(0,0,Screen.width, Screen.height), bgTexture);
                }
				
				if(GUI.Button(new Rect(0, 0, 48, 48), "", btBack)) {
    			    JukeBox.Tap();
    				state = State.LEVELS;
    			}
                GUI.Label(new Rect(30,100, Screen.width - 60, 140),"objectives :", objectivesTitleStyle);
                GUI.Label(new Rect(40,140,Screen.width - 80, Screen.height - 200), objectives[selectLevel], objectivesStyle);
				
				if(GUI.Button(new Rect((Screen.width - btSize.x)/2, Screen.height - btSize.y - 20 - (IsFreeVersion ? 50 : 0), btSize.x, btSize.y), "", btStartLevel)) {
					JukeBox.Tap();
					StartGame(false);
				}
				GameMaster.SetGame(true);
				break;
		}
	}
	
	void StartGame(bool isResume){
		if(Application.levelCount > 1) {
		    if(!isResume) {
				CleanPlayerPrefs();
				PlayerPrefs.SetInt("level", selectLevel);
			}
			state = State.LOADING;
			Application.LoadLevel(1);
		}	    
	}
	
	public static bool OpenNewLevel(int arg) {
		int maxLevel = (PlayerPrefs.HasKey("IsFreeVersion") ? PlayerPrefs.GetInt("IsFreeVersion") > 0 ? true : false : false) ? MAX_LEVEL_TRIAL : MAX_LEVEL;
		if(arg < maxLevel) {
			int cuurentOpenLevel = PlayerPrefs.HasKey("openLevel") ? PlayerPrefs.GetInt("openLevel") : 0;
			if(cuurentOpenLevel < arg) {
				PlayerPrefs.SetInt("openLevel", arg);
			}
			return true;
		} else {
			return false;
		}
	}
	
	public static void CleanPlayerPrefs() {
		PlayerPrefs.DeleteKey("player");
		PlayerPrefs.DeleteKey("health");
		PlayerPrefs.DeleteKey("timer");
		PlayerPrefs.DeleteKey("transform");
		PlayerPrefs.DeleteKey("air");
		PlayerPrefs.DeleteKey("lives");
		PlayerPrefs.DeleteKey("benchMark");
		PlayerPrefs.DeleteKey("fishes");
		PlayerPrefs.DeleteKey("tasksTimer");
		PlayerPrefs.DeleteKey("tasksFishes");
		PlayerPrefs.DeleteKey("level");
	}
}
