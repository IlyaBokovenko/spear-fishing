using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Nose))]
public class GenericSeekingBehaviour : FishBehaviour {
    public VelocityMatching velocityMatcher;
    public OrientationMatching orientationMatcher;    
    
    private GameObject _target;
    public GameObject target
    {
        get{return _target;}
        set{_target = value; targetTransform = value.transform;}
    }
    
    public Vector3 center
    {
        get{return _transform.position;}
    }
    
    private Vector3 noseRelative;
    public Vector3 nose
    {
        get{return _transform.TransformPoint(noseRelative);}
    }
    
    public float maxSpeed = 2;    

    protected Transform _transform;
    protected Transform targetTransform;
    
    protected virtual void Awake(){
        children = new FishBehaviour[2]{velocityMatcher, orientationMatcher};
        noseRelative = ((Nose)GetComponent(typeof(Nose))).position;        
        _transform = transform;
    }
    
    public virtual Vector3 From(){        
        return center;
    }
    public virtual Vector3 To(){
        return targetTransform.position;
    }
    
    public virtual Vector3 Direction(){
        return (To() - From()).normalized;
    }    

    protected override void PrivateDrawGizmosSelected(){
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(targetTransform.position, 0.1f);
        Gizmos.DrawLine(From(), To());        
    }
}