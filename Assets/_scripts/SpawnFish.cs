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
            // StartCoroutine("Spawn");
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
    /*
	void Spawn() {
		Quaternion rotation = Quaternion.identity;//Random.rotation;        
        Vector3 position = transform.position;
		//position.x = Random.Range( -spawnVolumeBounds.x/2 * 10.0f, spawnVolumeBounds.x/2 * 10.0f) * 0.1f;
		//position.y = Random.Range( -spawnVolumeBounds.y/2 * 10.0f, spawnVolumeBounds.y/2 * 10.0f) * 0.1f;
		//position.z = Random.Range( -spawnVolumeBounds.z/2* 10.0f, spawnVolumeBounds.z/2 * 10.0f) * 0.1f;
		
		//position.y = 1.0f;
		
		GameObject fish  = (GameObject)Instantiate(fishSample, position, rotation);
        cloneName = fish.name;          
        DeathNotifier notifier = (DeathNotifier)fish.AddComponent(typeof(DeathNotifier));
        notifier.notefee = this;        
        FishAI fishComponent  = (FishAI)fish.GetComponent(typeof(FishAI));
        fishComponent.setSize(RandomSize());        
        
		//Debug.Log(fishSample.name + " respawn at : " + position);
		Debug.Break();
		//MoveToClearWater(fish);
	}//*/


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
        for(int i = 0; i < 100; i++){
            point = SpawnPoint();
            Collider[] cs = Physics.OverlapSphere(point, objSize);
            if(cs.Length == 0)
                break;
        }
        
        obj.transform.position = point;
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
