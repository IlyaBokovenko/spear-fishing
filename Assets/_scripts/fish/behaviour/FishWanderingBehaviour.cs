using UnityEngine;
using System.Collections;

public class FishWanderingBehaviour : FishArbitratedBehaviour {
    
    public GenericSeekingBehaviour seeking;
    
    public float wanderOffset = 10f;
    public float wanderRadiusXZ = 1f;
    public float wanderRadiusY = 3f;
    public float wanderRate = 30f;
    public float carrotUpdatePeriod = 0.1f;
    private float lastCarrotUpdate = 0.0f;
 
    private Transform _transform;
    private GameObject invisiblePole;
    private Transform invisiblePoleTransform;
    private GameObject invisibleCarrot;
    private Transform invisibleCarrotTransform;
    
    
    void Awake(){
        children = new FishBehaviour[1]{seeking};        
    }
    
    void Start(){        
        _transform = transform;
        lastCarrotUpdate = Time.time;
		CreatePoleWithCarrot();        
        seeking.target = invisibleCarrot;
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
        Destroy(invisiblePole);
        base.SelfDestroy();        
    }

	protected override void PrivateDrawGizmosSelected(){	      
        if(!invisiblePole)
            return;

        Gizmos.color = Color.white;
        DrawUnitCubeIn(invisiblePoleTransform);
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

        Vector3 fishDirection = _transform.forward; 
        fishDirection.y = 0;
        fishDirection = fishDirection.normalized;
        invisiblePoleTransform.position = _transform.position + fishDirection * wanderOffset; 
        Quaternion rotation = _transform.rotation;        
        invisiblePoleTransform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
    }
    
    void UpdatePoleScale(){
        if(!invisiblePole)
            return;            
        
        Vector3 selfScale = new Vector3(wanderRadiusXZ, wanderRadiusY, wanderRadiusXZ);
        Vector3 parentScale = _transform.lossyScale;
        Vector3 invertedParentScale = Vector3.zero;
        invertedParentScale.x = 1/parentScale.x; invertedParentScale.y = 1/parentScale.y; invertedParentScale.z = 1/parentScale.z;
        invisiblePoleTransform.localScale = Vector3.Scale(invertedParentScale, selfScale);        
    }

    private void CreatePoleWithCarrot(){
        invisiblePole = new GameObject("Pole with carrot");
        invisiblePoleTransform = invisiblePole.transform;
        invisiblePoleTransform.parent = _transform;
        
        invisibleCarrot = new GameObject("Invisible carrot");        
        invisibleCarrotTransform = invisibleCarrot.transform;
        invisibleCarrotTransform.parent = invisiblePoleTransform;
        invisibleCarrotTransform.localPosition = Random.insideUnitSphere;        
        
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
        Vector3 oldPos = invisibleCarrotTransform.localPosition.normalized;
        Quaternion randomRotation = Quaternion.Euler(RandomAngle(), RandomAngle(), RandomAngle());
        Vector3 newPos = randomRotation * oldPos;
        invisibleCarrotTransform.localPosition = newPos;
    }

    private float RandomAngle(){
        return Utils.RandomBinomial() * wanderRate;
    } 
}