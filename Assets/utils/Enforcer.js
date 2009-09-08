var velocity : Vector3;
var angularVelocity : Vector3;

function Start(){
    rigidbody.AddForce(velocity, ForceMode.VelocityChange);
    rigidbody.AddTorque(angularVelocity * Mathf.Deg2Rad, ForceMode.VelocityChange);
}
