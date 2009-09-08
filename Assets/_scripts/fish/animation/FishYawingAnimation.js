class FishYawingAnimation extends FishAnimation
{
    var yawDrift : float = 5;
    var yawPeriod : float = 5;
    var sideDrift : float = 0.2;

    private var phase : float = 0;
    private var period : float = Mathf.Infinity;

    function LateUpdate(){
        updatePeriodAndPhase();

        // Debug.Log(String.Format("velocity: {0}, period: {1}", rigidbody.velocity.magnitude, period));    

        yawShift = yawDrift * Mathf.Sin(phase);    
        transform.localEulerAngles += Vector3(0, yawShift, 0);

        sideShift = -sideDrift * Mathf.Cos(phase - Mathf.PI / 4);
        transform.localPosition += Vector3(sideShift, 0, 0);
    }

    //////    //////    //////    //////    //////

    function updatePeriodAndPhase(){
        var fishLength = 5;
        period = yawPeriod/transform.parent.rigidbody.velocity.magnitude;        
        phase +=  2 * Mathf.PI * (Time.deltaTime / period);    
    }

    function isIdle() {
        return period > 10;
    }
}