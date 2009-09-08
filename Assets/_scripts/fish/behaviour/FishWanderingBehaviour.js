class FishWanderingBehaviour extends FishArbitratedBehaviour
{
    var wanderOffset : float = 60;
    var wanderRadiusXZ : float = 10;
    var wanderRadiusY : float = 30;
    var wanderRate : float = 5;
    
    private var invisiblePole : GameObject;
    private var invisibleCarrot : GameObject;
    private var seeking : FishSeekingBehaviour;
    
    function Start(){
        CreatePoleWithCarrot();
        seeking = gameObject.AddComponent(FishSeekingBehaviour);
        seeking.target = invisibleCarrot;        
    }
    
    // function FixedUpdate(){
    //     GetSteering().ApplyTo(gameObject);
    // }

    function GetSteering () : SteeringOutput{
        Profiler.StartProfile(PT.Wandering);
          
        if(!invisiblePole)
            return SteeringOutput.empty;
        UpdatePole();
        ShuffleCarrot();
        
        Profiler.EndProfile(PT.Wandering); 
        
        return seeking.GetSteering();
    }
    
    function SelfDestroy(){
        if(seeking)
            seeking.SelfDestroy();
            
        Destroy(invisiblePole);
        super.SelfDestroy();        
    }
    
    function OnDrawGizmosSelected(){    
        if(!invisiblePole)
            return;
            
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(invisiblePole.transform.position, invisiblePole.transform.lossyScale * 2);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(invisibleCarrot.transform.position, Mathf.Min(wanderRadiusXZ / 5, 0.2));        
    }
    
////////////////////////////////////////////////////////////////////////
    function UpdatePole(){
        if(!invisiblePole)
            return;
            
        var fishDirection = transform.forward; 
        fishDirection.y = 0;
        fishDirection = fishDirection.normalized;
        invisiblePole.transform.position = transform.position + fishDirection * wanderOffset;
        
        var selfScale = Vector3(wanderRadiusXZ, wanderRadiusY, wanderRadiusXZ);
        var parentScale = transform.lossyScale;
        var invertedParentScale = Vector3();
        invertedParentScale.x = 1/parentScale.x; invertedParentScale.y = 1/parentScale.y; invertedParentScale.z = 1/parentScale.z;
        invisiblePole.transform.localScale = Vector3.Scale(invertedParentScale, selfScale);
    }

    private function CreatePoleWithCarrot(){
        invisiblePole = GameObject("Pole with carrot");
        invisiblePole.transform.parent = transform;
        UpdatePole();
        
        invisibleCarrot = GameObject("Invisible carrot");        
        invisibleCarrot.transform.parent = invisiblePole.transform;
        invisibleCarrot.transform.localPosition = Vector3.forward;        
    }

    private function ShuffleCarrot(){
        var oldPos = invisibleCarrot.transform.localPosition.normalized;
        var randomRotation = Quaternion.Euler(RandomAngle(), RandomAngle(), RandomAngle());
        var newPos = randomRotation * oldPos;
        invisibleCarrot.transform.localPosition = newPos;
    }
    
    private function RandomAngle() : float{
        return Utils.RandomBinomial() * wanderRate;
    } 
}