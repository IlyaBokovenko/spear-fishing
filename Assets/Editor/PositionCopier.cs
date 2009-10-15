using UnityEngine;
using UnityEditor;
using System.Collections;
 
public class PositionCopier : ScriptableObject
{
        private static Vector3 position;
        private static Quaternion rotation;
        private static Vector3 scale;
        private static string myName; 
    
    [MenuItem ("Custom/Position Copier/Copy Position")]
    static void DoRecord()
    {
       position = Selection.activeTransform.localPosition;
    }
 
    [MenuItem ("Custom/Position Copier/Paste Position")]
    static void DoApply()
    {
        Selection.activeTransform.localPosition = position;
    }
}