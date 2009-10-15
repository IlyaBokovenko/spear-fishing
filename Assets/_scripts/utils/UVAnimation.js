public var vector : Vector2 = Vector2(0,0);
public var scrollSpeed :float  = 0.1;

function Update () {
   var offset : float = Time.time * scrollSpeed;
   renderer.material.SetTextureOffset ("_MainTex", Vector2(offset * vector.x,offset * vector.y));
} 