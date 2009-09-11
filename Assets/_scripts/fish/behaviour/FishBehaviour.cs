using UnityEngine;
using System.Collections;

public class FishBehaviour : GenericScript
{    
    public virtual SteeringOutput GetSteering(){
        return new SteeringOutput();
    }    
    
    public virtual string ToString(){
        string name = GetType().Name;
        name = name.Replace("Fish", "");
        name = name.Replace("Behaviour", "");
        return name;
    }
}
