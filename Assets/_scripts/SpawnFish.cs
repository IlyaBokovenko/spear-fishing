using UnityEngine;
using System.Collections;

public class SpawnFish : MonoBehaviour {
    public GameObject fishSample;
    public int population  = 20;
    public Vector3 spawnVolumeBounds  = new Vector3(50, 50, 50);
    public float minSizeDeviation = 0.5f;
    public float maxSizeDeviation = 2.0f;
    
    private string cloneName;
    
    void Start(){
        for(int i = 0; i < population; i++){
            Spawn();
        }         
    }
    
    private void OnDrawGizmosSelected(){    
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, spawnVolumeBounds);
	}

    ////////////////////////////////////////////////////////////////////////
	void Spawn(){
        Quaternion rotation = Random.rotation;        
        GameObject fish  = (GameObject)Instantiate(fishSample, new Vector3(10000, 10000, 10000), rotation);
        cloneName = fish.name = fishSample.name;          
        DeathNotifier notifier = (DeathNotifier)fish.AddComponent(typeof(DeathNotifier));
        notifier.notefee = this;        
        FishAI fishComponent  = (FishAI)fish.GetComponent(typeof(FishAI));
        fishComponent.setSize(RandomSize());        
        MoveToClearWater(fish);        
    }

    float RandomSize(){
        float minPower = Mathf.Log(minSizeDeviation,2);
        float maxPower = Mathf.Log(maxSizeDeviation,2);
        float power = minPower + Random.value * (maxPower - minPower);
        float size = Mathf.Pow(2, power);
        return size;        
    }
    
    void MoveToClearWater(GameObject obj){        
        Collider fc = (Collider)obj.GetComponentInChildren(typeof(Collider));        
        
        float objSize = fc.bounds.extents.magnitude;    
        
        Vector3 point = Vector3.zero;
        for(int i = 0; i < 300; i++){
            point = SpawnPoint();
            Collider[] cs = Physics.OverlapSphere(point, objSize * 2);
            if(cs.Length == 0){
                obj.transform.position = point;
                return;
            }                
        }
        
        print("can't find place for fish");
    }

    Vector3 SpawnPoint(){
        Vector3 randomPointInUnitCube = new Vector3(Random.value, Random.value, Random.value);
        Vector3 randomPointAround0 = randomPointInUnitCube - new Vector3(0.5f, 0.5f, 0.5f);
        return transform.position + Vector3.Scale(randomPointAround0, spawnVolumeBounds);
    }
    
    void OnObjectDied(string objectName){
        if(objectName.Equals(cloneName)) { 
            Spawn();
        }
    }
}
