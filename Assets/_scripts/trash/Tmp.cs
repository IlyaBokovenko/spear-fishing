using UnityEngine;
using System.Collections;

public class Tmp : MonoBehaviour {
    
    PrefHolder vh;

	void Start () {
	    print("qweqwe".GetHashCode());
	    print("qweqwe".GetHashCode());
	}
	
	void OnChanged(object value){
	    int v = (int)value;
	    print(v);
	}
	
	void Update () {
	    
	}
}
