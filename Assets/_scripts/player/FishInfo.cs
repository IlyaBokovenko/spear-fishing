using UnityEngine;
using System.Collections;

public class FishInfo {
	public static float Grouper = 100.0f;
	public static float RedSnapper = 120.0f;
	public static float YellowFinTuna = 140.0f;
	
	public static string GROUPER = "Grouper";
	public static string REDSNAPPER = "RedSnapper";
	public static string YELLOWFINTUNA = "YellowFinTuna";
	
	private int[] fishCount;
 	private float[] fishWeight;

 	public static float getInfo(string type, float koef) {
		if(type == GROUPER)
			return (Grouper * Mathf.Pow(koef, 3));
		if(type == REDSNAPPER)
			return (RedSnapper * Mathf.Pow(koef, 3));
		if(type == YELLOWFINTUNA)
			return (YellowFinTuna  * Mathf.Pow(koef, 3));
		return 0;
	}
	
	public FishInfo(ArrayList fishes) {
		fishCount = new int[3];
		fishWeight = new float[3];
		foreach(string fish in fishes) {
			string[] param = fish.Split(":"[0]);
			if(GROUPER == param[0]) {
				fishCount[0]++;
				fishWeight[0] += FishInfo.getInfo(param[0], float.Parse(param[1]));
			}
			if(REDSNAPPER == param[0]) {
				fishCount[1]++;
				fishWeight[1] += FishInfo.getInfo(param[0], float.Parse(param[1]));
			}
			if(YELLOWFINTUNA == param[0]) {
				fishCount[2]++;
				fishWeight[2] += FishInfo.getInfo(param[0], float.Parse(param[1]));
			}
		}
	}
	
	public int getCount(string type) {
		if(type == GROUPER)
			return fishCount[0];
		if(type == REDSNAPPER)
			return fishCount[1];
		if(type == YELLOWFINTUNA)
			return fishCount[2];
		return 0;
	}
	
	public float getWeight(string type) {
		if(type == GROUPER)
			return fishWeight[0];
		if(type == REDSNAPPER)
			return fishWeight[1];
		if(type == YELLOWFINTUNA)
			return fishWeight[2];
		return 0;
	}
	
	public static string formatWeight(float value){
	    return string.Format("{0,1:00.0}", value);
	}
	
	public string getWeightString(string type){
	     return formatWeight(getWeight(type));
	}
	
	public static int getCount(string type, ArrayList fishes) {
		int result = 0;
		foreach(string fish in fishes) {
			string[] param = fish.Split(":"[0]);
			if(type == param[0]) {
				result++;
			}
		}
		return result;
	}
	
	public static int getWeight(string type, ArrayList fishes) {
		int result = 0;
		foreach(string fish in fishes) {
			string[] param = fish.Split(":"[0]);
			if(type == param[0]) {
				result += (int)FishInfo.getInfo(param[0], float.Parse(param[1]));
			}
		}
		return result;
	}	
}
