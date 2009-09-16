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
 
    private GameObject invisiblePole;
    private GameObject invisibleCarrot;
    
    void Awake(){
        children = new FishBehaviour[1]{seeking};        
    }
    
    void Start(){        
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
        Quaternion randomRotation = Quaternion.Euler(RandomAngle(), RandomAngle(), RandomAngle());
        Vector3 newPos = randomRotation * oldPos;
        invisibleCarrot.transform.localPosition = newPos;
    }

    private float RandomAngle(){
        return Utils.RandomBinomial() * wanderRate;
    } 
}