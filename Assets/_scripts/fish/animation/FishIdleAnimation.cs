using UnityEngine;
using System.Collections;

public class FishIdleAnimation : FishAnimation
{
    public float period = 10f;
    public float rollDrift = 5f;
    public float heightDrift = 0.2f;

    void LateUpdate(){
        Profiler.StartProfile(PT.Idle);

        float angularValue = 2 * Mathf.PI * Time.time / period;

        float rollShift = rollDrift * Mathf.Sin(angularValue);    
        transform.localEulerAngles += new Vector3(rollShift, 0 , 0);

        float heightShift = - heightDrift * Mathf.Cos(angularValue);
        transform.localPosition += new Vector3(0, heightShift, 0);

        Profiler.EndProfile(PT.Idle);
    }
}