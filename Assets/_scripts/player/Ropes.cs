using UnityEngine;
using System.Collections;

public class Ropes : MonoBehaviour {
	public Material material;
	
	private MeshRenderer meshRenderer;
	private MeshFilter meshFilter;
	
	private ArrayList points;
	
	void Start () {
		meshFilter = (MeshFilter)gameObject.AddComponent("MeshFilter");
	    meshRenderer = (MeshRenderer)gameObject.AddComponent("MeshRenderer");
		//meshRenderer.material = material;
		
		//Vector3[] vertices = {new Vector3(1.0f,0.0f,0.0f) , new Vector3(0.0f,1.0f,0.0f), new Vector3(1.0f,1.0f,0.0f)};
		
		/*vertices[0] = new Vector3(1.0f,0.0f,0.0f);
			vertices[1] = new Vector3(0.0f,1.0f,0.0f);
			vertices[2] = new Vector3(1.0f,1.0f,0.0f);
		*/
		/*
		Vector2[] uv = new Vector2[3];
		uv[0] = new Vector2(0.0f, 0.0f);
		uv[1] = new Vector2(0.0f, 1.0f);
		uv[2] = new Vector2(1.0f, 1.0f);
		*/
		points = new ArrayList();
		
		points.Add(new RopePoint(transform.position));
		points.Add(new RopePoint(transform.position * 2.0f));
		
		GenerateMesh();
		
		//int[] triangles = {0,1,2,2,3,0};
		
		//mesh.vertices = point.getVertices();//vertices;
		//mesh.triangles = triangles;
		//mesh.uv = uv;
	}
	
	void Update () {
	
	}
	
	public void GenerateMesh() {
		if(points.Count > 1) {
			Mesh mesh = meshFilter.mesh;
	        mesh.Clear();
			Vector3[] vertices = new Vector3[points.Count * 4];
			int[] triangles = new int[8 * (points.Count - 1)];		
			for(int index = 0; index < points.Count; index ++) {
				Vector3[] tmpVertices = ((RopePoint)points[index]).getVertices();
				vertices[index*4] = tmpVertices[0];
				vertices[index*4 + 1] = tmpVertices[1];
				vertices[index*4 + 2] = tmpVertices[2];
				vertices[index*4 + 3] = tmpVertices[3];
			
				//triangles
			}
		}
		
		//int[] triangles = {0,1,2,2,3,0};
		
		//mesh.vertices = point.getVertices();//vertices;
		//mesh.triangles = triangles;
	
	}
	
	public class RopePoint {
		public Vector3 position;
		public float size = 1.0f;
		
		public RopePoint(Vector3 arg){
			this.position = arg;
		}
		
		public Vector3[] getVertices() {
			Vector3[] result = {new Vector3(this.position.x - size, this.position.y, this.position.z),
				 new Vector3( this.position.x, this.position.y + size, this.position.z),
				 new Vector3( this.position.x + size, this.position.y, this.position.z),
				 new Vector3( this.position.x, this.position.y - size, this.position.z)};
			return result;
		}
	}
}
