    using UnityEngine;
using System.Collections;

// [ExecuteInEditMode]
public class MainMenu : MonoBehaviour {
	public Texture2D bgTexture;
	public Texture2D bgHelp;
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
	
	private int minutesToBreath
	{
	    get{return PlayerPrefs.GetInt("minutesToBreath", 2);}
	    set{PlayerPrefs.SetInt("minutesToBreath", value);}
	}
	
	private int difficulty	
	{
	    get{return PlayerPrefs.GetInt("difficulty", 1);}
	    set{PlayerPrefs.SetInt("difficulty", value);}
	}
	
	private float sound
	{
	    get{return PlayerPrefs.GetFloat("sound", 0.5f);}
	    set{PlayerPrefs.SetFloat("sound", value);}
	}
	
	private int graphicsLevel
	{
	    get{return PlayerPrefs.GetInt("graphicsLevel", 1);}
	    set{PlayerPrefs.SetInt("graphicsLevel", value);}
	}
	
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
        HELP,
        LOADING	    
	}
	
	private State state;
	//Test
	private bool isResume;
	private int count;
	
	// Use this for initialization
	
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
						if(Application.levelCount > 1) {
							state = State.LOADING;
							Application.LoadLevel(1);
						}
					}
				}
				if(GUI.Button(rectStart,"",btStart)) {
				    JukeBox.Tap();
					if(Application.levelCount > 1) {
						CleanPlayerPrefs();
						state = State.LOADING;
						Application.LoadLevel(1);
					}
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
					state = State.HELP;
				}
				/*
				if(GUI.Button(rectBenchMark,"",btBenchMark)) {
					if(Application.levelCount > 1) {
						CleanPlayerPrefs();
						state = LOADING;
						PlayerPrefs.SetInt("benchMark", 1);
						Application.LoadLevel(1);
					}
				}*/
				if(GUI.Button(rectMoreGames,"",lnMoreGames)) {
                    JukeBox.Tap();
				}				
				break;				
			case State.SETTINGS :
                GUI.DrawTexture(new Rect(0,0,Screen.width, Screen.height), bgSettings);
			    
    			if(GUI.Button(new Rect(0, 0, 48, 48), "", btBack)) {
    			    JukeBox.Tap();
    				state = State.MENU;
    			}
			    
			    GUI.BeginGroup(new Rect(40, 80, 400, 240));
			
			    GUI.Label(new Rect(0, 0, 380, 24), "BREATH TIME", labelStyle);
			    minutesToBreath = GUI.SelectionGrid(new Rect(0,30, 400, 24), minutesToBreath - 1, new string[3]{"1 min", "2 min", "3 min"}, 3, controlStyle) + 1;			    

			    GUI.Label(new Rect(0, 60, 380, 24), "DIFFICULTY", labelStyle);
			    difficulty = GUI.SelectionGrid(new Rect(0,90, 400, 24), difficulty, new string[3]{"easy", "normal", "hard"}, 3, controlStyle);
			    
			    GUI.Label(new Rect(0, 120, 380, 24), "VOLUME", labelStyle);
			    sound = GUI.HorizontalSlider(new Rect(0,150, 400, 24), sound, 0.0f, 1.0f);
			    AudioListener.volume = sound;
			    
			    GUI.Label(new Rect(0, 170, 380, 24), "GRAPHICS LEVEL", labelStyle);
			    graphicsLevel = GUI.SelectionGrid(new Rect(0,200, 400, 24), graphicsLevel, new string[2]{"low", "high"}, 2, controlStyle);
			    
			    GUI.EndGroup();
			    
			    break;
			case State.HELP :
				if(bgHelp) {
					GUI.DrawTexture(new Rect(0,0,Screen.width, Screen.height), bgHelp);
				}
				if(GUI.Button(new Rect(0, 0, 48, 48), "", btBack)) {
				    JukeBox.Tap();
					state = State.MENU;
				}
				break;
			case State.LOADING :
				if(bgTexture != null) 
					GUI.DrawTexture(new Rect(0,0,Screen.width, Screen.height), bgTexture);
				break;
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
