using UnityEngine;
using System.Collections;

public class HighlightableControlButton : ControlButton {
    private MonoBehaviour context;
    private Texture2D textureHighlight;
    
    public bool highlighted = false;
    private bool isBlinking = false;
    
    public HighlightableControlButton(MonoBehaviour _context, Rect _rect, Texture2D _textureOn, Texture2D _textureOff, Texture2D _textureHighlight)
        : base(_rect, _textureOn, _textureOff){
            context = _context;
            textureHighlight = _textureHighlight;
        }

	override public void Draw(){
	    if(highlighted && textureHighlight){
	        GUI.DrawTexture(rect, textureHighlight);	        
	    }
	    else
	        base.Draw();
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
