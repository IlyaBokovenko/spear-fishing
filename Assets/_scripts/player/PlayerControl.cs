using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour, IBitable {
	private float swimSpeed = 0.5f;
	private float boostSpeed = 2.0f;
	
	private float sensitivity = 2.0f;
	private Quaternion defaultGunPosition;
	private GameMaster gameMaster = null;
	private HUD hud = null;
	
	public GUIStyle menuTextStyle;
	public Speargun gun;
	public GameObject[] ignoreObjects;
	
	public AudioClip yam;
	
	private Transform goTransform;
	private Transform gunTransform;
	private Vector3 defaultPosition;
	private Transform defaultTransform;
	
	private ControlButton buttonFire;
	private ControlButton buttonAim;
	private ControlButton buttonBoost;
	
	//Keys
	public bool isBoost
	{
	    get{return buttonBoost != null && buttonBoost.isDown;}
	}
	
	void Awake () {
		goTransform = transform;
		gunTransform = gun.transform;
	}
	
	void Start() {
		foreach(GameObject element in ignoreObjects) {
			Physics.IgnoreCollision(element.collider, collider);
		}
		if(gun)
			defaultGunPosition = gunTransform.localRotation;
		defaultPosition = new Vector3(-0.7f, 0.0f, -0.7f);
		gameMaster = (GameMaster)gameObject.GetComponent(typeof(GameMaster));
		hud = (HUD)gameObject.GetComponent(typeof(HUD));	
	}
	
	void Update () {
		if(gun.animation.isPlaying) return;
        if(!gameMaster.isGame) return;		
			
		if(Application.platform == RuntimePlatform.OSXEditor) {
			if(Input.GetMouseButton(0)) {
				goTransform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * 10.0f, Space.World);
				goTransform.Rotate(Vector3.right * Input.GetAxis("Mouse Y") * 10.0f);
				RestrictRotation();
			}
			if(gun && Input.GetKey(KeyCode.LeftShift)) {
				gunTransform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * 10.0f, Space.World);
				gunTransform.Rotate(Vector3.right * Input.GetAxis("Mouse Y") * 10.0f);					
				Vector3 point = camera.WorldToScreenPoint(gunTransform.TransformPoint(- Vector3.forward * 5.0f));
				buttonAim.setDown(true);
				if(hud)
					hud.setCrosshair(new Vector2(point.x,point.y));
			} else {
				gunTransform.localRotation = defaultGunPosition;
				Vector3 point = camera.WorldToScreenPoint(gunTransform.TransformPoint(- Vector3.forward * 5.0f));
				buttonAim.setDown(false);
				if(hud)
					hud.setCrosshair(new Vector2(point.x,point.y));
			}
			
			buttonFire.setDown(Input.GetKeyUp ("space"));				
			buttonBoost.setDown(Input.GetMouseButton(1));
			
		} else {
		    buttonBoost.UpdateState();				
			buttonAim.UpdateState();
			buttonFire.UpdateState();			
		    
			Vector3 accelerator = gravityFilter(iPhoneInput.acceleration - defaultPosition);
			goTransform.Rotate(- Vector3.up * accelerator.y * Time.deltaTime * sensitivity, Space.World);
			goTransform.Rotate( Vector3.right * accelerator.x * Time.deltaTime * sensitivity);
			RestrictRotation();
			
			if(gunTransform.localRotation != defaultGunPosition) {	
				gunTransform.localRotation = defaultGunPosition;
				Vector3 point = camera.WorldToScreenPoint(gunTransform.TransformPoint(- Vector3.forward * 5.0f));
				if(hud)
					hud.setCrosshair(new Vector2(point.x,point.y));
			}				
			
			if(buttonAim.isDown){
				Vector2 deltaAim = buttonAim.TouchOffset(); 
				gunTransform.Rotate(Vector3.up * deltaAim.x * 0.5f);
				gunTransform.Rotate( Vector3.right * deltaAim.y * 0.5f);
				Vector3 point = camera.WorldToScreenPoint(gunTransform.TransformPoint(- Vector3.forward * 5.0f));
				if(hud)
					hud.setCrosshair(new Vector2(point.x,point.y));				    
			}				
		}
		goTransform.Translate(Vector3.forward * (this.isBoost ? boostSpeed : swimSpeed) * Time.deltaTime);
		
	}
	
	void OnTriggerEnter(Collider other){
	    if(other.tag.Equals("Health") ){
	        audio.PlayOneShot(yam);
	        other.gameObject.SendMessage("Yam");
	        gameMaster.AddHealth();
	    }
	}
	
	
	public void setAimButtonControl(ControlButton arg) { buttonAim = arg; }
	public void setFireButtonControl(ControlButton arg) {
	     buttonFire = arg; 
	     buttonFire.AddPressedDelegate(new OnPressedDelegate(this.OnFire));    
    }
	public void setBoostButtonControl(ControlButton arg) { buttonBoost = arg; }

	public void OnHit(Spear spear) {}
	
	public void OnBite() { 
		if(gameMaster)
			gameMaster.DoBite();
	} 
	
	void OnFire(bool down){
	    if(down)
	        Fire();
	}

	void Fire(){	    
        if(gun && !gameMaster.isSurface) {            
			gun.Fire();
		}
	}
	
	public bool IsBoost() {
		return isBoost;
	}
	
	Vector3 gravityFilter(Vector3 arg) {
		Vector3 result = Vector3.zero;
		result.x = Mathf.Round(arg.x * 100.0f);
		result.y = Mathf.Round(arg.y * 100.0f);
		result.z = Mathf.Round(arg.z * 100.0f);
		return result;
	}
	
	void RestrictRotation(){
	    Vector3 angles = goTransform.eulerAngles;
		float pitch = goTransform.eulerAngles.x;
		pitch = Utils.DegToShifted(pitch);
		pitch = Mathf.Min(Mathf.Abs(pitch), 75) * Mathf.Sign(pitch);					
		angles.x = pitch;
		angles.z = 0;
		goTransform.eulerAngles = angles;		
	}
}
