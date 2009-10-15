using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SunRay : MonoBehaviour {
    
    private LineRenderer renderer;
	// Use this for initialization
	void Start () {
	    renderer = (LineRenderer)(GetComponent(typeof(LineRenderer)));
	    Transform _transform = transform;
	    Vector3 pos = _transform.position;
	    pos.y = 3;
	    renderer.SetPosition(0, pos);	    
	    pos.y = -6;	    
	    renderer.SetPosition(1, pos);
	}
	
	// Update is called once per frame
	void Update () {
	}
}
