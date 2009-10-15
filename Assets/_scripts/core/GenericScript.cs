using UnityEngine;
using System.Collections;

public class GenericScript : MonoBehaviour {

	public virtual void SelfDestroy()
	{
	    Destroy(this);
	}
	
	public void DestroyGameObject(){
	    Destroy(gameObject);
	}
}
