using UnityEngine;
using System.Collections;
using System;

[RequireComponent (typeof (Nose))]
public class FishObstacleAvoidingBehaviour : FishArbitratedBehaviour {   

    public GenericSeekingBehaviour seeking;
    
    public float timeToThinkAhead = 3f;
    public float minDistance = 2f;

    public float whiskersAngle = 30f;    
    public float whiskersLength = 1.0f;
    public float timeToKeepAvoiding = 0.5f;
    
    public float  raycastPeriod = 0.3f;
    private float lastCastTime = 0.0f;

    private Transform _transform;
    private Transform seekingTargetTransform;
    
    enum State{
        Idle,
        Avoiding,
        PostAvoiding,
        Turning
    }
    
    private State state = State.Idle;
    private float lastCollisionTime = 0.0f;            
    
    private GameObject seekingTarget;
    
    private bool isCollided = false;
    private RaycastHit hit;
    
    private Line[] whiskers;
    
    private SteeringOutput steering;
    
    private int obstaclesLayerMask;
    
    public LayerMask ignoreAvoiding;
    
    private Vector3 nose;
    
    FishObstacleAvoidingBehaviour(){
        priority = 2;
        
    } 
    
    protected override ArrayList ActiveChildren(){
        if(state != State.Idle)
            return base.ActiveChildren();
        else
            return new ArrayList();
    }
    
    public override string ToString(){
        return base.ToString() + " : " + Enum.GetName(typeof(State), state);
    }
            
    void Awake(){
        Nose _nose  = (Nose)GetComponent(typeof(Nose)); 
        nose = _nose.position;	    
        children = new FishBehaviour[1]{seeking};
        _transform = transform;
        seekingTarget = new GameObject("collision avoidance target");
        seekingTargetTransform = seekingTarget.transform;   
	    seeking.target = seekingTarget;	    
    }
	
	void Start () {	    
	    obstaclesLayerMask = ~ignoreAvoiding.value;	    

	    whiskers = CreateWhiskers();
	
		lastCastTime = Time.time;
	}
	
	public override SteeringOutput GetSteering(){
	    Profiler.StartProfile(PT.CollisionAvoiding);
	    
	    if(!seeking)
	        steering = SteeringOutput.empty;
	    else{
            TryCheckCollisions();
            ChangeState();
            ProcessState();
	    }        
        
        Profiler.EndProfile(PT.CollisionAvoiding);
        
        return steering;        
	}
	
	public override void SelfDestroy(){
	    Destroy(seekingTarget);
	    base.SelfDestroy();
	}
	
	private Line MainRay(){
        float distance = rigidbody.velocity.magnitude * timeToThinkAhead;
	    return new Line(nose, nose + Vector3.forward * distance);
	}

	private Line[] CreateWhiskers(){
	    Line[] ret = new Line[4];
	    Vector3 whisker = Quaternion.Euler(whiskersAngle, 0, 0) *  Vector3.forward;
	    for(int i = 0, angle = 0; i < 4; i++, angle += 90){
	        Quaternion rotation = Quaternion.Euler(0, 0, angle);
	        Vector3 dir = rotation * whisker;
	        
	        Line line = new Line(nose, nose + dir * whiskersLength);
	        ret[i] = line;	        
	    }	
	    
	    return ret;    
	}	
	
	private void ChangeState(){
        if(isCollided){
            state = State.Avoiding;
            lastCollisionTime = Time.time;
        }else{
            if(state == State.Avoiding){                
                state = State.PostAvoiding;
            }else if(state == State.PostAvoiding){
                if(Time.time - lastCollisionTime > timeToKeepAvoiding){
                    state = State.Idle;
                    seekingTargetTransform.position = _transform.position;
                }
            }
        }        
	}
	
	private void ProcessState(){
	    switch(state){	        
            case State.Idle:
                steering = SteeringOutput.empty;
                break;
            case State.Turning:
                steering = seeking.orientationMatcher.GetSteering();
                break;
            case State.Avoiding:
            case State.PostAvoiding:
                steering = seeking.GetSteering();            
                break;
        }        
	}
	
	private bool Cast(Line line, out RaycastHit hit) {
        Line worldLine = line.ToWorldFrom(_transform);
	    bool collided = Physics.Raycast (worldLine.from, worldLine.direction, out hit, worldLine.length, obstaclesLayerMask);	    
	    return collided;
	}
	
	private void CheckCollisions(){
	    isCollided = Cast(MainRay(), out hit);        
        if(!isCollided){
            foreach(Line line in whiskers){
                isCollided = Cast(line, out hit);
                if(isCollided)
                    break;
            }
        }  
        
        if(isCollided){
            seekingTargetTransform.position = hit.point + hit.normal * minDistance;
        }        
	}
	
	private void TryCheckCollisions(){
	    if(Time.time - lastCastTime > raycastPeriod){
	        CheckCollisions();
	        lastCastTime = Time.time;
	    }
	}
	
	protected override void PrivateDrawGizmosSelected(){	    
	    if(state != State.Idle){
	        Gizmos.color = Color.green;
	        Gizmos.DrawSphere(hit.point, 0.1f);
	        Gizmos.DrawLine(hit.point, seekingTargetTransform.position);
	    }
    }    
    
    void OnDrawGizmosSelected(){
        if(!_transform)
            return;
        
        Gizmos.color = Color.blue;
        Line mainRay = MainRay().ToWorldFrom(_transform);
        Gizmos.DrawLine(mainRay.from, mainRay.to);        
        
        if(whiskers != null){
            Gizmos.color = Color.magenta;
            foreach(Line line in whiskers){
                Line worldLine = line.ToWorldFrom(_transform);
                Gizmos.DrawLine(worldLine.from, worldLine.to); 
            }                    
        }        
    }    
}

struct Line{
    public Vector3 from;
    public Vector3 to;
    
    public Vector3 direction
    {
        get {return (to - from).normalized;}
    }
    
    public float length
    {
        get {return Vector3.Distance(from, to);}
    }
    
    public Line(Vector3 _from, Vector3 _to){
        from = _from;
        to = _to;
    }
    
    public Line ToWorldFrom(Transform t){
        Line ret = new Line();
        ret.from = t.TransformPoint(from);
        ret.to = ret.from + t.TransformDirection(to - from);
        return ret;
    }
    
    override public string ToString(){
        return string.Format("from: {0}; to: {1}", from, to);
    }
}