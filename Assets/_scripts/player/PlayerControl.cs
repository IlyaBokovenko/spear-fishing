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
	public GameObject gun;
	public GameObject[] ignoreObjects;
	
	private Transform goTransform;
	private Transform gunTransform;
	private Vector3 defaultPosition;
	private Transform defaultTransform;
	
	private ControlButton buttonFire;
	private ControlButton buttonAim;
	private ControlButton buttonBoost;
	
	private bool isEnabled = true;
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
		if(!gun.animation.isPlaying && isEnabled) {
			if(Application.platform == RuntimePlatform.OSXEditor) {
				if(Input.GetMouseButton(0)) {
					goTransform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * 10.0f, Space.World);
					goTransform.Rotate(Vector3.right * Input.GetAxis("Mouse Y") * 10.0f);
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
				Vector3 accelerator = gravityFilter(iPhoneInput.acceleration - defaultPosition);
				goTransform.Rotate(- Vector3.up * accelerator.y * Time.deltaTime * sensitivity, Space.World);
				goTransform.Rotate( Vector3.right * accelerator.x * Time.deltaTime * sensitivity);
				
				if(gunTransform.localRotation != defaultGunPosition) {	
					gunTransform.localRotation = defaultGunPosition;
					Vector3 point = camera.WorldToScreenPoint(gunTransform.TransformPoint(- Vector3.forward * 5.0f));
					if(hud)
						hud.setCrosshair(new Vector2(point.x,point.y));
				}
				
				buttonBoost.ProcessTouches();				
				buttonAim.ProcessTouches();
				buttonFire.ProcessTouches();			
				
				if(buttonAim.isDown){
					Vector2 deltaAim = new Vector2(buttonAim.touchPos.x - (buttonAim.rect.x + buttonAim.rect.width/2), buttonAim.touchPos.y - (buttonAim.rect.y + buttonAim.rect.height/2)); 
					gunTransform.Rotate(Vector3.up * deltaAim.x * 0.5f, Space.World);
					gunTransform.Rotate( - Vector3.right * deltaAim.y * 0.5f);
					Vector3 point = camera.WorldToScreenPoint(gunTransform.TransformPoint(- Vector3.forward * 5.0f));
					if(hud)
						hud.setCrosshair(new Vector2(point.x,point.y));				    
				}				
			}
			goTransform.Translate(Vector3.forward * (this.isBoost ? boostSpeed : swimSpeed) * Time.deltaTime);
		}
	}
	
	void OnTriggerEnter(Collider other){
	    if(other.tag.Equals("Health") ){	        
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

	public void setEnableControl(bool arg) { isEnabled = arg; }

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
			gun.animation.Play();
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
}
