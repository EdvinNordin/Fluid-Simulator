Shader "FluidSim/Advect" 
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
			uniform sampler2D _Source;
			uniform sampler2D _Obstacles;
			
			uniform float2 _InverseSize;
			uniform float _TimeStep;
			uniform float _Dissipation;
		
			struct v2f 
			{
    			float4  pos : SV_POSITION;
    			float2  uv : TEXCOORD0;
			};

			v2f vert(appdata_base v)
			{
    			v2f o;
    			o.pos = UnityObjectToClipPos(v.vertex); //Transforms a point from object space to the camera’s clip space in homogeneous coordinates.
    			o.uv = v.texcoord.xy;
    			return o;
			}
			
			float4 frag(v2f i) : COLOR
			{			
			    float2 u = tex2D(_Velocity, i.uv).xy;
			    float2 coord = i.uv - (u * _InverseSize * _TimeStep);			   
			    float4 result = _Dissipation * tex2D(_Source, coord);			    
			    float solid = tex2D(_Obstacles, i.uv).x;
			    
			    if(solid > 0.0) result = float4(0,0,0,0);
			    
			    return result;
			}
			
			ENDCG

    	}
	}
}
