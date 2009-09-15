using UnityEngine;
using System.Collections;

public class DeathNotifier : MonoBehaviour {
    private static Hashtable objectsByName = new Hashtable();

    public Component notefee;
    
    private string objName;
    
    void Awake(){
        objName = gameObject.name;
    }
    
	
	 ~DeathNotifier(){
        if(Application.isPlaying)
	        notefee.SendMessage("OnObjectDied", objName, SendMessageOptions.DontRequireReceiver);
	 }
}
