class FishLookAheadBehaviour extends FishBehaviour{
    
    private var orientationMatcher : OrientationMatching;
    
    function FishLookAheadBehaviour(){
        priority = 1;
    }
    
        
    function Start(){
        orientationMatcher = gameObject.AddComponent(OrientationMatching);
    }
    
    function SelfDestroy(){
        orientationMatcher.SelfDestroy();
        super.SelfDestroy();
    }
    
    function GetSteering() : SteeringOutput{
        if(!orientationMatcher)
            return SteeringOutput.empty;
        
        if(!Utils.Approximately(0, rigidbody.velocity.magnitude))
            orientationMatcher.orientation = rigidbody.velocity;
        else
            orientationMatcher.orientation = transform.forward;
            
        return orientationMatcher.GetSteering();
    }
}