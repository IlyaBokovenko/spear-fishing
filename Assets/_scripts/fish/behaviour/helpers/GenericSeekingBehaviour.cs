using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Nose))]
public class GenericSeekingBehaviour : FishBehaviour {
    public VelocityMatching velocityMatcher;
    public OrientationMatching orientationMatcher;    
    
    public GameObject target;
    public float maxSpeed = 2;
    
    protected Vector3 nose;
    protected Transform _transform;
    
    protected virtual void Awake(){
        children = new FishBehaviour[2]{velocityMatcher, orientationMatcher};
        nose = ((Nose)GetComponent(typeof(Nose))).position;        
        _transform = transform;
    }
    
    public Vector3 DirectionFromNose(){
        Vector3 from = _transform.TransformPoint(nose);
        Vector3 to = target.transform.position;
        Vector3 direction = (to - from).normalized;
        return direction;                
    }
    
    public Vector3 DirectionFromCenter(){
        Vector3 from = _transform.position;
        Vector3 to = target.transform.position;
        Vector3 direction = (to - from).normalized;
        return direction;                
    }

    public virtual Vector3 Direction(){
        return DirectionFromCenter();
    }   
}