using UnityEngine;
using System.Collections;

public class InstanceCounter : MonoBehaviour {
    private static Hashtable objectsByName = new Hashtable();

    private int objId;
    string objName;
    
	void Awake () {
	    objId = gameObject.GetInstanceID();
	    objName = gameObject.name;
	    
	    if(objId == 0) 
	        return;

	    Hashtable sameObjects = (Hashtable)objectsByName[objName];
	    if(sameObjects == null){
	        sameObjects = new Hashtable();
	        objectsByName.Add(objName, sameObjects);
	    }	        
	    
	    sameObjects.Add(objId, 0);
	    print("object " + objName + " added");
	}
	
	 ~InstanceCounter(){
 	    if(objId == 0) 
 	        return;
	    
 	    Hashtable sameObjects = (Hashtable)objectsByName[objName]; 	    
 	    if(sameObjects ==  null || !sameObjects.ContainsKey(objId))
	        Debug.LogError("object named" + objName + " were destroyed without construction");
	        
	    sameObjects.Remove(objId);	    
	    print("object " + objName + " removed");
	 }
	 
	 public int numberOfObjectsLike(GameObject obj){
	    Hashtable sameObjects = (Hashtable)objectsByName[obj.name];
	    return sameObjects != null ? sameObjects.Count : 0;
	 }
	
}
