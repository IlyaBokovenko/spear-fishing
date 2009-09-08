class Player extends MonoBehaviour
{

    public var acceleration : float = 100;
    public var firingPower: float = 1000;
    public var gun : Transform;
    public var bullet : GameObject;
    
    public var speed : float = 4; 
    
    // function Start(){
    //     Fire();
    // }
    
        
    function Awake() {
        Screen.SetResolution(480, 320, true);
    }
    
    function FixedUpdate(){
       rigidbody.velocity = transform.forward * speed;
    }
    
    function OnApplicationQuit(){
        Profiler.PrintResults();
    }

    function Update () {

        var rotation = transform.localEulerAngles;
        rotation += 2 * Vector3(-Input.GetAxis ("Mouse Y"), Input.GetAxis ("Mouse X"), 0);
        transform.localEulerAngles = rotation;

        if(Input.GetKey("w")){
            rigidbody.AddForce(transform.forward * acceleration);
        }
        if(Input.GetKey("s")){
            rigidbody.AddForce(transform.forward * -acceleration);
        }

        if(Input.GetKey("a")){        
            rigidbody.AddForce(transform.right * -acceleration);
        }
        if(Input.GetKey("d")){        
            rigidbody.AddForce(transform.right * acceleration);
        }

        fireIfNeeded();
    }

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    function fireIfNeeded(){
       if(Input.GetKeyDown(KeyCode.Space)) {
           Fire();
       }
    }
    
    function Fire(){
        var newBullet : GameObject = Instantiate(bullet, gun.position, transform.rotation);
        Physics.IgnoreCollision(collider, newBullet.collider);
        newBullet.transform.localScale = bullet.transform.lossyScale;
        newBullet.rigidbody.velocity = rigidbody.velocity;
        var spear : Spear = newBullet.GetComponent(Spear);
        spear.FireWithForce(firingPower);        
    }
}