using UnityEngine;
using System.Collections;

public class Utils : System.Object {
    public const float Epsilon = 0.000001f;
	
    public static float RadNormalized(float angleRadians){
        angleRadians = angleRadians % 2*Mathf.PI;
        if(angleRadians < 0)        
            angleRadians += 2*Mathf.PI;
        
        return angleRadians;        
    }
        
    public static float DegNormalized(float angleDegrees){
        angleDegrees = angleDegrees % 360;
        if(angleDegrees < 0)        
            angleDegrees += 360;
        
        return angleDegrees;
    }

    public static float RadToShifted(float angleRadians){
        angleRadians = RadNormalized(angleRadians);
        return angleRadians <= Mathf.PI ? angleRadians : angleRadians - 2*Mathf.PI;
    }
    
    public static Vector3 RadToShifted(Vector3 vectorRadians){
        Vector3 ret = Vector3.zero;
        ret.x = RadToShifted(vectorRadians.x);
        ret.y = RadToShifted(vectorRadians.y);
        ret.z = RadToShifted(vectorRadians.z);
        
        return ret;
    } 
    
    public static float DegToShifted(float angleDegrees){
        angleDegrees = DegNormalized(angleDegrees);
        return angleDegrees <= 180 ? angleDegrees : angleDegrees - 360;
    }   
    
    public static Vector3 DegToShifted(Vector3 vectorDegrees){
        Vector3 ret = Vector3.zero;
        ret.x = DegToShifted(vectorDegrees.x);
        ret.y = DegToShifted(vectorDegrees.y);
        ret.z = DegToShifted(vectorDegrees.z);
        
        return ret;
    } 
    
    public static Vector3 ClampComponents(Vector3 v, float min, float max){
        v.x = Mathf.Clamp(v.x, min, max);
        v.y = Mathf.Clamp(v.y, min, max);
        v.z = Mathf.Clamp(v.z, min, max);
        
        return v;
    }
    
    public static Vector3 ClampMagnitude(Vector3 v, float min, float max) {
        float mag = Mathf.Clamp(v.magnitude, min, max);
        return v.normalized * mag;
    }
    
    public static Vector3 PowComponents(Vector3 v, float power){
        v.x = Mathf.Pow( v.x, power);
        v.y = Mathf.Pow( v.y, power);
        v.z = Mathf.Pow( v.z, power);
        
        return v;
    }
    
    public static float RandomBinomial(){
        return Random.value - Random.value;
    }
    
    // Because Mathf.Approximately doesn't work in compiled application    
    public static bool Approximately(float one, float another){
        return Mathf.Abs(one - another) <= Epsilon;
    }
}