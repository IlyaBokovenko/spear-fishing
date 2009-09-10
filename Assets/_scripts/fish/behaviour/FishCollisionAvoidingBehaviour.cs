using UnityEngine;
using System.Collections;

public class FishCollisionAvoidingBehaviour : FishArbitratedBehaviour {
    public float timeToThinkAhead = 3f;
    public float minDistance = 2f;

    public float whiskersOffset = 1f;
    public float whiskersAngle = 30f;    
    public float whiskersLength = 1.0f;
    public float timeToKeepAvoiding = 0.5f;
    
    enum State{
        Idle,
        Avoiding,
        PostAvoiding    
    }
    private State state = State.Idle;
    private float lastCollisionTime = 0.0f;    
        
    private FishSeekingBehaviour seeking;
    private GameObject seekingTarget;
    private RaycastHit hit;
    
    private Line[] whiskers;
    
    private SteeringOutput steering;
    
    FishCollisionAvoidingBehaviour(){
        priority = 2;
    }
	
	void Start () {
	    
	    seeking = (FishSeekingBehaviour)gameObject.AddComponent(typeof(FishSeekingBehaviour));
	    seekingTarget = new GameObject("collision avoidance target");
	    seekingTarget.transform.parent = transform;
	    seeking.target = seekingTarget;	    
	    
	    whiskers = CreateWhiskers();
	}
	
	public override SteeringOutput GetSteering(){
	    if(!seeking)
	        return SteeringOutput.empty;	        
	    

        if(Utils.Approximately(rigidbody.velocity.magnitude, 0.0f))
            return SteeringOutput.empty;

        ChangeState();
        ProcessState();
        
        return steering;
	}
	
	public override void SelfDestroy(){
	    seeking.SelfDestroy();
	    Destroy(seekingTarget);
	    base.SelfDestroy();
	}
	
	private Line MainRay(){
	    float distance = rigidbody.velocity.magnitude * timeToThinkAhead;
	    return new Line(Vector3.zero, Vector3.forward * distance);
	}

	private Line[] CreateWhiskers(){
	    Line[] ret = new Line[4];	    
	    Vector3 whisker = Quaternion.Euler(whiskersAngle, 0, 0) *  Vector3.forward;
	    for(int i = 0, angle = 0; i < 4; i++, angle += 90){
	        Quaternion rotation = Quaternion.Euler(0, 0, angle);
	        Vector3 dir = rotation * whisker;
	        Vector3 orig = Vector3.forward * whiskersOffset;
	        
	        Line line = new Line(orig, orig + dir * whiskersLength);
	        ret[i] = line;	        
	    }	
	    
	    return ret;    
	}
	
	private void ChangeState(){
	    bool collided = CheckCollisions(out hit);
        if(collided){
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
        int layerMask = 1 << gameObject.layer;
        layerMask = ~layerMask;
        Line worldLine = line.ToWorldFrom(transform);
	    bool collided = Physics.Raycast (worldLine.from, worldLine.direction, out hit, worldLine.length, layerMask);	    
	    return collided;
	}
	
	private bool CheckCollisions(out RaycastHit hit){
	    bool collided = Cast(MainRay(), out hit);        
        if(!collided){
            foreach(Line line in whiskers){
                collided = Cast(line, out hit);
                if(collided)
                    break;
            }
        }
        
        return collided;        
	}
	
	private void OnDrawGizmosSelected(){    
        Gizmos.color = Color.blue;
        Line mainRay = MainRay().ToWorldFrom(transform);
        Gizmos.DrawLine(mainRay.from, mainRay.to);
        
        if(whiskers != null){
            Gizmos.color = Color.red;
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