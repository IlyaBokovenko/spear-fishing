using UnityEngine;
using System.Collections;

public delegate void OnPressedDelegate(bool down);

public class ControlButton {
	private Texture2D textureOn;
	private Texture2D textureOff;
	public Rect rect;
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
	
	public ControlButton(Rect _rect, Texture2D _textureOn, Texture2D _textureOff) {
		textureOn = _textureOn;
		textureOff = _textureOff;
		rect = _rect;	
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
	
	virtual public void Draw() {
		if((_isDown && textureOn) || (!_isDown && textureOff))
			GUI.DrawTexture(rect, _isDown ? textureOn : textureOff);
	}
	
	public bool Contains(Vector2 point) {
		return rect.Contains(point);
	}
	
	public void setDown(bool arg) {    
	    if(arg == _isDown)  return;	    
		_isDown = arg;
        if(pressedDelegate != null) pressedDelegate(arg);            
	}
	
	public void ProcessTouches(){
	    foreach(iPhoneTouch touch in iPhoneInput.touches){
	        Vector2 touchCoords = new Vector2(touch.position.x, Screen.height - touch.position.y);
	        if(touch.phase != iPhoneTouchPhase.Ended && Contains(touchCoords)){
	            _touchPos = touchCoords;
	            setDown(true);
	            return;
	        }   
	    }
	    setDown(false);
	}

}
