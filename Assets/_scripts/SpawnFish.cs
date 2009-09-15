using UnityEngine;
using System.Collections;

public class SpawnFish : MonoBehaviour {
    public GameObject fishSample;
    public int population  = 20;
    public Vector3 spawnVolumeBounds  = new Vector3(50, 50, 50);
    public float minSizeDeviation = 0.5f;
    public float maxSizeDeviation = 2.0f;
    
    private string cloneName;

    IEnumerator Start(){
        for(int i = 0; i < population; i++){
            yield return StartCoroutine("Spawn");
        }         
    }
    
    private void OnDrawGizmosSelected(){    
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, spawnVolumeBounds);
	}

    ////////////////////////////////////////////////////////////////////////

    IEnumerator Spawn(){
        Quaternion rotation = Random.rotation;
        Vector3 position = SpawnPoint();
        GameObject fish  = (GameObject)Instantiate(fishSample, position, rotation);
        cloneName = fish.name;          
        DeathNotifier notifier = (DeathNotifier)fish.AddComponent(typeof(DeathNotifier));
        notifier.notefee = this;
        
        FishAI fishComponent  = (FishAI)fish.GetComponent(typeof(FishAI));
        float minPower = Mathf.Log(minSizeDeviation,2);
        float maxPower = Mathf.Log(maxSizeDeviation,2);
        float power = minPower + Random.value * (maxPower - minPower);
        fishComponent.setSize(Mathf.Pow(2, power));               
        
        while(true){
            yield return new WaitForSeconds(0.2f);
            Bounds b = new Bounds(transform.position, spawnVolumeBounds);
            if(b.Contains(fish.transform.position))
                break;
                
            print("correcting spawned fish position");
            fish.transform.position = SpawnPoint();
        }
    }


    Vector3 SpawnPoint(){
        Vector3 randomPointInUnitCube = new Vector3(Random.value, Random.value, Random.value);
        Vector3 randomPointAround0 = randomPointInUnitCube - new Vector3(0.5f, 0.5f, 0.5f);
        return transform.position + Vector3.Scale(randomPointAround0, spawnVolumeBounds);
    }
    
    IEnumerator OnObjectDied(string objectName){
        if(objectName.Equals(cloneName)) {            
            yield return StartCoroutine("Spawn");
        }
    }
}
