Shader "Zone4/Rainboooooaw"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		_Tiling ("Tiling", Float) = 1
		_TilingSpeed ("Tiling Speed", Float) = 1.0
		_RainbowTex ("RainbowTex", 2D) = "white" {}
		_Random ("Tiling", Float) = 1.0
		_Enabled ("Enabled", Float) = 0.0
		//_RotationSpeed ("Rotation Speed", Float) = 2.0
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile DUMMY PIXELSNAP_ON
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
			};
			
			fixed4 _Color;
						
			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _RainbowTex;
			float _Tiling;
			float _TilingSpeed;
			float _Enabled;
			//float _RotationSpeed;

			fixed4 frag(v2f IN) : SV_Target
			{
				//float sinXY = sin( 1.57 );
				//float cosXY = cos( 1.57 );
				//float2x2 rotationMatrix = float2x2( 1.0, -1.0, 0.0, 1.0 );
				//float2 rainbowTexcoord = mul( rotationMatrix, IN.texcoord.xy );
				
				fixed4 c = tex2D( _MainTex, IN.texcoord );	
				c.rgb *= IN.color.rgb;
				c.rgb = saturate( c.rgb - _Enabled );
				c.rgb += tex2D( _RainbowTex, IN.texcoord * _Tiling + _Time * _TilingSpeed ) * _Enabled;
				c.rgb *= c.a;
				return c;
			}
		ENDCG
		}
	}
}
