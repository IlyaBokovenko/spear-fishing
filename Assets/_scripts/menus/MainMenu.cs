using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {
	public Texture2D bgTexture;
	public Texture2D bgHelp;
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
	
	//Rects
	private Vector2 btSize = new Vector2(238,32);
	private Rect rectResume;
	private Rect rectStart;
	private Rect rectSettings;
	private Rect rectHighScores;
	private Rect rectBenchMark;
	private Rect rectHelp;
	private Rect rectMoreGames;
	
	public static int MENU = 0;
	public static int SETTINGS = 1;
	public static int HELP = 2;
	public static int LOADING = 3;
	
	private int state;
	//Test
	private bool isResume;
	private int count;
	// Use this for initialization
	
	void Start () {
		int yLayout = 134;
		state = MENU;
		isResume = PlayerPrefs.HasKey("health")? true : false;
		
		if(isResume) {
			rectResume = new Rect((Screen.width - btSize.x) / 2, yLayout, btSize.x, btSize.y);
			yLayout += (int)btSize.y + 4;
		}
		rectStart = new Rect((Screen.width - btSize.x) / 2, yLayout, btSize.x, btSize.y);
		yLayout += (int)btSize.y + 4;
		//rectSettings = new Rect((Screen.width - btSize.x) / 2, yLayout, btSize.x, btSize.y);
		rectHighScores = new Rect((Screen.width - btSize.x) / 2, yLayout, btSize.x, btSize.y);
		yLayout += (int)btSize.y + 4;
		rectHelp = new Rect((Screen.width - btSize.x) / 2, yLayout, btSize.x, btSize.y);
		//rectBenchMark = new Rect((Screen.width - btSize.x) / 2, yLayout, btSize.x, btSize.y);
		rectMoreGames = new Rect((Screen.width - btSize.x) / 2, Screen.height - 6 - btSize.y, btSize.x, btSize.y);
		
		 PlayerPrefs.SetInt("game", 0);
	}
	// Update is called once per frame
	void OnGUI () {
		switch(state) {
			case 0 :
				if(bgTexture != null) 
					GUI.DrawTexture(new Rect(0,0,Screen.width, Screen.height), bgTexture);

				if(isResume) {
					if(GUI.Button(rectResume,"",btResume)) {
						if(Application.levelCount > 1) {
							state = LOADING;
							Application.LoadLevel(1);
						}
					}
				}
				if(GUI.Button(rectStart,"",btStart)) {
					if(Application.levelCount > 1) {
						CleanPlayerPrefs();
						state = LOADING;
						Application.LoadLevel(1);
					}
				}
				/*
				if(GUI.Button(rectSettings,"",btSettings)) {
					
				}*/
				if(GUI.Button(rectHighScores,"",btHighScores)) {
					if(Application.levelCount > 2) {
						state = LOADING;
						Application.LoadLevel(Application.levelCount - 1);
					}
				}
				if(GUI.Button(rectHelp,"",btHelp)) {
					state = HELP;
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

				}
				break;
			case 2 :
				if(bgHelp) {
					GUI.DrawTexture(new Rect(0,0,Screen.width, Screen.height), bgHelp);
				}
				if(GUI.Button(new Rect(Screen.width - 68,Screen.height - 68,68,68), "", btBack)) {
					state = MENU;
				}
				break;
			case 3 :
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
