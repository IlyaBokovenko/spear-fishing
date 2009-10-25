using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {
    public float driftPeriod = 10f;
    public float rotationPeriod = 3f;
    public float heightDrift = 0.1f;
    public float reappearInterval = 180;
    
    private Transform _transform;
    
    private bool isAnimationEnabled = true;

	// Use this for initialization
	void Start () {
	    _transform = transform;
	    _transform.rotation = Quaternion.identity;
	    
	    isAnimationEnabled = PlayerPrefs.GetInt("graphicsLevel", 1) > 0;
	}
	
    void FixedUpdate(){
        if(isAnimationEnabled)  Animate();
    }
    
    public void Yam(){
        gameObject.active = false;
        Invoke("Reappear", reappearInterval);
    }
    
    private void Reappear(){
        gameObject.active = true;
    }
    
    private void Animate(){
        float driftPhase = 2 * Mathf.PI * Time.time / driftPeriod;
        float heightShift = heightDrift * Mathf.Cos(driftPhase);
        _transform.localPosition += new Vector3(0, heightShift * Time.deltaTime, 0);
        
        float rotationPhase = 360 * Time.time / rotationPeriod;
        _transform.rotation = Quaternion.Euler(0, rotationPhase, 0);        
    }
}
