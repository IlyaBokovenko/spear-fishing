using UnityEngine;
using System.Collections;

public class FishCollisionAvoidingBehaviour : FishArbitratedBehaviour {
    public float timeToThinkAhead = 3f;
    public float minDistance = 2f;

    public float whiskersOffset = 0.5f;
    public float whiskersAngle = 30f;    
    public float whiskersLength = 0.2f;
        
    private FishSeekingBehaviour seeking;
    private GameObject seekingTarget;
    
    private Line[] whiskers = new Line[4];
    
    FishCollisionAvoidingBehaviour(){
        priority = 2;
    }
	
	void Start () {
	    
	    seeking = (FishSeekingBehaviour)gameObject.AddComponent(typeof(FishSeekingBehaviour));
	    seekingTarget = new GameObject("collision avoidance target");
	    seekingTarget.transform.parent = transform;
	    seeking.target = seekingTarget;	    
	    
	    CreateWhiskers();
	}
	
	public override SteeringOutput GetSteering(){
	    if(!seeking)
	        return SteeringOutput.empty;	        
	    
        RaycastHit hit;        
        if(Utils.Approximately(MainRay().length, 0.0f))
            return SteeringOutput.empty;
            

            
        bool collided = Cast(MainRay(), out hit);        
        if(!collided){
            foreach(Line line in whiskers){
                collided = Cast(line, out hit);
                if(collided)
                    break;
            }
        }
        
	    if(collided){
	        seekingTarget.transform.position = hit.point + hit.normal * minDistance;
	        return seeking.GetSteering();
	    }
	    
	    seekingTarget.transform.position = transform.position;	    
	    return SteeringOutput.empty;    
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

	private void CreateWhiskers(){	    
	    Vector3 whisker = Quaternion.Euler(whiskersAngle, 0, 0) *  Vector3.forward;
	    for(int i = 0, angle = 0; i < 4; i++, angle += 90){
	        Quaternion rotation = Quaternion.Euler(0, 0, angle);
	        Vector3 dir = rotation * whisker;
	        Vector3 orig = Vector3.forward * whiskersOffset;
	        
	        Line line = new Line(orig, orig + dir * whiskersLength);
	        whiskers[i] = line;	        
	    }	    
	}
	
	private bool Cast(Line line, out RaycastHit hit){
        int layerMask = 1 << gameObject.layer;
        layerMask = ~layerMask;
        Line worldLine = line.ToWorldFrom(transform);
	    bool collided = Physics.Raycast (worldLine.from, worldLine.direction, out hit, worldLine.length, layerMask);	    
	    return collided;
	}
	
	private void OnDrawGizmosSelected(){    
        Gizmos.color = Color.blue;
        Line mainRay = MainRay().ToWorldFrom(transform);
        Gizmos.DrawLine(mainRay.from, mainRay.to);

        Gizmos.color = Color.red;
        foreach(Line line in whiskers){
            Line worldLine = line.ToWorldFrom(transform);
            Gizmos.DrawLine(worldLine.from, worldLine.to);    
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