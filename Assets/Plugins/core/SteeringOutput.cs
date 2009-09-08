using UnityEngine;
using System;
using System.Collections;

public struct SteeringOutput
{
	public static SteeringOutput empty
	{
		get {return new SteeringOutput(Vector3.zero, Vector3.zero, Vector3.zero);}
	}
	
	public const float forceTolerance = 0.0001f;
	public const float torqueTolerance = 0.0001f;
	public const float relativeTorqueTolerance = 0.0001f;
	
	private Vector3 force;
	private Vector3 torque;
	private Vector3 relativeTorque;
	
	public SteeringOutput(Vector3 _force, Vector3 _torque, Vector3 _relativeTorque){
		force = _force;
		torque = _torque;    
		relativeTorque = _relativeTorque;	
	}	
	public static SteeringOutput WithTorque(Vector3 t){
	    SteeringOutput inst = SteeringOutput.empty;
	    inst.AddTorque(t);
	    return inst;
	}
	
	public static SteeringOutput WithForce(Vector3 f){
	    SteeringOutput inst = SteeringOutput.empty;
	    inst.AddForce(f);
	    return inst;
	}
	
	public static SteeringOutput WithRelativeTorque(Vector3 rt){
	    SteeringOutput inst = SteeringOutput.empty;
	    inst.AddRelativeTorque(rt);
	    return inst;
	}
	
	public void AddForce(Vector3 f){
	    force += f;
	}
	
	public void AddTorque(Vector3 t){
	    torque += t;
	}
	
	public void AddRelativeTorque(Vector3 rt){
	    relativeTorque += rt;
	}
	
	public void ApplyTo(GameObject gameObject){
	    if(SignificantForce())
	        gameObject.rigidbody.AddForce(force * Time.deltaTime, ForceMode.VelocityChange);
	    if(SignificantTorque())
	        gameObject.rigidbody.AddTorque(torque * Time.deltaTime, ForceMode.VelocityChange);
	    if(SignificantRelativeTorque())
	        gameObject.rigidbody.AddRelativeTorque(relativeTorque * Time.deltaTime, ForceMode.VelocityChange);       
	}
	
	public bool SignificantForce(){
	    return force.magnitude > forceTolerance;
	}
	
	public bool SignificantTorque(){
	    return torque.magnitude > torqueTolerance;
	}
	
	public bool SignificantRelativeTorque(){
	    return relativeTorque.magnitude > relativeTorqueTolerance;
	}
	
	public bool Significant(){
	    return SignificantForce() || SignificantTorque() || SignificantRelativeTorque();        
	}
	
	public SteeringOutput CombinedWith(SteeringOutput other){
	    SteeringOutput ret = SteeringOutput.empty;
	    ret.force = force + other.force;
	    ret.torque = torque + other.torque;
	    ret.relativeTorque = relativeTorque + other.relativeTorque;
	    return ret;
	}
	
	public static SteeringOutput operator+(SteeringOutput one, SteeringOutput other){
		return one.CombinedWith(other);
	}
}


