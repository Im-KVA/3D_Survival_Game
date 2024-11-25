// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Raygeas/AZURE Vegetation"
{
	Properties
	{
		[SingleLineTexture][Header(Maps)][Space(7)]_Texture00("Texture", 2D) = "white" {}
		[SingleLineTexture]_SmoothnessTexture3("Smoothness", 2D) = "white" {}
		[SingleLineTexture]_SnowMask("Snow Mask", 2D) = "white" {}
		_Tiling("Tiling", Float) = 1
		_SnowTiling("Snow Tiling", Float) = 1
		[Header(Settings)][Space(5)]_Color1("Main Color", Color) = (1,1,1,0)
		_AlphaCutoff("Alpha Cutoff", Range( 0 , 1)) = 0.35
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		[Header(Second Color Settings)][Space(5)][Toggle(_COLOR2ENABLE_ON)] _Color2Enable("Enable", Float) = 0
		_Color2("Second Color", Color) = (0,0,0,0)
		[KeywordEnum(Vertex_Position_Based,UV_Based)] _Color2OverlayType("Overlay Method", Float) = 0
		_Color2Level("Level", Float) = 0
		_Color2Fade("Fade", Range( -1 , 1)) = 0.5
		[Header(Show Settings)][Space(5)][Toggle(_SNOW_ON)] _SNOW("Enable", Float) = 0
		[KeywordEnum(World_Normal_Based,UV_Based)] _SnowOverlayType("Overlay Method", Float) = 0
		_SnowAmount("Amount", Range( 0 , 1)) = 0.5
		_SnowFade("Fade", Range( 0 , 1)) = 0.3
		[Header(Wind Settings)][Space(5)][Toggle(_WIND_ON)] _WIND("Enable", Float) = 1
		_WindForce("Force", Range( 0 , 1)) = 0.3
		_WindWavesScale("Waves Scale", Range( 0 , 1)) = 0.25
		_WindSpeed("Speed", Range( 0 , 1)) = 0.5
		[Toggle(_FIXTHEBASEOFFOLIAGE_ON)] _Fixthebaseoffoliage("Anchor the foliage base", Float) = 0
		[Header(Lighting Settings)][Space(5)]_DirectLightOffset("Direct Light Offset", Range( 0 , 1)) = 0
		_DirectLightInt("Direct Light Int", Range( 1 , 10)) = 1
		_IndirectLightInt("Indirect Light Int", Range( 1 , 10)) = 1
		_TranslucencyInt("Translucency Int", Range( 0 , 100)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Grass"  "Queue" = "Geometry+0" }
		Cull Off
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _WIND_ON
		#pragma shader_feature_local _FIXTHEBASEOFFOLIAGE_ON
		#pragma shader_feature_local _SNOW_ON
		#pragma shader_feature_local _COLOR2ENABLE_ON
		#pragma shader_feature_local _COLOR2OVERLAYTYPE_VERTEX_POSITION_BASED _COLOR2OVERLAYTYPE_UV_BASED
		#pragma shader_feature_local _SNOWOVERLAYTYPE_WORLD_NORMAL_BASED _SNOWOVERLAYTYPE_UV_BASED
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
			float4 screenPosition;
			float3 worldNormal;
			INTERNAL_DATA
		};

		struct SurfaceOutputCustomLightingCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform float _WindSpeed;
		uniform float _WindWavesScale;
		uniform float _WindForce;
		uniform sampler2D _Texture00;
		uniform float _Tiling;
		uniform float _DirectLightOffset;
		uniform float4 _Color1;
		uniform float4 _Color2;
		uniform float _Color2Level;
		uniform float _Color2Fade;
		uniform float4 _Texture00_ST;
		uniform float _SnowAmount;
		uniform sampler2D _SnowMask;
		uniform float _SnowTiling;
		uniform float _SnowFade;
		uniform float _DirectLightInt;
		uniform float _IndirectLightInt;
		uniform sampler2D _SmoothnessTexture3;
		uniform float _Smoothness;
		uniform float _TranslucencyInt;
		uniform float _AlphaCutoff;


		float3 mod3D289( float3 x ) { return x - floor( x / 289.0 ) * 289.0; }

		float4 mod3D289( float4 x ) { return x - floor( x / 289.0 ) * 289.0; }

		float4 permute( float4 x ) { return mod3D289( ( x * 34.0 + 1.0 ) * x ); }

		float4 taylorInvSqrt( float4 r ) { return 1.79284291400159 - r * 0.85373472095314; }

		float snoise( float3 v )
		{
			const float2 C = float2( 1.0 / 6.0, 1.0 / 3.0 );
			float3 i = floor( v + dot( v, C.yyy ) );
			float3 x0 = v - i + dot( i, C.xxx );
			float3 g = step( x0.yzx, x0.xyz );
			float3 l = 1.0 - g;
			float3 i1 = min( g.xyz, l.zxy );
			float3 i2 = max( g.xyz, l.zxy );
			float3 x1 = x0 - i1 + C.xxx;
			float3 x2 = x0 - i2 + C.yyy;
			float3 x3 = x0 - 0.5;
			i = mod3D289( i);
			float4 p = permute( permute( permute( i.z + float4( 0.0, i1.z, i2.z, 1.0 ) ) + i.y + float4( 0.0, i1.y, i2.y, 1.0 ) ) + i.x + float4( 0.0, i1.x, i2.x, 1.0 ) );
			float4 j = p - 49.0 * floor( p / 49.0 );  // mod(p,7*7)
			float4 x_ = floor( j / 7.0 );
			float4 y_ = floor( j - 7.0 * x_ );  // mod(j,N)
			float4 x = ( x_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			float4 y = ( y_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			float4 h = 1.0 - abs( x ) - abs( y );
			float4 b0 = float4( x.xy, y.xy );
			float4 b1 = float4( x.zw, y.zw );
			float4 s0 = floor( b0 ) * 2.0 + 1.0;
			float4 s1 = floor( b1 ) * 2.0 + 1.0;
			float4 sh = -step( h, 0.0 );
			float4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;
			float4 a1 = b1.xzyw + s1.xzyw * sh.zzww;
			float3 g0 = float3( a0.xy, h.x );
			float3 g1 = float3( a0.zw, h.y );
			float3 g2 = float3( a1.xy, h.z );
			float3 g3 = float3( a1.zw, h.w );
			float4 norm = taylorInvSqrt( float4( dot( g0, g0 ), dot( g1, g1 ), dot( g2, g2 ), dot( g3, g3 ) ) );
			g0 *= norm.x;
			g1 *= norm.y;
			g2 *= norm.z;
			g3 *= norm.w;
			float4 m = max( 0.6 - float4( dot( x0, x0 ), dot( x1, x1 ), dot( x2, x2 ), dot( x3, x3 ) ), 0.0 );
			m = m* m;
			m = m* m;
			float4 px = float4( dot( x0, g0 ), dot( x1, g1 ), dot( x2, g2 ), dot( x3, g3 ) );
			return 42.0 * dot( m, px);
		}


		inline float Dither4x4Bayer( int x, int y )
		{
			const float dither[ 16 ] = {
				 1,  9,  3, 11,
				13,  5, 15,  7,
				 4, 12,  2, 10,
				16,  8, 14,  6 };
			int r = y * 4 + x;
			return dither[r] / 16; // same # of instructions as pre-dividing due to compiler magic
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float mulTime34 = _Time.y * ( _WindSpeed * 5 );
			float simplePerlin3D35 = snoise( ( ase_worldPos + mulTime34 )*_WindWavesScale );
			float temp_output_231_0 = ( simplePerlin3D35 * 0.01 );
			#ifdef _FIXTHEBASEOFFOLIAGE_ON
				float staticSwitch376 = ( temp_output_231_0 * pow( v.texcoord.xy.y , 2.0 ) );
			#else
				float staticSwitch376 = temp_output_231_0;
			#endif
			#ifdef _WIND_ON
				float staticSwitch341 = ( staticSwitch376 * ( _WindForce * 30 ) );
			#else
				float staticSwitch341 = 0.0;
			#endif
			float Wind191 = staticSwitch341;
			float3 temp_cast_0 = (Wind191).xxx;
			v.vertex.xyz += temp_cast_0;
			float4 ase_screenPos = ComputeScreenPos( UnityObjectToClipPos( v.vertex ) );
			o.screenPosition = ase_screenPos;
		}

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			#ifdef UNITY_PASS_FORWARDBASE
			float ase_lightAtten = data.atten;
			if( _LightColor0.a == 0)
			ase_lightAtten = 0;
			#else
			float3 ase_lightAttenRGB = gi.light.color / ( ( _LightColor0.rgb ) + 0.000001 );
			float ase_lightAtten = max( max( ase_lightAttenRGB.r, ase_lightAttenRGB.g ), ase_lightAttenRGB.b );
			#endif
			#if defined(HANDLE_SHADOWS_BLENDING_IN_GI)
			half bakedAtten = UnitySampleBakedOcclusion(data.lightmapUV.xy, data.worldPos);
			float zDist = dot(_WorldSpaceCameraPos - data.worldPos, UNITY_MATRIX_V[2].xyz);
			float fadeDist = UnityComputeShadowFadeDistance(data.worldPos, zDist);
			ase_lightAtten = UnityMixRealtimeAndBakedShadows(data.atten, bakedAtten, UnityComputeShadowFade(fadeDist));
			#endif
			float2 temp_cast_0 = (_Tiling).xx;
			float2 uv_TexCoord445 = i.uv_texcoord * temp_cast_0;
			float2 Tiling446 = uv_TexCoord445;
			float4 tex2DNode1 = tex2D( _Texture00, Tiling446 );
			float OpacityMask263 = tex2DNode1.a;
			float4 ase_screenPos = i.screenPosition;
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float2 clipScreen458 = ase_screenPosNorm.xy * _ScreenParams.xy;
			float dither458 = Dither4x4Bayer( fmod(clipScreen458.x, 4), fmod(clipScreen458.y, 4) );
			dither458 = step( dither458, ( unity_LODFade.x > 0.0 ? unity_LODFade.x : 1.0 ) );
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = Unity_SafeNormalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_normWorldNormal = normalize( ase_worldNormal );
			float dotResult413 = dot( ase_worldlightDir , ase_normWorldNormal );
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float4 temp_output_10_0 = ( _Color1 * tex2DNode1 );
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			#if defined(_COLOR2OVERLAYTYPE_VERTEX_POSITION_BASED)
				float staticSwitch360 = ase_vertex3Pos.y;
			#elif defined(_COLOR2OVERLAYTYPE_UV_BASED)
				float staticSwitch360 = i.uv_texcoord.y;
			#else
				float staticSwitch360 = ase_vertex3Pos.y;
			#endif
			float SecondColorMask335 = saturate( ( ( staticSwitch360 + _Color2Level ) * ( _Color2Fade * 2 ) ) );
			float4 lerpResult332 = lerp( temp_output_10_0 , ( _Color2 * tex2D( _Texture00, Tiling446 ) ) , SecondColorMask335);
			#ifdef _COLOR2ENABLE_ON
				float4 staticSwitch340 = lerpResult332;
			#else
				float4 staticSwitch340 = temp_output_10_0;
			#endif
			float4 color288 = IsGammaSpace() ? float4(0.8962264,0.8962264,0.8962264,0) : float4(0.7799658,0.7799658,0.7799658,0);
			float2 uv_Texture00 = i.uv_texcoord * _Texture00_ST.xy + _Texture00_ST.zw;
			#if defined(_SNOWOVERLAYTYPE_WORLD_NORMAL_BASED)
				float staticSwitch390 = ase_worldNormal.y;
			#elif defined(_SNOWOVERLAYTYPE_UV_BASED)
				float staticSwitch390 = i.uv_texcoord.y;
			#else
				float staticSwitch390 = ase_worldNormal.y;
			#endif
			float2 temp_cast_1 = (_SnowTiling).xx;
			float2 uv_TexCoord464 = i.uv_texcoord * temp_cast_1;
			float saferPower354 = max( ( ( staticSwitch390 * ( _SnowAmount * 5 ) ) - tex2D( _SnowMask, uv_TexCoord464 ).r ) , 0.0001 );
			float SnowMask314 = saturate( pow( saferPower354 , ( _SnowFade * 20 ) ) );
			float4 lerpResult295 = lerp( staticSwitch340 , ( color288 * tex2D( _Texture00, uv_Texture00 ) ) , SnowMask314);
			#ifdef _SNOW_ON
				float4 staticSwitch342 = lerpResult295;
			#else
				float4 staticSwitch342 = staticSwitch340;
			#endif
			float4 Albedo259 = staticSwitch342;
			float4 DirectLight440 = ( ( saturate( (dotResult413*1.0 + _DirectLightOffset) ) * ase_lightAtten ) * ase_lightColor * Albedo259 * _DirectLightInt );
			UnityGI gi462 = gi;
			float3 diffNorm462 = ase_worldNormal;
			gi462 = UnityGI_Base( data, 1, diffNorm462 );
			float3 indirectDiffuse462 = gi462.indirect.diffuse + diffNorm462 * 0.0001;
			float4 IndirectLight439 = ( float4( indirectDiffuse462 , 0.0 ) * Albedo259 * _IndirectLightInt );
			SurfaceOutputStandard s443 = (SurfaceOutputStandard ) 0;
			s443.Albedo = float3( 0,0,0 );
			s443.Normal = ase_worldNormal;
			s443.Emission = float3( 0,0,0 );
			s443.Metallic = 0.0;
			s443.Smoothness = ( tex2D( _SmoothnessTexture3, Tiling446 ) * _Smoothness ).r;
			s443.Occlusion = 1.0;

			data.light = gi.light;

			UnityGI gi443 = gi;
			#ifdef UNITY_PASS_FORWARDBASE
			Unity_GlossyEnvironmentData g443 = UnityGlossyEnvironmentSetup( s443.Smoothness, data.worldViewDir, s443.Normal, float3(0,0,0));
			gi443 = UnityGlobalIllumination( data, s443.Occlusion, s443.Normal, g443 );
			#endif

			float3 surfResult443 = LightingStandard ( s443, viewDir, gi443 ).rgb;
			surfResult443 += s443.Emission;

			#ifdef UNITY_PASS_FORWARDADD//443
			surfResult443 -= s443.Emission;
			#endif//443
			float3 Smoothness441 = saturate( surfResult443 );
			float3 ase_worldViewDir = Unity_SafeNormalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float dotResult401 = dot( ase_worldlightDir , ase_worldViewDir );
			float TranslucencyMask417 = (-dotResult401*1.0 + -0.2);
			float dotResult399 = dot( ase_worldlightDir , ase_normWorldNormal );
			float4 Translucency442 = saturate( ( ( TranslucencyMask417 * ( ( ( (dotResult399*1.0 + 1.0) * ase_lightAtten ) * ase_lightColor * Albedo259 ) * 0.25 ) ) * _TranslucencyInt ) );
			c.rgb = ( DirectLight440 + IndirectLight439 + float4( Smoothness441 , 0.0 ) + Translucency442 ).rgb;
			c.a = 1;
			clip( ( OpacityMask263 * dither458 ) - _AlphaCutoff );
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			o.Normal = float3(0,0,1);
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting keepalpha fullforwardshadows nolightmap  nodynlightmap nodirlightmap nometa noforwardadd vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 customPack2 : TEXCOORD2;
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.customPack2.xyzw = customInputData.screenPosition;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				surfIN.screenPosition = IN.customPack2.xyzw;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputCustomLightingCustom, o )
				surf( surfIN, o );
				UnityGI gi;
				UNITY_INITIALIZE_OUTPUT( UnityGI, gi );
				o.Alpha = LightingStandardCustomLighting( o, worldViewDir, gi ).a;
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=18100
128;66;1918;929;5969.322;480.4935;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;447;-2787.07,436.0726;Inherit;False;750;278.1;;3;446;444;445;Tiling;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;336;-3546.929,-476.2514;Inherit;False;1474.883;565.8699;;10;310;335;334;382;248;377;360;309;361;391;Second Color Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;361;-3500.829,-247.2093;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PosVertexDataNode;309;-3465.548,-405.9526;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;313;-5466.776,-482.5264;Inherit;False;1858.103;826.4135;;15;305;314;354;349;350;291;362;299;363;322;390;312;352;464;465;Snow Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;444;-2721.233,552.7335;Inherit;False;Property;_Tiling;Tiling;3;0;Create;True;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;445;-2533.233,533.7333;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldNormalVector;312;-5398.226,-429.1961;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;248;-3156.202,-40.47443;Inherit;False;Property;_Color2Fade;Fade;12;0;Create;False;0;0;False;0;False;0.5;0.8;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;465;-5281.399,107.4906;Inherit;False;Property;_SnowTiling;Snow Tiling;4;0;Create;True;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;360;-3241.447,-302.6742;Inherit;False;Property;_Color2OverlayType;Overlay Method;10;0;Create;False;0;0;False;0;False;0;0;0;True;;KeywordEnum;2;Vertex_Position_Based;UV_Based;Create;True;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;310;-3049.677,-120.8673;Inherit;False;Property;_Color2Level;Level;11;0;Create;False;0;0;False;0;False;0;-0.24;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;352;-5429.32,-234.8225;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;291;-5263.6,-91.42583;Inherit;False;Property;_SnowAmount;Amount;15;0;Create;False;0;0;False;0;False;0.5;0.358;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;377;-2852.993,-219.3139;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;464;-5093.399,88.49036;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;390;-5130.754,-301.3329;Inherit;False;Property;_SnowOverlayType;Overlay Method;14;0;Create;False;0;0;False;0;False;0;0;0;True;;KeywordEnum;2;World_Normal_Based;UV_Based;Create;True;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleNode;391;-2855.358,-37.0037;Inherit;False;2;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;446;-2258.231,529.7333;Inherit;False;Tiling;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;262;-5465.631,-1647.714;Inherit;False;2646.528;1067.095;;23;448;450;449;259;263;342;295;340;315;370;369;332;288;367;374;10;337;3;247;1;366;368;156;Albedo;1,1,1,1;0;0
Node;AmplifyShaderEditor.ScaleNode;322;-4946.734,-84.80173;Inherit;False;5;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;363;-4813.988,33.03799;Inherit;True;Property;_SnowMask;Snow Mask;2;1;[SingleLineTexture];Create;True;0;0;False;0;False;-1;None;16d574e53541bba44a84052fa38778df;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;382;-2654.732,-130.5992;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;349;-4722.126,230.9095;Inherit;False;Property;_SnowFade;Fade;16;0;Create;False;0;0;False;0;False;0.3;0.252;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;448;-5412.208,-1091.293;Inherit;False;446;Tiling;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;299;-4731.806,-198.0521;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;334;-2471.036,-131.0528;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;362;-4401.161,-84.57065;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;450;-5200.624,-958.4913;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WireNode;449;-5202.812,-1174.035;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;368;-5183.645,-1144.078;Inherit;True;Property;_Texture00;Texture;0;1;[SingleLineTexture];Create;False;0;0;False;2;Header(Maps);Space(7);False;None;603a681375cacca45ae65a756606e807;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.ScaleNode;350;-4408.739,236.1816;Inherit;False;20;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;366;-4861.479,-960.2595;Inherit;True;Property;_TextureSample0;Texture Sample 0;18;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-4862.312,-1351.035;Inherit;True;Property;_LeavesTexture;Leaves Texture;0;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;247;-4777.857,-1146.771;Inherit;False;Property;_Color2;Second Color;9;0;Create;False;0;0;False;0;False;0,0,0,0;0.1597293,0.4433961,0.04433948,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;354;-4189.933,66.05038;Inherit;False;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;335;-2281.603,-136.4185;Inherit;False;SecondColorMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;3;-4780.577,-1540.59;Inherit;False;Property;_Color1;Main Color;5;0;Create;False;0;0;False;2;Header(Settings);Space(5);False;1,1,1,0;0.1386746,0.461,0.2773493,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;337;-4495.567,-1227.672;Inherit;False;335;SecondColorMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;374;-4883.29,-774.5762;Inherit;False;1;0;SAMPLER2D;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SaturateNode;305;-4012.858,65.51949;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-4457.864,-1451.376;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;367;-4461.048,-1057.932;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;369;-4263.512,-831.2733;Inherit;True;Property;_TextureSample1;Texture Sample 1;18;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;314;-3844.451,61.15887;Inherit;False;SnowMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;332;-4191.402,-1271.017;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;288;-4174.92,-1029.744;Inherit;False;Constant;_SnowColor;Snow Color;12;0;Create;True;0;0;False;0;False;0.8962264,0.8962264,0.8962264,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;392;-5462.447,1272.131;Inherit;False;2898.686;1981.707;;5;425;412;404;394;393;Lighting;0.7,0.686289,0.49,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;66;-5466.88,434.7253;Inherit;False;2621.259;742.2787;;18;191;341;188;56;345;190;36;376;359;356;231;35;358;357;182;228;34;344;Wind;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;370;-3860.318,-934.1633;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;340;-3859.422,-1455.011;Inherit;False;Property;_Color2Enable;Enable;8;0;Create;False;0;0;False;2;Header(Second Color Settings);Space(5);False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;315;-3788.891,-1227.871;Inherit;False;314;SnowMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;393;-5361.706,2675.193;Inherit;False;2228.953;513.9716;;17;442;435;434;427;424;421;418;415;411;408;406;402;400;399;398;397;463;Translucency;0.8,0.7843137,0.5607843,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;394;-5364.779,1393.619;Inherit;False;1214.467;434.7671;;7;417;407;405;403;401;396;395;Translucency Mask;0.8,0.7843137,0.5607843,1;0;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;395;-5252.485,1606.638;Inherit;False;World;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.LerpOp;295;-3568.515,-1269.782;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;398;-5316.385,2740.921;Inherit;False;True;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;36;-5432.24,691.3121;Inherit;False;Property;_WindSpeed;Speed;20;0;Create;False;0;0;False;0;False;0.5;0.2;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;396;-5314.778,1443.62;Inherit;False;True;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;397;-5286.607,2888.92;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;400;-5017.406,2950.468;Inherit;False;Constant;_Float1;Float 1;18;0;Create;True;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleNode;344;-5135.242,696.7037;Inherit;False;5;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;401;-5000.993,1524.467;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;399;-5013.608,2808.92;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;342;-3277.597,-1452.366;Inherit;False;Property;_SNOW;Enable;13;0;Create;False;0;0;False;2;Header(Show Settings);Space(5);False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WorldPosInputsNode;228;-4965.367,528.4799;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;403;-4886.512,1748.443;Inherit;False;Constant;_TranslucencyOffset;Translucency Offset;19;0;Create;True;0;0;False;0;False;-0.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;34;-4965.56,696.4692;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;405;-4787.39,1525.151;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightAttenuation;463;-4764.616,2965.657;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;259;-3046.768,-1456.52;Inherit;False;Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;402;-4765.339,2810.053;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;404;-5366.333,1896.834;Inherit;False;1679.855;716.6;;13;440;437;433;432;430;428;423;420;416;413;410;409;461;Direct Light;0.8,0.7843137,0.5607843,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;412;-4103.667,1396.346;Inherit;False;1479.169;401.6033;;7;441;436;426;422;419;414;443;Smoothness;0.8,0.7843137,0.5607843,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;190;-4870.377,811.9411;Inherit;False;Property;_WindWavesScale;Waves Scale;19;0;Create;False;0;0;False;0;False;0.25;0.4;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;410;-5325.434,1969.801;Inherit;False;True;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;182;-4719.293,608.7031;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;406;-4527.381,3097.51;Inherit;False;259;Albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.LightColorNode;411;-4509.65,2972.542;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.WorldNormalVector;409;-5293.338,2125.355;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ScaleAndOffsetNode;407;-4624.75,1613.723;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;408;-4490.469,2862.259;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;415;-4285.006,2949.23;Inherit;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DotProductOpNode;413;-5026.275,2040.562;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;416;-5095.354,2190.44;Inherit;False;Property;_DirectLightOffset;Direct Light Offset;22;0;Create;True;0;0;False;2;Header(Lighting Settings);Space(5);False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;358;-4405.196,1070.673;Inherit;False;Constant;_Float0;Float 0;14;0;Create;True;0;0;False;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;417;-4396.828,1608.337;Inherit;False;TranslucencyMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;414;-4064.032,1482.131;Inherit;False;446;Tiling;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;35;-4506.449,699.5806;Inherit;True;Simplex3D;False;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0.4;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;357;-4483.792,940.533;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;419;-3862.988,1457.242;Inherit;True;Property;_SmoothnessTexture3;Smoothness;1;1;[SingleLineTexture];Create;False;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;422;-3838.436,1687.676;Inherit;False;Property;_Smoothness;Smoothness;7;0;Create;True;0;0;False;0;False;0;0.07;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;420;-4778.006,2041.694;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleNode;418;-4113.641,2948.857;Inherit;False;0.25;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ScaleNode;231;-4188.372,703.8373;Inherit;False;0.01;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;356;-4198.264,960.5025;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;421;-4211.854,2836.586;Inherit;False;417;TranslucencyMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;425;-3630.025,1900.546;Inherit;False;933.2305;378.5339;;5;439;438;431;429;462;Indirect Light;0.8,0.7843137,0.5607843,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;424;-3916.838,2879.035;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;423;-4544.02,2041.592;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;426;-3491.987,1583.242;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;56;-3952.269,977.8912;Inherit;False;Property;_WindForce;Force;18;0;Create;False;0;0;False;0;False;0.3;0.48;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;359;-3989.254,821.5805;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;427;-4047.613,3062.894;Inherit;False;Property;_TranslucencyInt;Translucency Int;25;0;Create;True;0;0;False;0;False;0;8;0;100;0;1;FLOAT;0
Node;AmplifyShaderEditor.LightAttenuation;461;-4582.841,2241.318;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;429;-3590.024,2147.642;Inherit;False;Property;_IndirectLightInt;Indirect Light Int;24;0;Create;True;0;0;False;0;False;1;1;1;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.IndirectDiffuseLighting;462;-3535.912,1950.822;Inherit;False;Tangent;1;0;FLOAT3;0,0,1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ScaleNode;345;-3665.352,982.3984;Inherit;False;30;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;431;-3489.251,2044.545;Inherit;False;259;Albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;430;-4460.419,2523.262;Inherit;False;Property;_DirectLightInt;Direct Light Int;23;0;Create;True;0;0;False;0;False;1;1;1;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;428;-4357.409,2432.611;Inherit;False;259;Albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;434;-3718.531,2945.038;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;433;-4326.078,2163.435;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CustomStandardSurface;443;-3307.774,1488.447;Inherit;False;Metallic;Tangent;6;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,1;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LightColorNode;432;-4339.132,2313.63;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.StaticSwitch;376;-3795.695,700.9271;Inherit;False;Property;_Fixthebaseoffoliage;Anchor the foliage base;21;0;Create;False;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;437;-4096.474,2334.455;Inherit;False;4;4;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;436;-3029.443,1489.01;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;435;-3544.774,2945.197;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;188;-3463.908,823.2846;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;-1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LODFadeNode;456;-2652.505,-1295.872;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;438;-3221.504,2026.079;Inherit;True;3;3;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;441;-2832.614,1484.81;Inherit;False;Smoothness;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Compare;457;-2405.401,-1323.656;Inherit;False;2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;440;-3913.9,2329.491;Inherit;False;DirectLight;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;341;-3275.733,794.1859;Inherit;False;Property;_WIND;Enable;17;0;Create;False;0;0;False;2;Header(Wind Settings);Space(5);False;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;442;-3365.886,2940.199;Inherit;False;Translucency;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;439;-2939.794,2021.035;Inherit;False;IndirectLight;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;263;-4489.452,-1309.93;Inherit;False;OpacityMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;454;-2184.163,-821.8277;Inherit;False;442;Translucency;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;459;-2179.402,-1340.656;Inherit;False;263;OpacityMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;451;-2172.163,-1089.828;Inherit;False;440;DirectLight;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;191;-3060.489,793.7185;Inherit;False;Wind;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DitheringNode;458;-2171.401,-1227.656;Inherit;False;0;False;3;0;FLOAT;0;False;1;SAMPLER2D;;False;2;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;453;-2180.163,-910.8277;Inherit;False;441;Smoothness;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;452;-2180.163,-1002.828;Inherit;False;439;IndirectLight;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;156;-3949.634,-716.5307;Inherit;False;Property;_AlphaCutoff;Alpha Cutoff;6;0;Create;True;0;0;False;0;False;0.35;0.35;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;236;-1946.258,-745.3771;Inherit;False;191;Wind;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;455;-1914.162,-992.8277;Inherit;False;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT3;0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;460;-1939.64,-1298.29;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;151;-1717.934,-1233.323;Float;False;True;-1;2;;0;0;CustomLighting;Raygeas/AZURE Vegetation;False;False;False;False;False;False;True;True;True;False;True;True;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.45;True;True;0;True;Grass;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;5;False;-1;10;False;-1;0;5;False;-1;10;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;True;156;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;445;0;444;0
WireConnection;360;1;309;2
WireConnection;360;0;361;2
WireConnection;377;0;360;0
WireConnection;377;1;310;0
WireConnection;464;0;465;0
WireConnection;390;1;312;2
WireConnection;390;0;352;2
WireConnection;391;0;248;0
WireConnection;446;0;445;0
WireConnection;322;0;291;0
WireConnection;363;1;464;0
WireConnection;382;0;377;0
WireConnection;382;1;391;0
WireConnection;299;0;390;0
WireConnection;299;1;322;0
WireConnection;334;0;382;0
WireConnection;362;0;299;0
WireConnection;362;1;363;1
WireConnection;450;0;448;0
WireConnection;449;0;448;0
WireConnection;350;0;349;0
WireConnection;366;0;368;0
WireConnection;366;1;450;0
WireConnection;1;0;368;0
WireConnection;1;1;449;0
WireConnection;354;0;362;0
WireConnection;354;1;350;0
WireConnection;335;0;334;0
WireConnection;374;0;368;0
WireConnection;305;0;354;0
WireConnection;10;0;3;0
WireConnection;10;1;1;0
WireConnection;367;0;247;0
WireConnection;367;1;366;0
WireConnection;369;0;374;0
WireConnection;314;0;305;0
WireConnection;332;0;10;0
WireConnection;332;1;367;0
WireConnection;332;2;337;0
WireConnection;370;0;288;0
WireConnection;370;1;369;0
WireConnection;340;1;10;0
WireConnection;340;0;332;0
WireConnection;295;0;340;0
WireConnection;295;1;370;0
WireConnection;295;2;315;0
WireConnection;344;0;36;0
WireConnection;401;0;396;0
WireConnection;401;1;395;0
WireConnection;399;0;398;0
WireConnection;399;1;397;0
WireConnection;342;1;340;0
WireConnection;342;0;295;0
WireConnection;34;0;344;0
WireConnection;405;0;401;0
WireConnection;259;0;342;0
WireConnection;402;0;399;0
WireConnection;402;2;400;0
WireConnection;182;0;228;0
WireConnection;182;1;34;0
WireConnection;407;0;405;0
WireConnection;407;2;403;0
WireConnection;408;0;402;0
WireConnection;408;1;463;0
WireConnection;415;0;408;0
WireConnection;415;1;411;0
WireConnection;415;2;406;0
WireConnection;413;0;410;0
WireConnection;413;1;409;0
WireConnection;417;0;407;0
WireConnection;35;0;182;0
WireConnection;35;1;190;0
WireConnection;419;1;414;0
WireConnection;420;0;413;0
WireConnection;420;2;416;0
WireConnection;418;0;415;0
WireConnection;231;0;35;0
WireConnection;356;0;357;2
WireConnection;356;1;358;0
WireConnection;424;0;421;0
WireConnection;424;1;418;0
WireConnection;423;0;420;0
WireConnection;426;0;419;0
WireConnection;426;1;422;0
WireConnection;359;0;231;0
WireConnection;359;1;356;0
WireConnection;345;0;56;0
WireConnection;434;0;424;0
WireConnection;434;1;427;0
WireConnection;433;0;423;0
WireConnection;433;1;461;0
WireConnection;443;4;426;0
WireConnection;376;1;231;0
WireConnection;376;0;359;0
WireConnection;437;0;433;0
WireConnection;437;1;432;0
WireConnection;437;2;428;0
WireConnection;437;3;430;0
WireConnection;436;0;443;0
WireConnection;435;0;434;0
WireConnection;188;0;376;0
WireConnection;188;1;345;0
WireConnection;438;0;462;0
WireConnection;438;1;431;0
WireConnection;438;2;429;0
WireConnection;441;0;436;0
WireConnection;457;0;456;1
WireConnection;457;2;456;1
WireConnection;440;0;437;0
WireConnection;341;0;188;0
WireConnection;442;0;435;0
WireConnection;439;0;438;0
WireConnection;263;0;1;4
WireConnection;191;0;341;0
WireConnection;458;0;457;0
WireConnection;455;0;451;0
WireConnection;455;1;452;0
WireConnection;455;2;453;0
WireConnection;455;3;454;0
WireConnection;460;0;459;0
WireConnection;460;1;458;0
WireConnection;151;10;460;0
WireConnection;151;13;455;0
WireConnection;151;11;236;0
ASEEND*/
//CHKSM=4F2A05B3DD545E9113EE74809DE99543752A1774