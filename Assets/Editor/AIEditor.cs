using UnityEngine;
using System.Collections;
using UnityEditor;
 
public class AIEditor : EditorWindow {    
    
    private static AIEditor instance;
    
    [MenuItem("Window/AI")]
    static void ShowWindow () {
        if(!instance)
            instance = CreateInstance();        
            
        instance.Show();
    }
    
    private static AIEditor CreateInstance(){
        AIEditor editor = new AIEditor();
        editor.autoRepaintOnSceneChange = true;        
        return editor;        
    }    
        
    void OnGUI(){
        GameObject obj = Selection.activeGameObject;
        if(!obj)
            return;
        
        FishAI ai = (FishAI)obj.GetComponent(typeof(FishAI));
        if(!ai)
            return;            
        
        GUILayout.Label(ai.BehavioursDescription());        
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
