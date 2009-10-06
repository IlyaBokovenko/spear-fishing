var scale = 10.0;
var speed = 1.0;

private var baseHeight : Vector3[];
private var player : GameObject;
private var playerRect : Rect;
private var radius : float = 6.0;
private var nearVertexs : Array;
private var goTransform : Transform;
private var updateDelay : int = 3;
private var count : int = 0;

function Awake() {
	this.goTransform = transform;
}

function Start () {
	this.player = GameObject.Find("Player");
	this.nearVertexs = new Array();
}

function Update () {
	var meshFilter : MeshFilter = GetComponent(MeshFilter);
	var mesh : Mesh = meshFilter.mesh;
   
	if (baseHeight == null)
		baseHeight = mesh.vertices;
   	
	if((count++) % updateDelay == 0) {
		if(this.player) {
			playerRect = new Rect(player.transform.position.x - 3.0, player.transform.position.z - 3.0, radius, radius);
		} else {
			playerRect = new Rect(0,0,0,0);
		}
	}
	
	var vertices = new Vector3[baseHeight.Length];
	for (var i=0;i<vertices.Length;i++) {
		var vertex = baseHeight[i];
		vertex.y += Mathf.Sin(Time.time * speed + baseHeight[i].x + baseHeight[i].y + baseHeight[i].z) * scale;
		vertices[i] = vertex;
		if((count++) % updateDelay == 0 && playerRect.Contains(new Vector2(vertex.x * 4.0, vertex.z * 4.0))) {
			this.nearVertexs.Add(vertex);
		}
	}
	mesh.vertices = vertices;
	mesh.RecalculateNormals();
	var meshCollider : MeshCollider = GetComponent(MeshCollider);
	
	if((count++) % updateDelay == 0) {
		var underwaterLevel : float = 0.0;
		for(var element : Vector3 in this.nearVertexs) {
			underwaterLevel += element.y;
		}
		underwaterLevel = this.nearVertexs.length > 0 ? underwaterLevel / this.nearVertexs.length + goTransform.position.y : goTransform.position.y;
		if(this.player) {
			this.player.SendMessage("setUnderwaterLevel", underwaterLevel);
		}
		this.nearVertexs.Clear();
	}
	
}