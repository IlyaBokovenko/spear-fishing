using UnityEngine;
using System.Collections;

public class SpawnFish : MonoBehaviour {
    public GameObject fishSample;
    public int population  = 20;
    public Vector3 spawnVolumeCenter  = new Vector3(50, 50, 50);
    public Vector3 spawnVolumeBounds  = new Vector3(50, 50, 50);

    void Update () {    
        // while(IsNeedToSpawn())
        //     spawnFish();    
    }

    void Start(){
        while(IsNeedToSpawn())
            Spawn();
    }

    ////////////////////////////////////////////////////////////////////////

    bool IsNeedToSpawn(){
        return Fishes().Length < population;
    }

    void Spawn(){
        Quaternion rotation = Random.rotation;
        GameObject fish  = (GameObject)Instantiate(fishSample, SpawnPoint(), rotation);
        FishAI fishComponent  = (FishAI)fish.GetComponent(typeof(FishAI));
        fishComponent.setSize(Mathf.Pow(2, Random.value - Random.value));
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
        return spawnVolumeCenter + Vector3.Scale(randomPointAround0, spawnVolumeBounds);
    }
}
