private var _contacts : LocalContactPoint[];

function Update () {
    if(_contacts){        
        DrawContacts(_contacts);
    }    
}

function OnCollisionEnter(c : Collision){
    ar = new Array();
    for (var cp : ContactPoint in c.contacts){
        var localCp = new LocalContactPoint(cp, transform);
        ar.Push(localCp);
    }    
    _contacts = ar.ToBuiltin(LocalContactPoint); 
    // Debug.Break();  
}

////////////////////////////////////////////////////////////////////////////////////////

function DrawContacts(contacts : LocalContactPoint[]){
    for (var cp : LocalContactPoint in contacts){
        var pt = cp.worldPoint();
        var dir = cp.worldNormal();
        var x : Vector3 = Vector3.right;
        var y : Vector3 = Vector3.up;            
        Vector3.OrthoNormalize(dir, x, y);
        Debug.DrawRay(pt, dir, Color.red);            
        Debug.DrawRay(pt, x * 0.1, Color.green);            
        Debug.DrawRay(pt, y * 0.1, Color.green);
        //Debug.Log(pt);
    }    
}

////////////////////////////////////////////////////////////////////////////////////////

class LocalContactPoint{        
    public var point: Vector3;
    public var normal: Vector3;
    private var transform : Transform;
    
    function LocalContactPoint(cp : ContactPoint, _transform : Transform){
        transform = _transform;
        point = transform.InverseTransformPoint(cp.point);
        normal = transform.InverseTransformDirection(cp.normal);        
    }    
    
    function worldPoint(){
        return transform.TransformPoint(point);
    }
    
    function worldNormal(){
        return transform.TransformDirection(normal);
    }
}

