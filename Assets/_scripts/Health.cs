using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {
    public float driftPeriod = 10f;
    public float rotationPeriod = 3f;
    public float heightDrift = 0.1f;

	// Use this for initialization
	void Start () {
	    transform.rotation = Quaternion.identity;
	}
	
    void FixedUpdate(){
        float driftPhase = 2 * Mathf.PI * Time.time / driftPeriod;
        float heightShift = heightDrift * Mathf.Cos(driftPhase);
        transform.localPosition += new Vector3(0, heightShift * Time.deltaTime, 0);
        
        float rotationPhase = 360 * Time.time / rotationPeriod;
        transform.rotation = Quaternion.Euler(0, rotationPhase, 0);
    }
}
