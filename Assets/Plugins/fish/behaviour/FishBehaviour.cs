using UnityEngine;
using System.Collections;

public class FishBehaviour : GenericScript
{    
    public virtual SteeringOutput GetSteering(){
        return new SteeringOutput();
    }    
}
