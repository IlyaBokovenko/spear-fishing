using UnityEngine;
using System.Collections;

public class ControlButton {
	private Texture2D textureOn;
	private Texture2D textureOff;
	public Rect rect;
	private bool isDown = false;
	
	public ControlButton(Rect _rect, Texture2D _textureOn, Texture2D _textureOff) {
		textureOn = _textureOn;
		textureOff = _textureOff;
		rect = _rect;
	}
	
	public void Draw() {
		if((isDown && textureOn) || (!isDown && textureOff))
			GUI.DrawTexture(rect, isDown ? textureOn : textureOff);
	}
	
	public bool Contains(Vector2 point) {
		return isDown = rect.Contains(point) ? true : false;
	}
	
	public void setDown(bool arg) {
		isDown = arg;
	}
}
