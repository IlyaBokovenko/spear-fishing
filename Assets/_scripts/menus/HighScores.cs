using UnityEngine;
using System.Collections;

public class HighScores : MonoBehaviour {
	public Texture2D bgTexture;
	public int maxPlayer = 5;
	public GUIStyle textStyle;
	public GUIStyle btBack;
	private PlayerScore[] players;
	private iPhoneKeyboard keyboard;
	private int newPlayerPosition = 0;
	private string text = "";
	
	public AudioClip menuTap;
	
	void Start () {
        // iPhoneKeyboard.autorotateToPortrait = false;
        //         iPhoneKeyboard.autorotateToPortraitUpsideDown  = false;
        //         iPhoneKeyboard.autorotateToLandscapeRight  = false;
        //         iPhoneKeyboard.autorotateToLandscapeLeft = true;
	    
		players = new PlayerScore[maxPlayer];
		int index = 0;
		string key = "";
		while(index < maxPlayer) {
			key = "player" + index;
			if(PlayerPrefs.HasKey(key)) {
				players[index] = new PlayerScore(PlayerPrefs.GetString(key));
			} else {
				players[index] = new PlayerScore();
			}
			players[index].id = index;
			players[index].rect = new Rect(120, 70 + index * 24, 300, 24);
			players[index].textStyle = textStyle;
			index++;
		}
		if(PlayerPrefs.HasKey("player")) {
			PlayerScore newPlayer = new PlayerScore(PlayerPrefs.GetString("player"));
			newPlayerPosition = maxPlayer;
			for(index = maxPlayer - 1; index >= 0 ; index--) {
				if(players[index].getWeight() < newPlayer.getWeight()) {
					if((index + 1) < maxPlayer) {
						players[index + 1].Copy(players[index]);
					}
					players[index].Copy(newPlayer);
					newPlayerPosition = index;
				}
			}
			if(newPlayerPosition < maxPlayer)
				keyboard = iPhoneKeyboard.Open(players[newPlayerPosition].name, iPhoneKeyboardType.Default);
			MainMenu.CleanPlayerPrefs();
		}
		
		GameMaster.SetGame(false);
	}
	
	void OnGUI () {
		if(bgTexture)
			GUI.DrawTexture(new Rect(0,0,Screen.width, Screen.height), bgTexture);
		
		foreach(PlayerScore player in players) {
			player.Show();
		}
		
		if(keyboard != null)
			players[newPlayerPosition].name = keyboard.text;
		
		if(GUI.Button(new Rect(0, 0, 48, 48), "", btBack)) {
		    JukeBox.Tap();
			Save();
			Application.LoadLevel(0);
		}		
	}
	
	void Save() {
	    for(int index = 0; index < maxPlayer; index++) {
			PlayerPrefs.SetString("player" + index, players[index].getData());
		}
	}
	
	void OnApllicationQuit() {
		Save();
	}
	
	public class PlayerScore {
		public Rect rect;
		public GUIStyle textStyle;
		public int id;
		public string name;
		public string count;
		public string weight;
		
		public PlayerScore() {
			name = "None";
			count = "0";
			weight = "0";
		}
		
		public PlayerScore(string arg) {
			string[] param = arg.Split(":"[0]);
			name = param[0] != null ? param[0] : "None";
			count = param[1] != null ? param[1] : "0";
			weight = param[2] != null ? param[2] : "0";
		}
		
		public void Show() {
			GUI.Label(new Rect(rect.x, rect.y, rect.width - 150, rect.height), (id + 1) + ". " + name, textStyle);
			GUI.Label(new Rect(rect.x + (rect.width - 150), rect.y, 60, rect.height), count, textStyle);
			GUI.Label(new Rect(rect.x + (rect.width - 90), rect.y, 90, rect.height), string.Format("{0,1:00.0}", float.Parse(weight)), textStyle);
		}
		
		public float getWeight() {
			return float.Parse(weight);
		}
		
		public void Copy(PlayerScore arg) {
			name = arg.name;
			count = arg.count;
			weight = arg.weight;
		}
		
		public string getData() {
			return name + ":" + count + ":" + weight;
		}
	}
}
