var fishSample : GameObject;
var population : int = 20;
var spawnVolumeCenter : Vector3 = Vector3(50, 50, 50);
var spawnVolumeBounds : Vector3 = Vector3(50, 50, 50);

function Update () {    
    // while(IsNeedToSpawn())
    //     spawnFish();    
}

function Start(){
    while(IsNeedToSpawn())
        SpawnFish();
}

////////////////////////////////////////////////////////////////////////

function IsNeedToSpawn() : boolean{
    return Fishes().length < population;
}

function SpawnFish(){
    var rotation = Random.rotation;
    //var rotation = Quaternion.identity;
    var fish : GameObject = Instantiate(fishSample, SpawnPoint(), rotation);
    var fishComponent : FishAI = fish.GetComponent(FishAI);
    fishComponent.setSize(Mathf.Pow(2, Random.value - Random.value));
}

function Fishes(){
    var allFishes = gameObject.FindGameObjectsWithTag("Fish");
    var liveFishes = Array();
    for(var fish in allFishes){
        var fishComponent : FishAI = fish.GetComponent(FishAI);
        if(!fishComponent.IsDead()){
            liveFishes.Push(fish);
        }
    }   
    
    return liveFishes;
}

function SpawnPoint() : Vector3{
    var randomPointInUnitCube = Vector3(Random.value, Random.value, Random.value);
    var randomPointAround0 = randomPointInUnitCube - Vector3(0.5, 0.5, 0.5);
    return spawnVolumeCenter + Vector3.Scale(randomPointAround0, spawnVolumeBounds);
}
