using UnityEngine;
using System.Collections;

public class RollMatching : FishBehaviour {
    public float roll = 0f;
    public float speed = 100f;    
    
    public override string ToString(){
        return base.ToString() + " (" + roll + ")";
    }

    public override SteeringOutput GetSteering (){  
        Profiler.StartProfile(PT.RollMatching);
        
        Vector3 up  = Quaternion.Euler(0, 0, roll) * Vector3.up;
        up = transform.InverseTransformDirection(up);
        up.z = 0;
        
        float delta = Quaternion.FromToRotation(Vector3.up, up).eulerAngles.z;
        delta = Utils.DegToShifted(delta);              
        
        float torque = delta * speed;
        
        Profiler.EndProfile(PT.RollMatching);
        
        return SteeringOutput.WithRelativeTorque(new Vector3(0, 0, torque) * Mathf.Deg2Rad);        
    }
    
    public void Start() {}
    
}

