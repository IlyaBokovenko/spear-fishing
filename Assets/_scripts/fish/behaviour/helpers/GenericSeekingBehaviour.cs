using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Nose))]
public class GenericSeekingBehaviour : FishBehaviour {
    public VelocityMatching velocityMatcher;
    public OrientationMatching orientationMatcher;    
    
    public float maxSpeed = 2;      
    
    private GameObject _target;
    protected Transform _transform;
    public GameObject target
    {
        get{return _target;}
        set{_target = value; targetTransform = value.transform;}
    }
    protected Transform targetTransform;
    
    public Vector3 center
    {
        get{return _transform.position;}
    }
    
    private Vector3 noseRelative;
    public Vector3 nose
    {
        get{return _transform.TransformPoint(noseRelative);}
    }
    
    
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
    
    public virtual float Distance(){
        return Vector3.Distance(To(), From());
    }   

    protected override void PrivateDrawGizmosSelected(){
        if(targetTransform == null)
            return;
            
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(targetTransform.position, 0.1f);
        Gizmos.DrawLine(From(), To());        
    }
    
}