class FishIdleAnimation extends FishAnimation
{
    var period : float = 10;
    var rollDrift : float = 5;
    var heightDrift : float = 0.2;

    function LateUpdate(){
        angularValue = 2 * Mathf.PI * Time.time / period;
        
        rollShift = rollDrift * Mathf.Sin(angularValue);    
        transform.localEulerAngles += Vector3(rollShift, 0 , 0);
    
        heightShift = - heightDrift * Mathf.Cos(angularValue);
        transform.localPosition += Vector3(0, heightShift, 0);
    }
}