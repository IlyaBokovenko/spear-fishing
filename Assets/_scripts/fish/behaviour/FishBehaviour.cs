using UnityEngine;
using System.Collections;

public class FishBehaviour : GenericScript
{    
    public virtual SteeringOutput GetSteering(){
        return new SteeringOutput();
    }
    
    protected virtual ArrayList children
    {
        get{return new ArrayList();}
    }
    
    public override string ToString(){
        string name = GetType().Name;
        name = name.Replace("Fish", "");
        name = name.Replace("Behaviour", "");
        return name;
    }
    
    public virtual string ToStringWithChildren(int indentationLevel){
        string ret = new string("\t"[0], indentationLevel) + ToString();
        foreach(FishBehaviour child in children){
            ret += "\n" + child.ToStringWithChildren(indentationLevel + 1);
        }
        
        return ret;
    }
    
    public string ToStringWithChildren(){
        return ToStringWithChildren(0);
    }
}
