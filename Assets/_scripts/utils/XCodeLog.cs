using UnityEngine;
using System.Collections;

public class XCodeLog {
    public static void Log(object some){
        string log = PlayerPrefs.GetString("log");
        log += some + "\n";
        PlayerPrefs.SetString("log", log);
    }

}
