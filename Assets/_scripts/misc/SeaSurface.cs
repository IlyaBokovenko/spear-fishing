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
    private Transform playerTransform;
    private int updateDelay = 3;
    private int count = 0;    
    
    Vector3 closestVertex;
    float cachedMinDist;
    float cachedUnderwaterLevel;        

    void Awake() {
    	goTransform = transform;
    }

    void Start () {
    	player = GameObject.Find("Player");
    	playerTransform = player.transform;
    }

    void Update () {        
          if(Mathf.Abs(playerTransform.position.y - goTransform.position.y) > 0.5f) return;
        
          MeshFilter meshFilter = (MeshFilter)GetComponent(typeof(MeshFilter));
          Mesh mesh  = meshFilter.mesh;
         
          if (baseHeight == null)
              baseHeight = mesh.vertices;
           
          Vector3[] vertices = new Vector3[baseHeight.Length];
          
          cachedMinDist = Mathf.Infinity;
          for (int i=0;i<vertices.Length;i++) {
               Vector3 vertex = baseHeight[i];
               vertex.y += Mathf.Sin(Time.time * speed + baseHeight[i].x + baseHeight[i].y + baseHeight[i].z) * scale;
               vertices[i] = vertex;
               UpdateUnderwaterLevel(vertex);
          }
          mesh.vertices = vertices;
          mesh.RecalculateNormals();          
          player.SendMessage("SetUnderwaterLevel", cachedUnderwaterLevel);
    }    
    
    void UpdateUnderwaterLevel(Vector3 vertex){        
        Vector3 vertexGlobal = goTransform.TransformPoint(vertex);
        float dist = Vector3.Distance(playerTransform.position, vertexGlobal);
        
        if(dist < cachedMinDist){            
            closestVertex = vertexGlobal;                   
            cachedMinDist = dist;
            cachedUnderwaterLevel = closestVertex.y;
        }           
        
    }
    
    void OnDrawGizmos(){
        if(player == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(goTransform.position + closestVertex, 0.1f);        
        
    }

}