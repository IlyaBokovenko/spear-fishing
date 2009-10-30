using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {
    public float reappearInterval = 180;
    
    private Transform _transform;
    
    private bool isAnimationEnabled = true;

	void Start () {
	}
	
    public void Yam(){        
        gameObject.SetActiveRecursively(false);
        Invoke("Reappear", reappearInterval);
    }
    
    private void Reappear(){
        gameObject.SetActiveRecursively(true);
    }    
}
