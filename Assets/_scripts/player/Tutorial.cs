using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour {    
    public GUITexture arrowGUI;
    public GUITexture crosshairGUI;
    
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
	    get{return gameMaster.depth;}
	}
	

	// Use this for initialization
	void Start () {
	    useGUILayout = false;
	    gameMaster = (GameMaster)GetComponent(typeof(GameMaster));
	    hud = (HUD)GetComponent(typeof(HUD));
	    
	    sm = new StateMachine();
	    Invoke("BeginTutorial", 0.5f);	    
	}
	
	void BeginTutorial(){
	    sm.MoveTo(new WelcomeState(this));
	}
	
	void FixedUpdate(){
        sm.Do();        
    }
    	
	void OnGUI() {
	    if(hud.isGame)  sm.OnGUI();
    }
    
    
    //     //     //     //     // 
    
    // Property setters
	
	public void setAimButtonControl(HighlightableControlButton arg) {buttonAim = arg;}
	public void setFireButtonControl(HighlightableControlButton arg) {buttonFire = arg;}
	public void setBoostButtonControl(HighlightableControlButton arg) {buttonBoost = arg;}
	
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
        
        protected void EndTutorial(){
            sm.MoveTo(null);
            context.enabled = false;            
        }
        
        protected GameObject Create3DArrow(){
            GameObject player = GameObject.FindWithTag("Player");
            GameObject arrow = (GameObject)Instantiate(context.arrow3D, Vector3.zero, Quaternion.identity);            
            arrow.transform.parent = player.transform;
            arrow.transform.localPosition = new Vector3(-0.19f, 0f, 0.3f);
            return arrow;
        }
        
        protected HighlightableControlButton CreateBlinking2DArrow(){
            Texture2D emptyTexture = CreateEmptyTexture();            
            HighlightableControlButton arrow =  new HighlightableControlButton(context, context.arrowGUI, null, emptyTexture, context.arrow2D);
            arrow.StartBlinking();
            return arrow;
        }
        
        protected void Set2DArrowPos(Vector2 point){
            Vector3 position = context.arrowGUI.transform.position;
            position.x = point.x/Screen.width;
            position.y = 1.0f - point.y/Screen.height;
            context.arrowGUI.transform.position = position;
        }
        protected void Point2DArrowToCrosshair(){
            Vector3 pos = context.arrowGUI.transform.position;
            pos.x = context.crosshairGUI.transform.position.x;
            pos.y = context.crosshairGUI.transform.position.y;
            context.arrowGUI.transform.position = pos;
        }
        
        private Texture2D CreateEmptyTexture(){
            Texture2D empty = new Texture2D(64,64);
            empty.SetPixels(new Color[64*64], 0);
            empty.Apply();
            return empty;            
        }     
    }
    
    class ChainedState : TutorialState{
        protected ChainedState back, next;
        
        Rect rectNext, rectBack, rectSkip;
        
        public ChainedState(Tutorial _context, ChainedState _back, ChainedState _next)
        :base(_context)
        {
            back = _back;
            next = _next;
            
            const float margin = 80;
            const float width = 60;
            float top = context.gameMaster.isFreeVersion ? 80 : 30;
            rectBack = new Rect(margin, top, width, 30);
            rectNext = new Rect(Screen.width - (width + margin), top, width, 30);            
            rectSkip = new Rect((Screen.width -  width)/2, top, width, 30);            
        }
        public ChainedState(Tutorial _context)
        :this(_context, null, null){}
        
        public override void OnGUI(){
             DrawControlButtons();
        }
        
        protected void DrawBack(){
         if(GUI.Button(rectBack, "<< Back")) MoveBack();
        }
        protected void DrawNext(){
            if(GUI.Button(rectNext, "Next >>")) MoveNext();
        }
        protected void DrawSkip(){
            if(GUI.Button(rectSkip, "Skip")) EndTutorial();
        }
        
        protected virtual void DrawControlButtons(){
            if(back != null) DrawBack();
            if(next != null) DrawNext();
            DrawSkip();
        }     
        
        protected virtual void MoveNext(){
            if(next != null) sm.MoveTo(next);
        }
        protected virtual void MoveBack(){
            if(back != null) sm.MoveTo(back);
        }
        
        protected void PraiseAndMoveNext(){
            if(next != null){
                sm.MoveTo(new PraiseState(context, back, next));
            }
        }        
    }
    
    class PraiseState : ChainedState{
        static Rect rectGood;
        const float timeToShowMessage = 3.0f;
        float startTime;
        static PraiseState(){
            const float width = 50;
            rectGood = new Rect((Screen.width -  width)/2 + 5, 120, width, 20);
        }
        
        public PraiseState(Tutorial _context, ChainedState _back, ChainedState _next)
        :base(_context, _back, _next){
            startTime = Time.time;
        }
        
        public override void OnGUI(){
            base.OnGUI();
            GUI.Box(rectGood, "Good!");
        }
        
        public override void Do(){
            if(Time.time - startTime > timeToShowMessage) MoveNext();
        }
    }    
    
    class WelcomeState : ChainedState{
        float initialTime;
        
        public WelcomeState(Tutorial context)
        :base(context)
        {
            next = new SwimState(context, null);
            initialTime = Time.time;
        }
        
        public override void OnGUI(){
            base.OnGUI();
            GUI.Box(new Rect(135, 120, 210, 22), "Welcome to SpearFishing tutorial !");        
        }

        public override void Do(){
            if(Time.time - initialTime > 4.0f)  MoveNext();
        } 
        
        protected override void DrawControlButtons(){}      
    }

    class SwimState : ChainedState{
        const float DEPTH_TRESHOLD = 3.0f;
        
        bool isUp = true;
        PointUpOrDown arrow;

        float initialDepth, minDepth, maxDepth;
        bool isBlinkingDepthText = false;        

        public SwimState(Tutorial context, ChainedState _back)
        :base(context){
            back = _back;
            next = new BoostState(context, this);
        }

        public override void OnGUI(){
            base.OnGUI();
            GUI.Box(new Rect(100, 119, 280, 22), "Swim up / down / sideways by tilting your device.");        
        }

        public override void Enter(){
            initialDepth = minDepth = maxDepth = context.depth;
            StartBlinkingDepthText();
            GameObject obj = Create3DArrow();
            arrow = (PointUpOrDown)obj.AddComponent(typeof(PointUpOrDown));
        }

        public override void Exit(){
            Destroy(arrow.gameObject);
            StopBlinkingDepthText();
        }

        public override void Do(){
            minDepth = Mathf.Min(minDepth, context.depth);
            maxDepth = Mathf.Max(maxDepth, context.depth);

            if(isUp && initialDepth - minDepth > DEPTH_TRESHOLD){
                isUp = false;
                initialDepth = minDepth = maxDepth = context.depth;
                arrow.PointDown();
            }
            if(!isUp && maxDepth - initialDepth > DEPTH_TRESHOLD){
                PraiseAndMoveNext();            
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

    class BoostState : ChainedState{
        OnPressedDelegate d;

        public BoostState(Tutorial context, ChainedState _back)
        :base(context){
            back = _back;
            next = new FireState(context, this);
        }

        public override void OnGUI(){
            base.OnGUI();
            GUI.Box(new Rect(120, 119, 240, 22), "Swim faster by pressing the \"Fins\" button");        
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
            if(down) PraiseAndMoveNext();
        }
    }
    
    class AimState : ChainedState{
        OnPressedDelegate d;
        HighlightableControlButton arrow;

        public AimState(Tutorial context, ChainedState _back)
        :base(context){
            back = _back;
            next = new RefuilingState(context, this);
        }

        public override void OnGUI(){
            base.OnGUI();
            GUI.Box(new Rect(60, 115, 360, 36), "Aim your speargun using the \"Joystick\" button.\nThe black cross in the middle shows the direction of the launch");        
        }

        public override void Enter(){
            d = new OnPressedDelegate(this.OnPressedAim);
            context.buttonAim.AddPressedDelegate(d);
            context.buttonAim.StartBlinking();
            arrow = CreateBlinking2DArrow();            
        }

        public override void Exit(){
            context.buttonAim.RemovePressedDelegate(d);
            context.buttonAim.StopBlinking();
            arrow.StopBlinking();
        }

        public override void Do(){
            Point2DArrowToCrosshair();
        }

        void OnPressedAim(bool down){
            if(down) PraiseAndMoveNext();
        }
    }
    
    class FireState : ChainedState{
        OnPressedDelegate d;

        public FireState(Tutorial context, ChainedState _back)
        :base(context){
            back = _back;
            next = new HealthState(context, this);
        }

        public override void OnGUI(){
            base.OnGUI();
            GUI.Box(new Rect(90, 119, 300, 22), "Launch a spear by pressing the \"Speargun\" button.");        
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
            if(down) PraiseAndMoveNext();
        }
    }
	
    class RefuilingState : ChainedState{
        HighlightableControlButton buttonArrow;
        Texture2D emptyTexture;
        GameObject arrow;
        
        public RefuilingState(Tutorial context, ChainedState _back)
        :base(context){
            back = _back;
            next = new FinalState(context);
        }

        public override void OnGUI(){
            base.OnGUI();
            GUI.Box(new Rect(110, 113, 260, 34), "Your oxygen supply depletes while diving.\nSwim to sea surface to refill the air tank.\nFollow the yellow arrow");
        }

        public override void Enter(){
            arrow = Create3DArrow();
            arrow.transform.localPosition = new Vector3(0.18f, 0.08f, 0.3f);
            arrow.AddComponent(typeof(PointUpOrDown));
            
            context.gameMaster.LockOxygenLow();
            buttonArrow = CreateBlinking2DArrow();
            Set2DArrowPos(new Vector2(155, 262));
        }

        public override void Exit(){
            Destroy(arrow);
            context.gameMaster.UnlockOxygen();
            buttonArrow.StopBlinking();
        }

        public override void Do(){
            if(context.gameMaster.isSurface){
                PraiseAndMoveNext();
            }            
        }
        
        protected override void DrawControlButtons(){
            DrawBack();
            DrawSkip();
        }
    }
    
   class HealthState : ChainedState{
        GameObject arrow;
        bool isBlinkingHealthText = false;        

        public HealthState(Tutorial context, ChainedState _back)
        :base(context){
            back = _back;
            next = new AimState(context, this);
        }

        public override void OnGUI(){
            base.OnGUI();
            GUI.Box(new Rect(95, 113, 290, 50), "When your health levels runs low, you can search\n the sea floor for \"First Aid\" icons, and use them.\nFollow the yellow arrow to find one.");        
        }

        public override void Enter(){ 
            context.gameMaster.setHealth(90f);
            StartBlinkingHealthText();
            arrow = Create3DArrow();
            arrow.AddComponent(typeof(PointToHealth));
        }

        public override void Exit(){
            StopBlinkingHealthText();
            Destroy(arrow);
        }

        public override void Do(){
            if(context.gameMaster.getHealth() > 91f ){
                    MoveNext();
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
    
    class FinalState : ChainedState{
        float initialTime;

        public FinalState(Tutorial context)
        :base(context){}

        public override void OnGUI(){
            base.OnGUI();
            GUI.Box(new Rect(100, 113, 280, 34), "You have completed the tutorial successfully !\nGood Luck!");        
        }

        public override void Enter(){
            initialTime = Time.time;
        }
        
        public override void Do(){
            if(Time.time - initialTime > 5){
                EndTutorial();
            }
        }
        
        protected override void DrawControlButtons(){}

    }
}

