using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SunRay : MonoBehaviour {
    
    private LineRenderer lineRenderer;
	// Use this for initialization
	void Start () {
	    lineRenderer = (LineRenderer)(GetComponent(typeof(LineRenderer)));
	    Transform _transform = transform;
	    Vector3 pos = _transform.position;
	    pos.y = 3;
	    lineRenderer.SetPosition(0, pos);	    
	    pos.y = -6;
	    pos.x += 6;	    
	    lineRenderer.SetPosition(1, pos);
	}
	
	// Update is called once per frame
	void Update () {
	}
}
