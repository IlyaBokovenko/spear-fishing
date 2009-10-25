var scale = 10.0;
var speed = 1.0;

private var baseHeight : Vector3[];
private var player : GameObject;
private var playerRect : Rect;
private var radius : float = 6.0;
private var goTransform : Transform;
private var updateDelay : int = 3;
private var count : int = 0;

function Awake() {
	this.goTransform = transform;
}

function Start () {
	this.player = GameObject.Find("Player");
}

function Update () {
	var meshFilter : MeshFilter = GetComponent(MeshFilter);
	var mesh : Mesh = meshFilter.mesh;
   
	if (baseHeight == null)
		baseHeight = mesh.vertices;
   	
	var vertices = new Vector3[baseHeight.Length];
	for (var i=0;i<vertices.Length;i++) {
		var vertex = baseHeight[i];
		vertex.y += Mathf.Sin(Time.time * speed + baseHeight[i].x + baseHeight[i].y + baseHeight[i].z) * scale;
		vertices[i] = vertex;
	}
	mesh.vertices = vertices;
	mesh.RecalculateNormals();
	var meshCollider : MeshCollider = GetComponent(MeshCollider);
}