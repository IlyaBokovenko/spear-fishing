using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour {    
	StateMachine sm;
    
    GameMaster gameMaster;   
    HUD hud; 
    
    bool isBlinkingDepthText = false;    

	HighlightableControlButton buttonFire;
	HighlightableControlButton buttonAim;
	HighlightableControlButton buttonBoost;
	GUIStyle depthText;
	GUIStyle healthText;
	
    public Texture2D arrow2D;
    public GameObject arrow3D;
	
	float depth
	{
	    get{return gameMaster.getDepth();}
	}
	

	// Use this for initialization
	void Start () {
	    gameMaster = (GameMaster)GetComponent(typeof(GameMaster));
	    hud = (HUD)GetComponent(typeof(HUD));
	    
	    sm = new StateMachine();
	    Invoke("BeginTutorial", 3.0f);	    
	}
	
	void BeginTutorial(){
	    sm.MoveTo(new SwimState(this));
	}
	
	void FixedUpdate(){
        sm.Do();        
    }
    	
	void OnGUI() {
	    if(hud.isGame)  sm.OnGUI();
    }
    
    
    //     //     //     //     // 
    
    // Property setters
	
	public void setAimButtonControl(HighlightableControlButton arg) { 
	    buttonAim = arg; 
	}
	public void setFireButtonControl(HighlightableControlButton arg) { 
	    buttonFire = arg; 
	}
	public void setBoostButtonControl(HighlightableControlButton arg) {
	    buttonBoost = arg; 
	}
	
	public void setDepthTextStyle(GUIStyle _depthText){
	    depthText = _depthText;
	}

	public void setHealthTextStyle(GUIStyle _healthText){
	    healthText = _healthText;
	}
		
	/////// State Machine
	
	class TutorialState : State{
        protected Tutorial context;

        public TutorialState(Tutorial _context){
            context = _context;
        }
    }

    class SwimState : TutorialState{
        const float DEPTH_TRESHOLD = 2.0f;

        float initialDepth, minDepth, maxDepth;
        bool isBlinkingDepthText = false;

        public SwimState(Tutorial context):base(context){}

        public override void OnGUI(){
            GUI.Box(new Rect(80, 119, 320, 22), "You can swim/dive by tilting your iPhone. Try it now.");        
        }

        public override void Enter(){
            initialDepth = minDepth = maxDepth = context.depth;
            StartBlinkingDepthText();
        }

        public override void Exit(){
            StopBlinkingDepthText();
        }

        public override void Do(){
            minDepth = Mathf.Min(minDepth, context.depth);
            maxDepth = Mathf.Max(maxDepth, context.depth);

            if(initialDepth - minDepth > DEPTH_TRESHOLD ||
                maxDepth - initialDepth > DEPTH_TRESHOLD ){
                    sm.MoveTo(new BoostState(context));                    
                }
        }

        // private
    	void StartBlinkingDepthText(){
    	    if(isBlinkingDepthText) return;
    	    isBlinkingDepthText = true;
    	    context.StartCoroutine(BlinkingDepth());
    	}

    	void StopBlinkingDepthText(){
    	    isBlinkingDepthText = false;
    	}

    	IEnumerator BlinkingDepth(){
    	    Color oldColor = context.depthText.normal.textColor;
    	    while(isBlinkingDepthText){
    	        context.depthText.normal.textColor = Color.yellow;
    	        yield return new WaitForSeconds (0.5f);
    	        context.depthText.normal.textColor = oldColor;
    	        yield return new WaitForSeconds (0.5f);
    	    }
    	    context.depthText.normal.textColor = oldColor;
    	}    
    }

    class BoostState : TutorialState{
        OnPressedDelegate d;

        public BoostState(Tutorial context):base(context){}

        public override void OnGUI(){
            GUI.Box(new Rect(40, 119, 400, 22), "Good. Now press \"Boost\" button to boost your movement.");        
        }

        public override void Enter(){
            d = new OnPressedDelegate(this.OnPressedBoost);
            context.buttonBoost.AddPressedDelegate(d);
            context.buttonBoost.StartBlinking();
        }

        public override void Exit(){
            context.buttonBoost.RemovePressedDelegate(d);
            context.buttonBoost.StopBlinking();
        }

        public override void Do(){
        }

        void OnPressedBoost(bool down){
            if(!down) return;
            sm.MoveTo(new AimState(context));
        }
    }
    
    class AimState : TutorialState{
        OnPressedDelegate d;

        public AimState(Tutorial context):base(context){}

        public override void OnGUI(){
            GUI.Box(new Rect(80, 119, 320, 22), "You can aim your speargun by pressing this button.");        
        }

        public override void Enter(){
            d = new OnPressedDelegate(this.OnPressedAim);
            context.buttonAim.AddPressedDelegate(d);
            context.buttonAim.StartBlinking();
        }

        public override void Exit(){
            context.buttonAim.RemovePressedDelegate(d);
            context.buttonAim.StopBlinking();
        }

        public override void Do(){
        }

        void OnPressedAim(bool down){
            if(!down) return;
            sm.MoveTo(new FireState(context));
        }
    }
    
    class FireState : TutorialState{
        OnPressedDelegate d;

        public FireState(Tutorial context):base(context){}

        public override void OnGUI(){
            GUI.Box(new Rect(100, 119, 280, 22), "Fire speargun by pressing this button");        
        }

        public override void Enter(){
            d = new OnPressedDelegate(this.OnPressedFire);
            context.buttonFire.AddPressedDelegate(d);
            context.buttonFire.StartBlinking();
        }

        public override void Exit(){
            context.buttonFire.RemovePressedDelegate(d);
            context.buttonFire.StopBlinking();
        }

        public override void Do(){
        }

        void OnPressedFire(bool down){
            if(!down) return;
            sm.MoveTo(new RefuilingState(context));
        }
    }
	
    class RefuilingState : TutorialState{
        HighlightableControlButton buttonArrow;
        
        public RefuilingState(Tutorial context):base(context){}

        public override void OnGUI(){
            GUI.Box(new Rect(100, 113, 280, 34), "Your oxygen level is depleting over time.\nFloat to surface to refill it.");
            buttonArrow.Draw();
        }

        public override void Enter(){
            context.gameMaster.setAir(0.75f);
            buttonArrow =  new HighlightableControlButton(context, new Rect(115, 150, 64, 64), null, null, context.arrow2D);
            buttonArrow.StartBlinking();
        }

        public override void Exit(){
            buttonArrow.StopBlinking();
        }

        public override void Do(){
            if(context.gameMaster.isSurface){
                sm.MoveTo(new HealthState(context));
            }            
        }

    }
    
   class HealthState : TutorialState{
        GameObject player;
        GameObject arrow;
        bool isBlinkingHealthText = false;        

        public HealthState(Tutorial context):base(context){
            player = GameObject.FindWithTag("Player");
        }

        public override void OnGUI(){
            GUI.Box(new Rect(80, 113, 320, 34), "If your health run low,\njust find health icon near the bottom and eat it");        
        }

        public override void Enter(){ 
            context.gameMaster.setHealth(90f);
            StartBlinkingHealthText();
            arrow = (GameObject)Instantiate(context.arrow3D, Vector3.zero, Quaternion.identity);            
            arrow.transform.parent = player.transform;
            arrow.transform.localPosition = new Vector3(-0.15f, 0f, 0.3f);
        }

        public override void Exit(){
            StopBlinkingHealthText();
            Destroy(arrow);
        }

        public override void Do(){
            if(context.gameMaster.getHealth() > 91f ){
                    sm.MoveTo(new FinalState(context));                    
                }
        }

        // private
    	void StartBlinkingHealthText(){
    	    if(isBlinkingHealthText) return;
    	    isBlinkingHealthText = true;
    	    context.StartCoroutine(BlinkingHealth());
    	}

    	void StopBlinkingHealthText(){
    	    isBlinkingHealthText = false;
    	}

    	IEnumerator BlinkingHealth(){
    	    Color oldColor = context.healthText.normal.textColor;
    	    while(isBlinkingHealthText){
    	        context.healthText.normal.textColor = Color.yellow;
    	        yield return new WaitForSeconds (0.5f);
    	        context.healthText.normal.textColor = oldColor;
    	        yield return new WaitForSeconds (0.5f);
    	    }
    	    context.healthText.normal.textColor = oldColor;
    	}    
    }
    
    class FinalState : TutorialState{
        float initialTime;

        public FinalState(Tutorial context):base(context){}

        public override void OnGUI(){
            GUI.Box(new Rect(100, 113, 280, 34), "Tutorial is complete!\nGood fishing!");        
        }

        public override void Enter(){
            initialTime = Time.time;
        }
        
        public override void Do(){
            if(Time.time - initialTime > 5){
                sm.MoveTo(null);
            }
        }

    }
}

