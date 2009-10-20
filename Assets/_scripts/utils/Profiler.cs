using System.Collections;
using System;
using UnityEngine;

public enum PT{
    Wandering,
    ExecBehs,
    Seeking,
    Velocity,
    Yawing,
    Idle,
    CollisionAvoiding,
    LookAhead,
    RollMatching,
    OrientationMatching,
    Escape,
    CA_Cast,
    CA_Change,
    CA_Process
}

public class Profiler
{
    public struct ProfilePoint
    {
        public DateTime lastRecorded;
        public TimeSpan totalTime;
        public int totalCalls;
    }
    
    private static ProfilePoint[] profiles = new ProfilePoint[Enum.GetValues(typeof(PT)).Length];
    private static DateTime startTime = DateTime.UtcNow;

    private Profiler()
    {
    }
    public static void StartProfile(PT tag)
    {
        // profiles[(int)tag].lastRecorded = DateTime.UtcNow;
    }

    public static void EndProfile(PT etag)
    {
        // int tag = (int)etag;
        // profiles[tag].totalTime += DateTime.UtcNow - profiles[tag].lastRecorded;
        // ++profiles[tag].totalCalls;
    }

    public static void Reset()
    {
        // profiles = new ProfilePoint[Enum.GetValues(typeof(PT)).Length];
        // startTime = DateTime.UtcNow;
    }

    public static void PrintResults()
    {
        TimeSpan endTime = DateTime.UtcNow - startTime;
        System.Text.StringBuilder output = new System.Text.StringBuilder();
        output.Append("============================\n\t\t\t\tProfile results:\n============================\n");
        foreach(int val in Enum.GetValues(typeof(PT)))
        {
            string name = Enum.GetName(typeof(PT), val);
            ProfilePoint p  = profiles[val];                        
            double totalTime = p.totalTime.TotalSeconds;
            int totalCalls = p.totalCalls;
            if (totalCalls < 1) continue;
            output.Append("\nProfile ");
            output.Append(name);
            output.Append(" took ");
            output.Append(totalTime.ToString("F5"));
            output.Append(" (" + (100.0f * totalTime / endTime.TotalSeconds).ToString("F3") + "%)");
            output.Append(" seconds to complete over ");
            output.Append(totalCalls);
            output.Append(" iteration");
            if (totalCalls != 1) output.Append("s");
            output.Append(", averaging ");
            output.Append((totalTime / totalCalls).ToString("F5"));
            output.Append(" seconds per call");
        }
        output.Append("\n\n============================\n\t\tTotal runtime: ");
        output.Append(endTime.TotalSeconds.ToString("F3"));
        output.Append(" seconds\n============================");
        Debug.Log(output.ToString());
    }
}