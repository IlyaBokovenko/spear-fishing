var prev : float = 0;
var prevTime : float = 0;
var counter : int = 0;

var acc : float = 100;

function Start(){
    // rigidbody.AddTorque(Vector3.forward, ForceMode.VelocityChange);    
}


function FixedUpdate () {
    // print(Time.deltaTime);
    
    // if(Time.time < 5){
        var force = Vector3.forward * acc * Time.deltaTime;
        rigidbody.AddTorque(force * Mathf.Deg2Rad, ForceMode.VelocityChange);
        // print(rigidbody.angularVelocity);        
    // }    
    
    var cur = transform.eulerAngles.z;
    if(prev > cur){
        counter++;
        print(String.Format("tm: {0}; num: {1}; speed: {2}", Time.time, counter, 360 / (Time.time - prevTime)));
        prevTime = Time.time;
    }    
    prev = cur;    
}