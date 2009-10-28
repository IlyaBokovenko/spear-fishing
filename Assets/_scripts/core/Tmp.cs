using UnityEngine;
using System.Collections;

public class Tmp : MonoBehaviour {
    
    PrefHolder vh;

	void Start () {
        vh = PrefHolder.newBool("test", false);
        //         vh.Subscribe(this.OnChanged);
        //         vh.value = 13;
        //         vh.value = 14;
        //         vh.Unsubscribe(this.OnChanged);
        //         vh.value = 15;
        // print(PlayerPrefs.GetInt("test", -1));
        vh.value = false;
        print(vh.value);
        vh.value = true;
        print(vh.value);
	}
	
	void OnChanged(object value){
	    int v = (int)value;
	    print(v);
	}
	
	void Update () {
	    
	}
}
