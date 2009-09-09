using UnityEngine;
using System.Collections;

public class FishYawingAnimation : FishAnimation 
{
    public float yawDrift  = 5f;
    public float yawPeriod  = 5f;
    public float sideDrift  = 0.2f;

    private float phase  = 0;
    private float period  = Mathf.Infinity;

    void LateUpdate(){
        Profiler.StartProfile(PT.Yawing);

        updatePeriodAndPhase();

        // Debug.Log(String.Format("velocity: {0}, period: {1}", rigidbody.velocity.magnitude, period));    

        float yawShift = yawDrift * Mathf.Sin(phase);    
        transform.localEulerAngles += new Vector3(0, yawShift, 0);

        float sideShift = -sideDrift * Mathf.Cos(phase - Mathf.PI / 4);
        transform.localPosition += new Vector3(sideShift, 0, 0);

        Profiler.EndProfile(PT.Yawing);
    }

    //////    //////    //////    //////    //////

    void updatePeriodAndPhase(){
        period = yawPeriod/transform.parent.rigidbody.velocity.magnitude;        
        phase +=  2 * Mathf.PI * (Time.deltaTime / period);    
    }

    bool isIdle() {
        return period > 10;
    }
}