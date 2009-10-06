using UnityEngine;
using System.Collections;

public class FishArbitratedBehaviour : FishBehaviour {
    public int priority = 0;

	public static FishArbitratedBehaviour[][] GroupByPriorities(ArrayList behaviours){
		// map priority -> behaviours
		Hashtable dict = new Hashtable();
		foreach(FishArbitratedBehaviour beh in behaviours){
			int priority = beh.priority;
			ArrayList list;
			if(!dict.ContainsKey(priority)){
				list = new ArrayList();
				dict[priority] = list;
			}else{
				list = (ArrayList)dict[priority];
			}
			list.Add(beh);
		}
	

		// arrange behaviour groups in execution order (highest priorities first)
		ArrayList priorities = new ArrayList(dict.Keys);
		FishArbitratedBehaviour[][] grouped = new FishArbitratedBehaviour[priorities.Count][];
		
		priorities.Sort();
		priorities.Reverse();
		for(int i = 0; i < priorities.Count; i++){
		    int priority = (int)priorities[i];
		    ArrayList group = (ArrayList)dict[priority];
			grouped[i] = (FishArbitratedBehaviour[])group.ToArray(typeof(FishArbitratedBehaviour));
		}	
		
		return grouped;
	}
}
