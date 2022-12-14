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
			    float dy = distance(_Point.y, i.uv.y);
				float dx = distance(_Point.x, i.uv.x);
				
				float force = (i.uv.y - dy) + (i.uv.x - dx);
			    
				//set the source position of the force
				float source = tex2D(_Source, i.uv);
				//get positive linear interpolation with force as value
			  	return max(0, lerp(source, _Fill, force));
			}
			
			ENDCG

    	}
	}
}