class FishEscapeBehaviour extends FishArbitratedBehaviour
{    	
    var target : GameObject;
    var fleeSpeed : float = 10;    
    var safetyDistance : float = 3;
    var panicTime : float = 4;    
    
    private var isEscaping : boolean = false;
    private var startEscapingTime : float;    
    
    private var seeking : FishSeekingBehaviour;
    
    function FishEscapeBehaviour(){
        priority = 1;
    }
    
    function Start(){
        target = gameObject.FindWithTag("Player");
        
        seeking = gameObject.AddComponent(FishSeekingBehaviour);
        seeking.target = target;
        seeking.isFlee = true;
        seeking.maxSpeed = fleeSpeed;
    }
    
    function SelfDestroy(){
    	if(seeking)
        	seeking.SelfDestroy();
        	
        super.SelfDestroy();
    }
    
    function GetSteering () : SteeringOutput{        
        
        if(!seeking || !target)
            return SteeringOutput();
            
        if(Vector3.Distance(transform.position, target.transform.position) < safetyDistance ){
            if(!isEscaping){
                startEscapingTime = Time.time;
            }
            isEscaping = true;
        }
            
        if(Time.time - startEscapingTime > panicTime){
            isEscaping = false;
        }    

        if(isEscaping)
        {
            return seeking.GetSteering();
        }else{
            return SteeringOutput();
        }
    }   
}
