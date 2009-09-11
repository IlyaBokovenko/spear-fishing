Shader "My Shaders/Self-Illumin/VertexLitZ" {
	Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("SelfIllum Color (RGB) Trans (A)", 2D) = "white" {}
    }
    SubShader {
        Tags {"RenderType"="Transparent" "Queue"="Transparent"}
		Pass {
	        ColorMask 0
    	}
		Pass {
            ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask RGB
			Material {
	        	Diffuse [_Color]
        	    Ambient [_Color]
				Emission [_Color]
	        }
			Lighting On
			SetTexture [_MainTex] {
            	Combine Texture * Primary, Texture * Primary
        	}
    	}
	}
}