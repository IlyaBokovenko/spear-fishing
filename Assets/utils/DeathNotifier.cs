using UnityEngine;
using System.Collections;

public class DeathNotifier : GenericScript {
    private static Hashtable objectsByName = new Hashtable();

    public Component notefee;
    
    private string objName;
    
    void Awake(){
        objName = gameObject.name;
    }
    

	void OnDestroyGameObject() // see GenericScript
     {         
            if(Application.isPlaying){
                notefee.SendMessage("OnObjectDied", objName);
            }
     }
}
