Shader "FluidSim/Convection" 
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
			
			uniform sampler2D _Velocity;
			uniform sampler2D _Temperature;
			uniform sampler2D _Density;
			uniform float _TimeStep;
		
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
			
			float4 frag(v2f i ): COLOR
			{
			    float T = tex2D(_Temperature, i.uv).x;
			    float2 V = tex2D(_Velocity, i.uv).xy;
			    float D = tex2D(_Density, i.uv).x;
			
			    float2 result = V;
			
			    if(T > 0.0) 
			    {
			        result += (_TimeStep * (T - 0.0) * 1.0 - D * 0.05 ) * float2(0, 1);
			    }
			    
			    return float4(result, 0, 1);
			    
			}
			
			ENDCG

    	}
	}
}