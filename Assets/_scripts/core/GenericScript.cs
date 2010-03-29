using UnityEngine;
using System.Collections;

public class GenericScript : MonoBehaviour {

	public virtual void SelfDestroy()
	{
	    Destroy(this);
	}
	
	public void DestroyGameObject(){
	    if(gameObject != null)  {
			Destroy(gameObject);
		}
	}
}
