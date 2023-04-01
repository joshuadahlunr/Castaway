﻿using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CinematicBars_RLPRO : ScriptableRendererFeature
{
    CinematicBars_RLPROPass RetroPass;
    public RenderPassEvent Event = RenderPassEvent.BeforeRenderingPostProcessing;


    public override void Create()
    {
        RetroPass = new CinematicBars_RLPROPass(Event);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
#if UNITY_2019 || UNITY_2020
        RetroPass.Setup(renderer.cameraColorTarget);
#else

#endif
        renderer.EnqueuePass(RetroPass);
    }
    public class CinematicBars_RLPROPass : ScriptableRenderPass
    {
        static readonly string k_RenderTag = "Renderr Glitch1 Effect";
        static readonly int MainTexId = Shader.PropertyToID("_InputTexture");
        static readonly int _StripesV = Shader.PropertyToID("_Stripes");
        static readonly int _FadeV = Shader.PropertyToID("_Fade");
        static readonly int TempTargetId = Shader.PropertyToID("Glitch1rr");

        CinematicBars retroEffect;
        Material RetroEffectMaterial;
        RenderTargetIdentifier currentTarget;

        public CinematicBars_RLPROPass(RenderPassEvent evt)
        {
            renderPassEvent = evt;
            var shader = Shader.Find("Hidden/Shader/CinematicBarsEffect_RLPRO");
            if (shader == null)
            {
                Debug.LogError("Shader not found.");
                return;
            }
            RetroEffectMaterial = CoreUtils.CreateEngineMaterial(shader);

        }
#if UNITY_2019 || UNITY_2020

#elif UNITY_2021
		public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
		{
			var renderer = renderingData.cameraData.renderer;
			currentTarget = renderer.cameraColorTarget;
		}
#else
		public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
		{
			var renderer = renderingData.cameraData.renderer;
			currentTarget = renderer.cameraColorTargetHandle;
		}
#endif

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (RetroEffectMaterial == null)
            {
                Debug.LogError("Material not created.");
                return;
            }
            if (!renderingData.cameraData.postProcessEnabled) return;

            var stack = VolumeManager.instance.stack;
            retroEffect = stack.GetComponent<CinematicBars>();
            if (retroEffect == null) { return; }
            if (!retroEffect.IsActive()) { return; }

            var cmd = CommandBufferPool.Get(k_RenderTag);
            Render(cmd, ref renderingData);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public void Setup(in RenderTargetIdentifier currentTarget)
        {
            this.currentTarget = currentTarget;
        }

        void Render(CommandBuffer cmd, ref RenderingData renderingData)
        {
            ref var cameraData = ref renderingData.cameraData;
            var source = currentTarget;
            int destination = TempTargetId;

            int shaderPass = 0;
            RetroEffectMaterial.SetFloat(_StripesV, 0.51f - retroEffect.amount.value);
            RetroEffectMaterial.SetFloat(_FadeV, retroEffect.fade.value);

            cmd.SetGlobalTexture(MainTexId, source);

            cmd.GetTemporaryRT(destination, Screen.width, Screen.height, 0, FilterMode.Point, RenderTextureFormat.Default);


            cmd.Blit(source, destination);
            cmd.Blit(destination, source, RetroEffectMaterial, shaderPass);
        }
        private void ParamSwitch(Material mat, bool paramValue, string paramName)
        {
            if (paramValue) mat.EnableKeyword(paramName);
            else mat.DisableKeyword(paramName);
        }
    }

}


