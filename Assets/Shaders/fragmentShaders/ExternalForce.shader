Shader "FluidSim/ExternalForce" 
{
	SubShader 
	{
    	Pass 
    	{
			ZTest Always
			//Blend SrcAlpha OneMinusSrcAlpha 

			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			
			uniform float2 _Point;
			uniform float _Radius;
			uniform float _Fill;
			uniform sampler2D _Source;
			
			struct v2f 
			{
				float4  pos : SV_POSITION;
				float2  uv : TEXCOORD0;
			};
			
			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord.xy;
				return o;
			}
			
			float4 frag(v2f i) : COLOR
			{
			    float d = distance(_Point, i.uv);
			    
				float impulse = 0;
			    
			    if(d < _Radius) 
			    {
			        float a = (_Radius - d) * 0.5;
					impulse = min(a, 1.0);
			    } 

				float source = tex2D(_Source, i.uv).x;
			  
			  	return max(0, lerp(source, _Fill, impulse)).xxxx;
			}
			
			ENDCG

    	}
	}
}