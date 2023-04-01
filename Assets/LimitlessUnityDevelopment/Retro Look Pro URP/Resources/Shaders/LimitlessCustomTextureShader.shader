Shader "Limitless/CustomTextureShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_CustomTex("Texture", 2D) = "white" {}
	}
	HLSLINCLUDE

	 #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
     #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"
     #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
     #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"

		TEXTURE2D(_MainTex);
	SAMPLER(sampler_MainTex);
	TEXTURE2D(_CustomTex);
	SAMPLER(sampler_CustomTex);
	half fade;

	float4 Frag(Varyings i) : SV_Target
	{
		float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex,i.uv);
		float4 col2 = SAMPLE_TEXTURE2D(_CustomTex, sampler_CustomTex, i.uv);
		return lerp(col, col2, col2.a*fade);
	}

		ENDHLSL

		SubShader
	{
		Cull Off ZWrite Off ZTest Always

			Pass
		{
			HLSLPROGRAM

				#pragma vertex Vert
				#pragma fragment Frag

			ENDHLSL
		}
	}
}