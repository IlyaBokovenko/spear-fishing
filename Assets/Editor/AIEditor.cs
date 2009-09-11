using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(PredatorFishAI))]
public class AIEditor : Editor
{
    void OnSceneGUI()
    {
        GameObject obj = ((Component)target).gameObject;
        string msg = "";
        foreach(FishBehaviour beh in obj.GetComponents(typeof(FishBehaviour))){
            if(beh.enabled)
                msg += beh.ToString() + "\n";            
        }     
        
        Handles.Label(obj.transform.position, msg);
    }
}