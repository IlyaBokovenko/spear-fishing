using UnityEngine;
using System.Collections;

public class SpawnFish : MonoBehaviour {
    public GameObject fishSample;
    public int population  = 20;
    public Vector3 spawnVolumeBounds  = new Vector3(50, 50, 50);
    public float minSizeDeviation = 0.5f;
    public float maxSizeDeviation = 2.0f;

    void Update () {    
        // while(IsNeedToSpawn())
        //     spawnFish();    
    }

    void Start(){
        while(IsNeedToSpawn())
            Spawn();
    }
    
    private void OnDrawGizmosSelected(){    
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, spawnVolumeBounds);
	}

    ////////////////////////////////////////////////////////////////////////

    bool IsNeedToSpawn(){
        return Fishes().Length < population;
    }

    void Spawn(){
        Quaternion rotation = Random.rotation;
        GameObject fish  = (GameObject)Instantiate(fishSample, SpawnPoint(), rotation);
        FishAI fishComponent  = (FishAI)fish.GetComponent(typeof(FishAI));
        float minPower = Mathf.Log(minSizeDeviation,2);
        float maxPower = Mathf.Log(maxSizeDeviation,2);
        float power = minPower + Random.value * (maxPower - minPower);
        fishComponent.setSize(Mathf.Pow(2, power));
    }

    GameObject[] Fishes(){
        GameObject[] allFishes = GameObject.FindGameObjectsWithTag("Fish");
        ArrayList liveFishes = new ArrayList();
        foreach(GameObject fish in allFishes){
            FishAI fishComponent  = (FishAI)fish.GetComponent(typeof(FishAI));
            if(!fishComponent.IsDead()){
                liveFishes.Add(fish);
            }
        }   

        return (GameObject[])liveFishes.ToArray(typeof(GameObject));
    }

    Vector3 SpawnPoint(){
        Vector3 randomPointInUnitCube = new Vector3(Random.value, Random.value, Random.value);
        Vector3 randomPointAround0 = randomPointInUnitCube - new Vector3(0.5f, 0.5f, 0.5f);
        return transform.position + Vector3.Scale(randomPointAround0, spawnVolumeBounds);
    }
}
