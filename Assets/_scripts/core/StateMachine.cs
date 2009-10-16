using UnityEngine;
using System.Collections;

public class StateMachine{
    State state;
    
    public State curState
    {
        get{return state;}
    }
    
    public void MoveTo(State _state){        
        if(state != null) state.Exit();
        state = _state;
        if(state != null) {
            state.sm = this;
            state.Enter();
        }
    }
    
    public void Do(){
        if(state != null) state.Do();
    }
    
    public void OnGUI(){
        if(state != null) state.OnGUI();
    }
}

public class State{
    protected internal StateMachine sm;    
    
    public virtual void Enter(){
        
    }
    
    public virtual void Do(){
        
    }
    
    public virtual void Exit(){
        
    }
    
    public virtual void OnGUI(){
        
    }
}
