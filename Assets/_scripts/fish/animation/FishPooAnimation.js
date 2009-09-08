class FishPooAnimation extends FishAnimation
{
    function Start(){
        InvokeRepeating("makePoo", 0, 0.2);    
    }
    

    function makePoo(){
       var poo = GameObject.CreatePrimitive(PrimitiveType.Cube);
       poo.transform.localScale.x= 0.5;
       poo.transform.localScale.y= 0.5;
       poo.transform.localScale.z = 2;       
       DestroyImmediate(poo.collider);
       poo.transform.position = transform.position;
       poo.transform.rotation = transform.rotation;
    }
}