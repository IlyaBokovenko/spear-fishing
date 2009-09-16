using UnityEngine;
using System.Collections;
using System;

[RequireComponent (typeof (Nose))]
public class FishObstacleAvoidingBehaviour : FishArbitratedBehaviour {   

    public FishSeekingBehaviour seeking;     
    
    public float timeToThinkAhead = 3f;
    public float minDistance = 2f;

    public float whiskersAngle = 30f;    
    public float whiskersLength = 1.0f;
    public float timeToKeepAvoiding = 0.5f;
    
    public float  raycastPeriod = 0.3f;
    private float lastCastTime = 0.0f;
    
    enum State{
        Idle,
        Avoiding,
        PostAvoiding    
    }
    
    private State state = State.Idle;
    private float lastCollisionTime = 0.0f;            
    
    private GameObject seekingTarget;
    
    private bool isCollided = false;
    private RaycastHit hit;
    
    private Line[] whiskers;
    
    private SteeringOutput steering;
    
    private int obstaclesLayerMask;
    public string[] layersToIgnore;
    
    private Vector3 nose;
    
    FishObstacleAvoidingBehaviour(){
        priority = 2;
        
    } 
    
    protected override ArrayList ActiveChildren(){
        ArrayList ret = base.ActiveChildren();
        if(state != State.Idle){
            ret.Add(seeking);
        }
        return ret;
    }
    
    public override string ToString(){
        return base.ToString() + " : " + Enum.GetName(typeof(State), state);
    }
            
    void Awake(){
        Nose _nose  = (Nose)GetComponent(typeof(Nose)); 
        nose = _nose.position;	    
        children = new FishBehaviour[1]{seeking};
    }
	
	void Start () {	    
	    SetObstaclesLayerMask();
	    
	    seekingTarget = new GameObject("collision avoidance target");
	    seekingTarget.transform.parent = transform;
	    seeking.target = seekingTarget;	    

	    whiskers = CreateWhiskers();	    
	}
	
	public override SteeringOutput GetSteering(){
	    Profiler.StartProfile(PT.CollisionAvoiding);
	    
	    if(!seeking || Utils.Approximately(rigidbody.velocity.magnitude, 0.0f))
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
	
	private void SetObstaclesLayerMask(){
        int mask = 0;
        foreach(string name in layersToIgnore){
            int layer = LayerMask.NameToLayer(name);
            int lmask = 1 << layer;
            mask |= lmask;
        }
        obstaclesLayerMask = ~mask;                    
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
                }
            }
        }        
	}
	
	private void ProcessState(){
	    switch(state){
            case State.Idle:
                steering = SteeringOutput.empty;
                break;
            case State.Avoiding:
            case State.PostAvoiding:
                seekingTarget.transform.position = hit.point + hit.normal * minDistance;
                steering = seeking.GetSteering();            
                break;
        }        
	}
	
	private bool Cast(Line line, out RaycastHit hit){
        Line worldLine = line.ToWorldFrom(transform);
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
	}
	
	private void TryCheckCollisions(){
	    if(Time.time - lastCastTime > raycastPeriod){
	        CheckCollisions();
	        lastCastTime = Time.time;
	    }
	}
	
	private void OnDrawGizmosSelected(){	    
	    if(!enabled)
	        return;
	    
        Gizmos.color = Color.blue;
        Line mainRay = MainRay().ToWorldFrom(transform);
        Gizmos.DrawLine(mainRay.from, mainRay.to);
        
        if(whiskers != null){
            Gizmos.color = Color.magenta;
            foreach(Line line in whiskers){
                Line worldLine = line.ToWorldFrom(transform);
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
        get {return (to - from).magnitude;}
    }
    
    public Line(Vector3 _from, Vector3 _to){
        from = _from;
        to = _to;
    }
    
    public Line ToWorldFrom(Transform t){
        Line ret = new Line();
        ret.from = t.TransformPoint(from);
        ret.to = t.TransformPoint(to);
        return ret;
    }
}