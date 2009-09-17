using UnityEngine;
using System.Collections;

public class FishBehaviour : GenericScript
{    
    public virtual SteeringOutput GetSteering(){        
        return SteeringOutput.empty;
    }
    
    protected FishBehaviour[] children = new FishBehaviour[0];
    protected virtual ArrayList ActiveChildren(){
        return new ArrayList(children);
    }
    
    public void RemoveChildrenFrom(ArrayList behs){
       foreach(FishBehaviour child in children) {
           child.RemoveChildrenFrom(behs);
           behs.Remove(child);
       }
    }
    
    public override string ToString(){
        string name = GetType().Name;
        name = name.Replace("Fish", "");
        name = name.Replace("Behaviour", "");
        return name;
    }
    
    public virtual string ToStringWithChildren(int indentationLevel){
        string ret = new string("\t"[0], indentationLevel) + ToString();
        foreach(FishBehaviour child in ActiveChildren()){
            ret += "\n" + child.ToStringWithChildren(indentationLevel + 1);
        }
        
        return ret;
    }
    
    public string ToStringWithChildren(){
        return ToStringWithChildren(0);
    }
    
    public override void SelfDestroy(){
        foreach(FishBehaviour child in children){
            if(child)
                child.SelfDestroy();
        }
        base.SelfDestroy();
    }
    
    protected virtual void PrivateDrawGizmosSelected(){        
    }
    
    public void DrawGizmosSelectedWithChildren(){
        if(!enabled)
            return;
        PrivateDrawGizmosSelected();
        foreach(FishBehaviour beh in ActiveChildren()){
            beh.DrawGizmosSelectedWithChildren();
        }        
    }
}
