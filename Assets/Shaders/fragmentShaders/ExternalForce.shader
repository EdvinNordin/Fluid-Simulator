Shader "FluidSim/ExternalForce" 
{
	SubShader 
	{
    	Pass 
    	{
			ZTest Always

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
				//distance between uv coordinate and mouse position
			    float d = distance(_Point, i.uv);
				float force = 0;

			    if(d < _Radius) 
			    {
			        float a = (_Radius - d) * 0.5;
					force = min(a, 1.0);
			    } 
				//set the source position of the force
				float source = tex2D(_Source, i.uv).x;
				//get positive linear interpolation with force as value
			  	return max(0, lerp(source, _Fill, force)).x;
			}
			
			ENDCG

    	}
	}
}