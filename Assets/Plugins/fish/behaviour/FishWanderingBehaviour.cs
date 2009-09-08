using UnityEngine;
using System.Collections;

public class FishWanderingBehaviour : FishArbitratedBehaviour {
    float wanderOffset = 60f;
    float wanderRadiusXZ = 10f;
    float wanderRadiusY = 30f;
    float wanderRate = 5f;

    private GameObject invisiblePole;
    private GameObject invisibleCarrot;
    private FishSeekingBehaviour seeking;

    void Start(){
        CreatePoleWithCarrot();
        seeking = (FishSeekingBehaviour)gameObject.AddComponent(typeof(FishSeekingBehaviour));
        seeking.target = invisibleCarrot;        
    }

    // function FixedUpdate(){
    //     GetSteering().ApplyTo(gameObject);
    // }

    public override SteeringOutput GetSteering (){
        Profiler.StartProfile(PT.Wandering);

        if(!invisiblePole)
            return SteeringOutput.empty;

        // UpdatePole();
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
    void UpdatePole(){
        if(!invisiblePole)
            return;

        Vector3 fishDirection = transform.forward; 
        fishDirection.y = 0;
        fishDirection = fishDirection.normalized;
        invisiblePole.transform.position = transform.position + fishDirection * wanderOffset;

        Vector3 selfScale = new Vector3(wanderRadiusXZ, wanderRadiusY, wanderRadiusXZ);
        Vector3 parentScale = transform.lossyScale;
        Vector3 invertedParentScale = Vector3.zero;
        invertedParentScale.x = 1/parentScale.x; invertedParentScale.y = 1/parentScale.y; invertedParentScale.z = 1/parentScale.z;
        invisiblePole.transform.localScale = Vector3.Scale(invertedParentScale, selfScale);
    }

    private void CreatePoleWithCarrot(){
        invisiblePole = new GameObject("Pole with carrot");
        invisiblePole.transform.parent = transform;
        UpdatePole();

        invisibleCarrot = new GameObject("Invisible carrot");        
        invisibleCarrot.transform.parent = invisiblePole.transform;
        invisibleCarrot.transform.localPosition = Vector3.forward;        
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