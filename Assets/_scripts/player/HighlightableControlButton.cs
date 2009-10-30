using UnityEngine;
using System.Collections;

public class HighlightableControlButton : ControlButton {
    private MonoBehaviour context;
    private Texture textureHighlight;
    
    private bool _highlighted = false;
    private bool highlighted
    {
        get{return _highlighted;}
        set{
            _highlighted = value;
            gui.texture = _highlighted ? textureHighlight : textureOff;
        }
    }
    private bool isBlinking = false;
    
    public HighlightableControlButton(MonoBehaviour _context, GUITexture _gui, Texture _textureOn, Texture _textureOff, Texture _textureHighlight)
        : base(_gui, _textureOn, _textureOff){
            context = _context;
            textureHighlight = _textureHighlight;
        }

	public void StartBlinking(){
	    if(isBlinking) return;
	    
	    isBlinking = true;
	    context.StartCoroutine(Blinking());
	}
	
	public void StopBlinking(){	    
	    isBlinking = false;
	}
	
	IEnumerator Blinking(){	    
        while(isBlinking){	        
	        yield return new WaitForSeconds (0.5f);
	        highlighted = !highlighted;
	    }
	    highlighted = false;
	}
}
