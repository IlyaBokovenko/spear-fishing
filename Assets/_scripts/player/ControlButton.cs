using UnityEngine;
using System.Collections;

public delegate void OnPressedDelegate(bool down);

public class ControlButton {
    // private bool isPlayer;
    
    protected GUITexture gui;
	protected Texture textureOn;
	protected Texture textureOff;
	private bool _isDown = false;
	public bool isDown
	{
	    get{return _isDown;}
	}
	
	private Vector2 _touchPos;
	public Vector2 touchPos
	{
	    get{return _touchPos;}
	}

    private OnPressedDelegate pressedDelegate;
	
	public ControlButton(GUITexture _gui, Texture _textureOn, Texture _textureOff) {
        // isPlayer = Application.platform == RuntimePlatform.OSXPlayer;        
	    gui = _gui;
		textureOn = _textureOn;
		textureOff = _textureOff;
	}
	
    public void AddPressedDelegate(OnPressedDelegate d){
        if(pressedDelegate == null)
            pressedDelegate = d;
            else
            pressedDelegate += d;
    }
    
    public void RemovePressedDelegate(OnPressedDelegate d){
        if(pressedDelegate != null) pressedDelegate -= d;
    }
	
	public void setDown(bool arg) {    
	    if(arg == _isDown)  return;	 
		_isDown = arg;
		gui.texture = _isDown ? textureOn : textureOff;
        if(pressedDelegate != null) pressedDelegate(arg);            
	}
	
	public void UpdateState(){	    
         // if(isPlayer){
         //     ProcessTouches();
         //          }else{
         //              ProcessClicks();
         //          }
         
         ProcessTouches();
	}
	
	public Vector2 TouchOffset(){
	    return new Vector2( touchPos.x - (gui.GetScreenRect().x + gui.GetScreenRect().width/2),
	                        touchPos.y - (gui.GetScreenRect().y + gui.GetScreenRect().height/2));
	}
	
	private void ProcessTouches(){                	    
	    foreach(iPhoneTouch touch in iPhoneInput.touches){
	        bool inside = gui.HitTest(touch.position);
	        if(touch.phase != iPhoneTouchPhase.Ended && inside){
	            _touchPos = touch.position;
	            setDown(true);
	            return;
	        }   
	    }
	    setDown(false);
	}
	
    // private void ProcessClicks(){       
    //     if( Input.GetButton("Fire1") &&
    //             gui.HitTest(Input.mousePosition)){
    //                 _touchPos = Input.mousePosition;
    //                 setDown(true);
    //         }else{
    //             setDown(false);
    //         }
    // }

}
