using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class LimitlessCustomTexture : ScriptableRendererFeature
{
    LimitlessCustomTexturePass GlitchPass;
    public RenderPassEvent Event = RenderPassEvent.BeforeRenderingPostProcessing;


    public override void Create()
    {
        GlitchPass = new LimitlessCustomTexturePass(Event);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
#if UNITY_2019 || UNITY_2020
        GlitchPass.Setup(renderer.cameraColorTarget);
#else

#endif
        renderer.EnqueuePass(GlitchPass);
    }
    public class LimitlessCustomTexturePass : ScriptableRenderPass
    {
        static readonly string k_RenderTag = "Render LimitlessCustomTexture Effect";
        static readonly int MainTexId = Shader.PropertyToID("_MainTex");

        static readonly int TempTargetId = Shader.PropertyToID("LimitlessCustomTexture");

        CustomTexV LimitlessCustomTexture;
        Material LimitlessCustomTextureMaterial;
        RenderTargetIdentifier currentTarget;


        public LimitlessCustomTexturePass(RenderPassEvent evt)
        {
            renderPassEvent = evt;
            var shader = Shader.Find("Limitless/CustomTextureShader");
            if (shader == null)
            {
                Debug.LogError("Shader not found.");
                return;
            }
            LimitlessCustomTextureMaterial = CoreUtils.CreateEngineMaterial(shader);

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
            if (LimitlessCustomTextureMaterial == null)
            {
                Debug.LogError("Material not created.");
                return;
            }
            if (LimitlessCustomTexture == null) {
            
            var stack = VolumeManager.instance.stack;
            LimitlessCustomTexture = stack.GetComponent<CustomTexV>();
            }

            if (LimitlessCustomTexture == null) { return; }
            if (!LimitlessCustomTexture.IsActive()) { return; }

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

            if (LimitlessCustomTexture.texture.value != null)
                LimitlessCustomTextureMaterial.SetTexture("_CustomTex", LimitlessCustomTexture.texture.value);

            cmd.SetGlobalTexture(MainTexId, source);
            LimitlessCustomTextureMaterial.SetFloat("fade", LimitlessCustomTexture.fade.value);
            


            cmd.GetTemporaryRT(destination, Screen.width, Screen.height, 0, FilterMode.Point, UnityEngine.Experimental.Rendering.GraphicsFormat.R16G16B16A16_UNorm);
            

            cmd.Blit(source, destination);
            cmd.Blit(destination, source, LimitlessCustomTextureMaterial, shaderPass);
        }

    }

}


