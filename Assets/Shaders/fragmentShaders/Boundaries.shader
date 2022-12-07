Shader "FluidSim/Boundaries" 
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
			
			uniform float2 _InverseSize;
		
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
				float4 result = float4(0,0,0,0);
				
				//draw border 
				if(i.uv.x <= _InverseSize.x) result = float4(1,1,1,1);
				if(i.uv.x >= 1.0-_InverseSize.x) result = float4(1,1,1,1);
				if(i.uv.y <= _InverseSize.y) result = float4(1,1,1,1);
				if(i.uv.y >= 1.0-_InverseSize.y) result = float4(1,1,1,1);
			
				return result;
			}
			
			ENDCG

    	}
	}
}