using UnityEngine;
using System.Collections;
using UnityEditor;

// [CustomEditor(typeof(FishAI))]
public class AIEditor
{
    // void OnSceneGUI()
    // {   
    //     FishAI ai = (FishAI)target;                       
    //     
    //     if(ai.activeBehaviours == null)
    //         return;
    //                     
    //     string msg = "";        
    //     foreach(FishBehaviour beh in ai.activeBehaviours){
    //         if(!beh)
    //             continue;
    //         if(beh.enabled)
    //             msg += beh.ToStringWithChildren() + "\n";            
    //     }     
    //     Handles.Label(ai.gameObject.transform.position + Vector3.right * 2, msg);                                
    // }    
}

// [CustomEditor(typeof(PredatorFishAI))]
// public class PredatorAIEditor : AIEditor{    
// }
// 
// [CustomEditor(typeof(PreyFishAI))]
// public class PreyAIEditor : AIEditor{    
// }