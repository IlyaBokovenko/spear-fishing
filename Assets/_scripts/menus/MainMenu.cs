using UnityEngine;
using System.Collections;

// [ExecuteInEditMode]
public class MainMenu : MonoBehaviour {
	public Texture2D bgTexture;
	public Texture2D bgSettings;
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
	
	public GUIStyle controlStyle;
	public GUIStyle labelStyle;
	
	public AudioClip menuTap;
	
	private PrefHolder minutesToBreath;	
	private PrefHolder difficulty;	
	private PrefHolder sound;
	private PrefHolder graphicsLevel;
	
	//Rects
	private static Vector2 btSize = new Vector2(238,32);
	private Rect rectResume;
	private Rect rectStart;
	private Rect rectHighScores;
	private Rect rectSettings;	
	private Rect rectBenchMark;
	private Rect rectHelp;
	private Rect rectMoreGames;
	
	enum State
	{
        MENU,
        SETTINGS,
        LOADING	    
	}
	
	private State state;
	//Test
	private bool isResume;
	private int count;
	
	// Use this for initialization
	
	void Awake(){
	    minutesToBreath = PrefHolder.newInt("minutesToBreath", 2);
	    difficulty = PrefHolder.newInt("difficulty", 1);
	    sound = PrefHolder.newFloat("sound", 0.5f);
	    graphicsLevel = PrefHolder.newInt("graphicsLevel", 1);
	    
	}
	
	void Start () {
		int yLayout = 102;
		state = State.MENU;
		isResume = PlayerPrefs.HasKey("health")? true : false;
		
		if(isResume) {
			rectResume = new Rect((Screen.width - btSize.x) / 2, yLayout, btSize.x, btSize.y);
			yLayout += (int)btSize.y + 4;
		}
		rectStart = new Rect((Screen.width - btSize.x) / 2, yLayout, btSize.x, btSize.y);
		yLayout += (int)btSize.y + 4;		
		rectHighScores = new Rect((Screen.width - btSize.x) / 2, yLayout, btSize.x, btSize.y);
		yLayout += (int)btSize.y + 4;
		rectSettings = new Rect((Screen.width - btSize.x) / 2, yLayout, btSize.x, btSize.y);
		yLayout += (int)btSize.y + 4;
		rectHelp = new Rect((Screen.width - btSize.x) / 2, yLayout, btSize.x, btSize.y);
		//rectBenchMark = new Rect((Screen.width - btSize.x) / 2, yLayout, btSize.x, btSize.y);
		rectMoreGames = new Rect((Screen.width - btSize.x) / 2, Screen.height - 6 - btSize.y, btSize.x, btSize.y);
		
		GameMaster.SetGame(false);
        JukeBox.AttachTo(this);
		JukeBox.PlayMenu();
		
		minutesToBreath.Subscribe(OnTap, false);
	    difficulty.Subscribe(OnTap, false);
	    sound.Subscribe(OnTap, false);
	    graphicsLevel.Subscribe(OnTap, false);	    
	}
	
	void OnTap(object value){
	    JukeBox.Tap();
	}	
	
	// Update is called once per frame
	void OnGUI () {

   
        // controlStyle = new GUIStyle(GUI.skin.button);
   
        // labelStyle = new GUIStyle(controlStyle);
	    
		switch(state) {
			case State.MENU :
				if(bgTexture != null) 
					GUI.DrawTexture(new Rect(0,0,Screen.width, Screen.height), bgTexture);

				if(isResume) {				    
					if(GUI.Button(rectResume,"",btResume)) {
					    JukeBox.Tap();
					    StartGame(true);
					}
				}
				if(GUI.Button(rectStart,"",btStart)) {
				    JukeBox.Tap();
                    StartGame(false);
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
				if(GUI.Button(rectMoreGames,"",lnMoreGames)) {
                    JukeBox.Tap();                    
                    Application.OpenURL("http://phobos.apple.com/WebObjects/MZSearch.woa/wa/search?submit=seeAllLockups&media=software&entity=software&term=yossi+malki");
				}				
				break;				
			case State.SETTINGS :
                GUI.DrawTexture(new Rect(0,0,Screen.width, Screen.height), bgSettings);
			    
    			if(GUI.Button(new Rect(0, 0, 48, 48), "", btBack)) {
    			    JukeBox.Tap();
    				state = State.MENU;
    			}
			    
			    GUI.BeginGroup(new Rect(40, 80, 400, 240));
			
			    GUI.Label(new Rect(0, 0, 380, 24), "BREATH DURATION", labelStyle);
			    minutesToBreath.value = GUI.SelectionGrid(new Rect(0,30, 400, 24), minutesToBreath - 1, new string[3]{"1 min", "2 min", "3 min"}, 3, controlStyle) + 1;			    

			    GUI.Label(new Rect(0, 60, 380, 24), "DIFFICULTY", labelStyle);
			    difficulty.value = GUI.SelectionGrid(new Rect(0,90, 400, 24), difficulty, new string[3]{"easy", "normal", "hard"}, 3, controlStyle);
			    
			    GUI.Label(new Rect(0, 120, 380, 24), "VOLUME", labelStyle);
			    sound.value = GUI.HorizontalSlider(new Rect(0,150, 400, 24), sound, 0.0f, 1.0f);
			    AudioListener.volume = sound;
			    
			    GUI.Label(new Rect(0, 170, 380, 24), "GRAPHICS LEVEL", labelStyle);
			    graphicsLevel.value = GUI.SelectionGrid(new Rect(0,200, 400, 24), graphicsLevel, new string[2]{"low", "high"}, 2, controlStyle);
			    
			    GUI.EndGroup();
			    
			    break;
			case State.LOADING :
				if(bgTexture != null) 
					GUI.DrawTexture(new Rect(0,0,Screen.width, Screen.height), bgTexture);
				break;
		}
		
	}
	
	void StartGame(bool isResume){
		if(Application.levelCount > 1) {
		    if(!isResume) CleanPlayerPrefs();
			state = State.LOADING;
			Application.LoadLevel(1);
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
		int index = 0;
		string key = "";
		while(true) {
			key = "fish" + (index++);
			if(PlayerPrefs.HasKey(key)) {
				PlayerPrefs.DeleteKey(key);
			} else {
				break;
			}
		}
	}
}
