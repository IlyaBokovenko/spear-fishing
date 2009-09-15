using UnityEngine;
using System.Collections;

public class FishWanderingBehaviour : FishArbitratedBehaviour {
    public float wanderOffset = 60f;
    public float wanderRadiusXZ = 20f;
    public float wanderRadiusY = 30f;
    public float wanderRate = 3f;
    public float carrotUpdatePeriod = 2f;
    private float lastCarrotUpdate = 0.0f;
    public float maxSpeed = 3f;
 
    private GameObject invisiblePole;
    private GameObject invisibleCarrot;
    private FishSeekingBehaviour seeking;
    
	protected override ArrayList children
    {
        get {ArrayList ret = base.children; ret.Add(seeking); return ret; }
    }    	
    
    void Start(){
        CreatePoleWithCarrot();
        seeking = (FishSeekingBehaviour)gameObject.AddComponent(typeof(FishSeekingBehaviour));
        seeking.target = invisibleCarrot;        
        seeking.maxSpeed = maxSpeed;
    }

    public override SteeringOutput GetSteering (){
        Profiler.StartProfile(PT.Wandering);
        
        SteeringOutput ret;

        if(!invisiblePole)
            ret = SteeringOutput.empty;
        else{
            UpdatePolePosition();
            TryShuffleCarrot();

            ret = seeking.GetSteering();            
        }
        
        Profiler.EndProfile(PT.Wandering);             

        return ret;
    }

    public override void SelfDestroy(){
        if(seeking)
            seeking.SelfDestroy();

        Destroy(invisiblePole);
        base.SelfDestroy();        
    }

    void OnDrawGizmosSelected(){    
        if(!invisiblePole)
            return;

        Gizmos.color = Color.white;
        DrawUnitCubeIn(invisiblePole.transform);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(invisibleCarrot.transform.position, Mathf.Min(wanderRadiusXZ / 5, 0.2f));        
    }
    
    void DrawUnitCubeIn(Transform t){
        Gizmos.DrawLine(t.TransformPoint(new Vector3(-1,-1,-1)), t.TransformPoint(new Vector3(-1,-1,1)));
        Gizmos.DrawLine(t.TransformPoint(new Vector3(-1,-1,1)), t.TransformPoint(new Vector3(1,-1,1)));
        Gizmos.DrawLine(t.TransformPoint(new Vector3(1,-1,1)), t.TransformPoint(new Vector3(1,-1,-1)));
        Gizmos.DrawLine(t.TransformPoint(new Vector3(1,-1,-1)), t.TransformPoint(new Vector3(-1,-1,-1)));
        
        Gizmos.DrawLine(t.TransformPoint(new Vector3(-1,1,-1)), t.TransformPoint(new Vector3(-1,1,1)));
        Gizmos.DrawLine(t.TransformPoint(new Vector3(-1,1,1)), t.TransformPoint(new Vector3(1,1,1)));
        Gizmos.DrawLine(t.TransformPoint(new Vector3(1,1,1)), t.TransformPoint(new Vector3(1,1,-1)));
        Gizmos.DrawLine(t.TransformPoint(new Vector3(1,1,-1)), t.TransformPoint(new Vector3(-1,1,-1)));
        
        Gizmos.DrawLine(t.TransformPoint(new Vector3(-1,-1,-1)), t.TransformPoint(new Vector3(-1,1,-1)));
        Gizmos.DrawLine(t.TransformPoint(new Vector3(-1,-1,1)), t.TransformPoint(new Vector3(-1,1,1)));
        Gizmos.DrawLine(t.TransformPoint(new Vector3(1,-1,1)), t.TransformPoint(new Vector3(1,1,1)));
        Gizmos.DrawLine(t.TransformPoint(new Vector3(1,-1,-1)), t.TransformPoint(new Vector3(1,1,-1)));        
    }

////////////////////////////////////////////////////////////////////////
    void UpdatePolePosition(){
        if(!invisiblePole)
            return;            

        Vector3 fishDirection = transform.forward; 
        fishDirection.y = 0;
        fishDirection = fishDirection.normalized;
        invisiblePole.transform.position = transform.position + fishDirection * wanderOffset; 
        Quaternion rotation = transform.rotation;        
        invisiblePole.transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
    }
    
    void UpdatePoleScale(){
        if(!invisiblePole)
            return;            
        
        Vector3 selfScale = new Vector3(wanderRadiusXZ, wanderRadiusY, wanderRadiusXZ);
        Vector3 parentScale = transform.lossyScale;
        Vector3 invertedParentScale = Vector3.zero;
        invertedParentScale.x = 1/parentScale.x; invertedParentScale.y = 1/parentScale.y; invertedParentScale.z = 1/parentScale.z;
        invisiblePole.transform.localScale = Vector3.Scale(invertedParentScale, selfScale);        
    }

    private void CreatePoleWithCarrot(){
        invisiblePole = new GameObject("Pole with carrot");
        invisiblePole.transform.parent = transform;
        
        invisibleCarrot = new GameObject("Invisible carrot");        
        invisibleCarrot.transform.parent = invisiblePole.transform;
        invisibleCarrot.transform.localPosition = Random.insideUnitSphere;        
        
        UpdatePoleScale();
        UpdatePolePosition();                
    }
    
    private void TryShuffleCarrot(){
        if(Time.time - lastCarrotUpdate > carrotUpdatePeriod){
            ShuffleCarrot();
            lastCarrotUpdate = Time.time;
        }
    }

    private void ShuffleCarrot(){
        Vector3 oldPos = invisibleCarrot.transform.localPosition.normalized;
        // Vector3 oldPos = Vector3.forward;
        Quaternion randomRotation = Quaternion.Euler(RandomAngle(), RandomAngle(), RandomAngle());
        Vector3 newPos = randomRotation * oldPos;
        invisibleCarrot.transform.localPosition = newPos;
    }

    private float RandomAngle(){
        return Utils.RandomBinomial() * wanderRate;
    } 
}