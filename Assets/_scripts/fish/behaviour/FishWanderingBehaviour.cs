using UnityEngine;
using System.Collections;

public class FishWanderingBehaviour : FishArbitratedBehaviour {
    public float wanderOffset = 60f;
    public float wanderRadiusXZ = 10f;
    public float wanderRadiusY = 30f;
    public float wanderRate = 5f;

    private GameObject invisiblePole;
    private GameObject invisibleCarrot;
    private FishSeekingBehaviour seeking;
    
    // private GameObject helper;

    void Start(){
        CreatePoleWithCarrot();
        seeking = (FishSeekingBehaviour)gameObject.AddComponent(typeof(FishSeekingBehaviour));
        seeking.target = invisibleCarrot;        
    }

    public override SteeringOutput GetSteering (){
        Profiler.StartProfile(PT.Wandering);

        if(!invisiblePole)
            return SteeringOutput.empty;

        UpdatePolePosition();
        ShuffleCarrot();        

        SteeringOutput ret = seeking.GetSteering();
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
        Gizmos.DrawWireCube(invisiblePole.transform.position, invisiblePole.transform.lossyScale * 2);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(invisibleCarrot.transform.position, Mathf.Min(wanderRadiusXZ / 5, 0.2f));        
    }

////////////////////////////////////////////////////////////////////////
    void UpdatePolePosition(){
        if(!invisiblePole)
            return;            

        Vector3 fishDirection = transform.forward; 
        fishDirection.y = 0;
        fishDirection = fishDirection.normalized;
        invisiblePole.transform.position = transform.position + fishDirection * wanderOffset; 
        invisiblePole.transform.rotation = Quaternion.identity;
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
        invisibleCarrot.transform.localPosition = Vector3.forward;        
        
        // helper = (GameObject)GameObject.CreatePrimitive(PrimitiveType.Cube);
        // helper.transform.parent = invisiblePole.transform;
        // helper.transform.localScale = new Vector3(2, 2, 2);

        UpdatePoleScale();
        UpdatePolePosition();                
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