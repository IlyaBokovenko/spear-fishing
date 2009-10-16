using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {
    public GameObject sample;
    public int population  = 20;
    public Vector3 spawnVolumeBounds  = new Vector3(50, 50, 50);
    public float minSizeDeviation = 0.5f;
    public float maxSizeDeviation = 2.0f;
    
    private GameObject[] children;
    
    private string cloneName;
    
    void Start(){
        children = new GameObject[0];

        MaintainPopulation();
        
        InvokeRepeating("MaintainPopulation", 0.0f, 5.0f);
    }
    
    private void OnDrawGizmosSelected(){    
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, spawnVolumeBounds);
	}

    ////////////////////////////////////////////////////////////////////////
	GameObject Spawn(){
        Quaternion rotation = Random.rotation;        
        GameObject instance  = (GameObject)Instantiate(sample, new Vector3(10000, 10000, 10000), rotation);
        cloneName = instance.name = sample.name;          
        instance.SendMessage("SetSize", RandomSize(), SendMessageOptions.DontRequireReceiver);
        if(!MoveToClearWater(instance))
            Destroy(instance);        
        AddChild(instance);
        return instance;
    }

    float RandomSize(){
        float minPower = Mathf.Log(minSizeDeviation,2);
        float maxPower = Mathf.Log(maxSizeDeviation,2);
        float power = minPower + Random.value * (maxPower - minPower);
        float size = Mathf.Pow(2, power);
        return size;        
    }
    
    bool MoveToClearWater(GameObject obj){        
        Collider fc = (Collider)obj.GetComponentInChildren(typeof(Collider));        
        
        float objSize = fc.bounds.extents.magnitude;    
        
        Vector3 point = Vector3.zero;
        for(int i = 0; i < 100; i++){
            point = SpawnPoint();
            Collider[] cs = Physics.OverlapSphere(point, objSize * 2);
            if(cs.Length == 0){
                obj.transform.position = point;
                return true;
            }else{
                // print(string.Format("{0} collided with {1} at point {2}", obj.name, cs[0].gameObject.name, point));
            }                
        }
        
        print(string.Format("can't find place for object \"{0}\"", obj.name));
        return false;        
    }

    Vector3 SpawnPoint(){
        Vector3 randomPointInUnitCube = new Vector3(Random.value, Random.value, Random.value);
        Vector3 randomPointAround0 = randomPointInUnitCube - new Vector3(0.5f, 0.5f, 0.5f);
        return transform.position + Vector3.Scale(randomPointAround0, spawnVolumeBounds);
    }
    
    ArrayList LiveChildren(){
        ArrayList live = new ArrayList();
        foreach(GameObject child in children) if(child) live.Add(child);
        return live;
    }
    
    void AddChild(GameObject instance){
        ArrayList live = LiveChildren();
        live.Add(instance);
        children = (GameObject[])live.ToArray(typeof(GameObject));
    }
    
    void MaintainPopulation(){
        ArrayList live = LiveChildren();        
        int objectsToSpawn = population - live.Count;
        for(int i = 0; i < objectsToSpawn; i++) {            
            GameObject instance = Spawn();
            // print(string.Format("spawned additional {0}", instance.name));
        }
    }
}
