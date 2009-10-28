using UnityEngine;

public delegate void OnValueChangedDelegate(object value);

public class ValueHolder
{
    protected object _value;
    
    OnValueChangedDelegate del;
    
    public ValueHolder(object init){
        _value = init;
    }    
    
    public virtual object value
    {
        get{return _value;}
        set{            
            bool changed = !_value.Equals(value);
            _value = value;
            if(changed){
                if(del != null)del(_value);                
            }                
        }
    }
    
    public void Subscribe(OnValueChangedDelegate d){
        if(del == null)
            del = d;
        else
            del += d;
            
        d(value);
    }
    
    public void Unsubscribe(OnValueChangedDelegate d){
        if(del != null)
            del -= d;
    }
    
    public static implicit operator bool(ValueHolder vh){
        return (bool)vh.value;
    }
    public static implicit operator int(ValueHolder vh){
        return (int)vh.value;
    }
    public static implicit operator float(ValueHolder vh){
        return (float)vh.value;
    }
    public static implicit operator string(ValueHolder vh){
        return (string)vh.value;
    }
}

public class PrefHolder : ValueHolder{
    System.Type t;
    string name;
    
    private PrefHolder(string _name, object init)
        :base(init)
    {
        t = init.GetType();
        name = _name;
    }
    
    public static PrefHolder newInt(string name, int def){
         return new PrefHolder(name, PlayerPrefs.GetInt(name, def));
    }    
    public static PrefHolder newFloat(string name, float def){
         return new PrefHolder(name, PlayerPrefs.GetFloat(name, def));
    }     
    public static PrefHolder newString(string name, string def){
         return new PrefHolder(name, PlayerPrefs.GetString(name, def));
    }        
    public static PrefHolder newBool(string name, bool def){
         return new PrefHolder(name, PlayerPrefs.GetInt(name, def ?  1 : 0) == 0 ? false : true);
    }
    
    public override object value
    {
        set{
            CheckType(value);
            if(!_value.Equals(value)) SetPref(value);
            base.value = value;            
        }
    }
    
    public object Reload(object def){
        if(t == typeof(int))    base.value = PlayerPrefs.GetInt(name, (int)def);
        else if(t == typeof(bool))    base.value = (PlayerPrefs.GetInt(name, (bool)def ? 1 : 0) == 0) ? false : true;
        else if(t == typeof(float)) base.value = PlayerPrefs.GetFloat(name, (float)def);
        else if(t == typeof(string)) base.value = PlayerPrefs.GetString(name, (string)def);
        else {
            Debug.LogError("type error in PrefHolder: got '" + def + "', expected value of type " + t);
            Debug.Break();            
        }
        return value;        
    }
    
    public bool Exists(){
        return PlayerPrefs.HasKey(name);
    }
    
    public void Delete(){
        PlayerPrefs.DeleteKey(name);
    }
    
    void SetPref(object obj){
        if(t == typeof(int))    PlayerPrefs.SetInt(name, (int)obj);
        else if(t == typeof(bool))  PlayerPrefs.SetInt(name, (bool)obj ?  1 : 0);
        else if(t == typeof(float)) PlayerPrefs.SetFloat(name, (float)obj);
        else if(t == typeof(string)) PlayerPrefs.SetString(name, (string)obj);
        else {
            Debug.LogError("type error in PrefHolder: got '" + obj + "', expected value of type " + t);
            Debug.Break();            
        }        
    }
    
    void CheckType(object obj){
        if(obj.GetType() != t){
            Debug.LogError("type error in PrefHolder: got '" + obj + "', expected value of type " + t);
            Debug.Break();
        }
    }    
}