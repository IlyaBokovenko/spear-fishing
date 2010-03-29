using UnityEngine;
using System.Collections;


public class Tasks {
    public enum TaskStates { COMPLETE, IN_PROCESS, FAIL, NONE, NEW }
    private int id = 0;
	private float timer;
	private float taskTime;    
    private FishesInfo fishesInfo;
    private ArrayList levelsTasks;
    private Task[] activeTasks;
    private TaskStates state;
    private ArrayList tasksBuffer;
	private GUIText tasksText;

    public static float[] tasksTime = {
		120.0f, 120.0f, 120.0f, 180.0f, 180.0f, 180.0f, 240.0f, 240.0f, 300.0f, 360.0f 
	};
	
	public TaskStates State {get{return state;}}

	public Tasks(int arg_id, GameObject arg_go) {
		id = arg_id;
		if(arg_go != null) {
			tasksText = (GUIText)arg_go.GetComponent(typeof(GUIText));
		}
		
		InitTasks();
		InitDefault(id);
		state = TaskStates.NONE;
	}
	
	public void InitDefault(int arg) {
		timer = 0.0f;
		fishesInfo = new FishesInfo();
		taskTime = (arg < tasksTime.Length) ? tasksTime[arg] : 120.0f;
		activeTasks = (arg < levelsTasks.Count) ? (Task[])levelsTasks[arg] : null;
		CheckTasks();
	}
    
	public void StartTask(int arg) {
		id = arg;
		InitDefault(id);
		state = TaskStates.NEW;
		clearPlayerPrefs();
	}
	
	public void StartTask() {
		StartTask(id);
	}
	
	public void clearPlayerPrefs() {
		PlayerPrefs.DeleteKey("tasksTimer");
		PlayerPrefs.DeleteKey("tasksFishes");
	}
	
	public void addFish(string arg) {
		fishesInfo.addFish(arg);
		CheckTasks();
	}
	
	public void Save() {
		PlayerPrefs.SetFloat("tasksTimer", timer);
		fishesInfo.SaveTo("tasksFishes");
	}
	public void Load() {
		timer = PlayerPrefs.HasKey("tasksTimer") ? PlayerPrefs.GetFloat("tasksTimer") : 0.0f;
		fishesInfo.LoadFrom("tasksFishes");
		CheckTasks();
		Update(0.0f);
	}
	
	private void InitTasks() {
		levelsTasks = new ArrayList();
		//1
		Task[] level1 = {
			new Task(Task.Type.COUNT, 3, "Catch fish")
		};
		levelsTasks.Add(level1);
		//2
		Task[] level2 = {
			new Task(Task.Type.WEIGHT, 15.0f, "Catch weight")
		};
		levelsTasks.Add(level2);
		//3
		Task[] level3 = {
			new Task(Task.Type.DIFFERENT, 2, "Different fish")
		};
		levelsTasks.Add(level3);
		//4
		Task[] level4 = {
			new Task(Task.Type.DIFFERENT, 3, "Different fish")
		};
		levelsTasks.Add(level4);
		//5
		Task[] level5 = {
			new Task(Task.Type.GROUPER, 2, "Grouper"),
			new Task(Task.Type.REDSNAPPER, 2, "Red Snapper")
		};
		levelsTasks.Add(level5);
		//6
		Task[] level6 = {
			new Task(Task.Type.GROUPER, 2, "Grouper"),
			new Task(Task.Type.REDSNAPPER, 2, "Red Snapper"),
			new Task(Task.Type.YELLOWFINTUNA, 1, "Yellowfin tuna")
		};
		levelsTasks.Add(level6);
		//7
		Task[] level7 = {
			new Task(Task.Type.GROUPER, 3, "Grouper"),
			new Task(Task.Type.REDSNAPPER, 3, "Red Snapper"),
			new Task(Task.Type.YELLOWFINTUNA, 3, "Yellowfin tuna")
		};
		levelsTasks.Add(level7);
		//8
		Task[] level8 = {
			new Task(Task.Type.DIFFERENT, 3, "Different fish"),
			new Task(Task.Type.WEIGHT, 50.0f, "Catch weight")
		};
		levelsTasks.Add(level8);
		//9
		Task[] level9 = {
			new Task(Task.Type.GROUPER, 3, "Grouper"),
			new Task(Task.Type.REDSNAPPER, 3, "Red Snapper"),
			new Task(Task.Type.YELLOWFINTUNA, 3, "Yellowfin tuna"),
			new Task(Task.Type.WEIGHT, 50.0f, "Catch weight")
		};
		levelsTasks.Add(level9);
	    //10
		Task[] level10 = {
			new Task(Task.Type.WEIGHT, 90.0f, "Catch weight")
		};
		levelsTasks.Add(level10);
    }
	
	public void CheckTasks() {
		if(tasksBuffer != null) {
			tasksBuffer.Clear();
		} else {
			tasksBuffer = new ArrayList();
		}
    	state = TaskStates.IN_PROCESS;
    	if(activeTasks != null) {
			int activeTasksCount = 0;
			foreach(Task task in activeTasks) {
				if(!task.Check(fishesInfo)) {
					tasksBuffer.Add(task.getTitle(fishesInfo));
					activeTasksCount++;
				}
			}
			if(activeTasksCount <= 0) {
				state = TaskStates.COMPLETE;
			}
		} else {
			state = TaskStates.NONE;
		}
	}
	
	public void setStarted() {
		state = TaskStates.IN_PROCESS;
	}
	
	public void Show() {
		if(tasksText != null) {
			tasksText.enabled = true;
		}
	}
	
	public void Hide() {
		if(tasksText != null) {
			tasksText.enabled = false;
		}
	}
	
	public TaskStates Update(float arg) {
		string result = "";
		if(activeTasks != null && tasksText != null) {
			timer += arg;
			result += "Time : " + (int)(taskTime - timer) + "\n";
			foreach(string task in tasksBuffer) {
				result += task + "\n";
			}
			tasksText.text = result;
		}
		if(timer > taskTime) {
			state = TaskStates.FAIL;
		}
		return state;
	}
}

public class Task {
	public enum Type {COUNT, WEIGHT, GROUPER, REDSNAPPER, YELLOWFINTUNA, DIFFERENT}
	private Type type;
	private float value;
	private float tmp;
	private string title;
	
	
	public Task(Type arg_type, float arg_value, string arg_title) {
		type = arg_type;
		value = arg_value;
		title = arg_title;
	}
	
	public string getTitle(FishesInfo fishesInfo) {
		switch(type) {
			case Type.COUNT :
				return title + " : " + (int)(value - fishesInfo.FishesCount);
			case Type.WEIGHT :
				return title + " : " + FishesInfo.formatWeight(value - fishesInfo.getWeight());
			case Type.GROUPER :
				return title + " : " + (int)(value - fishesInfo.getCountByType(FishesInfo.GROUPER));
			case Type.REDSNAPPER :
				return title + " : " + (int)(value - fishesInfo.getCountByType(FishesInfo.REDSNAPPER)); 
    		case Type.YELLOWFINTUNA :
				return title + " : " + (int)(value - fishesInfo.getCountByType(FishesInfo.YELLOWFINTUNA));
			case Type.DIFFERENT :
			 	return title + " : " + (int)(value - fishesInfo.getDifferentFishesCount());
		}
		return title;
	}
	
	public bool Check(FishesInfo fishesInfo) {
		switch(type) {
			case Type.COUNT :
				return (int)value <= fishesInfo.FishesCount ? true : false;
			case Type.WEIGHT :
				return value <= fishesInfo.getWeight() ? true : false;
			case Type.GROUPER :
				return (int)value <= fishesInfo.getCountByType(FishesInfo.GROUPER) ? true : false;
			case Type.REDSNAPPER :
				return (int)value <= fishesInfo.getCountByType(FishesInfo.REDSNAPPER) ? true : false; 
    		case Type.YELLOWFINTUNA :
				return (int)value <= fishesInfo.getCountByType(FishesInfo.YELLOWFINTUNA) ? true : false; 
			case Type.DIFFERENT :
				return (int)value <= fishesInfo.getDifferentFishesCount() ? true : false;
		}
		return true;
	}
}