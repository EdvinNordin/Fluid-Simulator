Shader "FluidSim/SubtractGradient" 
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
			uniform sampler2D _Pressure;
			uniform sampler2D _Obstacles;
			uniform float _GradientScale;
			uniform float2 _GridResolution;
			
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
			
			    // Find neighboring pressure:
			    float pN = tex2D(_Pressure, i.uv + float2(0, _GridResolution.y)).x;
			    float pS = tex2D(_Pressure, i.uv + float2(0, -_GridResolution.y)).x;
			    float pE = tex2D(_Pressure, i.uv + float2(_GridResolution.x, 0)).x;
			    float pW = tex2D(_Pressure, i.uv + float2(-_GridResolution.x, 0)).x;
			    float pC = tex2D(_Pressure, i.uv).x;
			
			    // Find neighboring obstacles:
			    float bN = tex2D(_Obstacles, i.uv + float2(0, _GridResolution.y)).x;
			    float bS = tex2D(_Obstacles, i.uv + float2(0, -_GridResolution.y)).x;
			    float bE = tex2D(_Obstacles, i.uv + float2(_GridResolution.x, 0)).x;
			    float bW = tex2D(_Obstacles, i.uv + float2(-_GridResolution.x, 0)).x;
			
			    // Use center pressure for solid cells:
			    if(bN > 0.0) pN = pC;
			    if(bS > 0.0) pS = pC;
			    if(bE > 0.0) pE = pC;
			    if(bW > 0.0) pW = pC;
			
			    // Enforce the free-slip boundary condition:
			    float2 oldV = tex2D(_Velocity, i.uv).xy;
			    float2 grad = float2(pE - pW, pN - pS) * _GradientScale;
			    float2 newV = oldV - grad;
			    
			    return float4(newV,0,1);  
			}
			
			ENDCG

    	}
	}
}