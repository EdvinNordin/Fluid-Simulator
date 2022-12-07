Shader "FluidSim/WaterSurface" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
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
			
			sampler2D _MainTex;
			sampler2D _Obstacles;
			float3 _FluidColor, _ObstacleColor;
		
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
			 	float3 col = _FluidColor * tex2D(_MainTex, i.uv).x;
			 	
			 	float obs = tex2D(_Obstacles, i.uv).x;
			 	
			 	float3 result = lerp(col, _ObstacleColor, obs);
			 	
				return float4(result,1);
			}
			
			ENDCG

    	}
	}
}
