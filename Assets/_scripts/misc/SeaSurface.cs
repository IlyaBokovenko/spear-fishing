using UnityEngine;
using System.Collections;

public class SeaSurface : MonoBehaviour {
    public float scale = 0.05f;
    public float speed = 1.0f;

    private Vector3[] baseHeight;
    private GameObject player;
    private Rect playerRect;
    private float radius = 6.0f;
    private Transform goTransform;
    private int updateDelay = 3;
    private int count = 0;    

    void Awake() {
    	goTransform = transform;
    }

    void Start () {
    	player = GameObject.Find("Player");
    }

    void Update () {        
          MeshFilter meshFilter = (MeshFilter)GetComponent(typeof(MeshFilter));
          Mesh mesh  = meshFilter.mesh;
         
          if (baseHeight == null)
              baseHeight = mesh.vertices;
           
          Vector3[] vertices = new Vector3[baseHeight.Length];
          for (int i=0;i<vertices.Length;i++) {
           Vector3 vertex = baseHeight[i];
           vertex.y += Mathf.Sin(Time.time * speed + baseHeight[i].x + baseHeight[i].y + baseHeight[i].z) * scale;
           vertices[i] = vertex;
          }
          mesh.vertices = vertices;
          mesh.RecalculateNormals();
    }

}