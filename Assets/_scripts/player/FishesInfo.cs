using UnityEngine;
using System.Collections;

public class FishesInfo {
	public static int TYPE_COUNT = 3;
	public static float GROUPER_WEIGHT = 100.0f;
	public static float REDSNAPPER_WEIGHT = 120.0f;
	public static float YELLOWFINTUNA_WEIGHT = 140.0f;
	
	public static string GROUPER = "Grouper";
	public static string REDSNAPPER = "RedSnapper";
	public static string YELLOWFINTUNA = "YellowFinTuna";
	
	public int FishesCount {get{return fishes != null ? fishes.Count : 0;}}
	
	private ArrayList fishes;
	private int[] cache_count;
	private float[] cache_weight;
	
	public FishesInfo() {
		fishes = new ArrayList();
		cache_count = new int[TYPE_COUNT];
		cache_weight = new float[TYPE_COUNT];
	}
	
	public static float getFishWeight(string arg) {
		if(arg == GROUPER) {
			return GROUPER_WEIGHT;
		} else if(arg == REDSNAPPER) {
			return REDSNAPPER_WEIGHT;
		} else if(arg == YELLOWFINTUNA) {
			return YELLOWFINTUNA_WEIGHT;
		}
		return 0.0f;
	}
	
	public int getDifferentFishesCount() {
		int result = 0;
		for(int index = 0; index < TYPE_COUNT; index++) {
			if(cache_count[index] > 0) {
				result++;
			}
		}
		return result;
	}
	
	public void addFish(string arg) {
		Fish fish = new Fish(arg);
		addToCache(fish);
		fishes.Add(fish);
	}
	
	private void addToCache(Fish fish) {
		int index = getTypeId(fish.Type);
	    if(index < TYPE_COUNT) {
			cache_weight[index] += fish.Weight;
			cache_count[index] ++;
		}
	}
	
	public int getCountByType(string type) {
		int index = getTypeId(type);
		int result = 0;
		if(index < TYPE_COUNT) {
			result = cache_count[index];
		}
		return result;
	}
	
	public float getWeightByType(string type) {
		int index = getTypeId(type);
		float result = 0.0f;
		if(index < TYPE_COUNT) {
			result = cache_weight[index];
		}
		return result;
	}
	
	public float getWeight() {
		float result = 0.0f;
		if(fishes != null) {
			foreach(Fish fish in fishes) {
				result += fish.Weight;
			}
		}
		return result;
	}
	 
	public string getWeightToString() {
		return formatWeight(getWeight());
	}
	
	public string getWeightByTypeToString(string type) {
		return formatWeight(getWeightByType(type));
	}
	
	public static string formatWeight(float value){
	    return string.Format("{0,1:00.0}", value);
	}
	
	private int getTypeId(string type) {return type == GROUPER ? 0 : type == REDSNAPPER ? 1 : type == YELLOWFINTUNA ? 2 : 3;}
	
	public void Save() {
		string result = "";
		foreach(Fish fish in fishes) {
			result += fish.Packing + ";";
		}
		PlayerPrefs.SetString("fishes", result);
	}
	
	public void SaveTo(string key) {
		string result = "";
		foreach(Fish fish in fishes) {
			result += fish.Packing + ";";
		}
		PlayerPrefs.SetString(key, result);
	}
	
	public int LoadFrom(string key) {
		int result = 0;
		if(PlayerPrefs.HasKey(key)) {
			string[] arrayFishes = PlayerPrefs.GetString(key).Split(";"[0]);
			fishes.Clear();
			foreach(string param in arrayFishes) {
				if(param != "") {
					Fish fish = new Fish(param);
					addToCache(fish);
					fishes.Add(fish);
					result++;
				}
			}     
		}
		return result;
	}
	
	public int Load() {
		int result = 0;
		if(PlayerPrefs.HasKey("fishes")) {
			string[] arrayFishes = PlayerPrefs.GetString("fishes").Split(";"[0]);
			fishes.Clear();
			foreach(string param in arrayFishes) {
				if(param != "") {
					Fish fish = new Fish(param);
					addToCache(fish);
					fishes.Add(fish);
					result++;
				}
			}     
		}
		return result;
	}
}

public class Fish {
	private string type;
	private float weight;
	                        
	public string Type {get{return type;}}
	public float Weight {get{return weight;}}
	public string Packing {get{return type + ":" + weight;}}	
	
	public Fish(string arg_type, float arg_weight) {
		type = arg_type;
		weight = arg_weight;
	}
	
	public Fish(string arg) {
		string[] param = arg.Split(":"[0]);
		if(param.Length > 1) {
			type = param[0];
			weight = float.Parse(param[1]);
		}
	}
}
