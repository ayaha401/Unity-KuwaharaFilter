using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

class KuwaharaFilterPass : ScriptableRenderPass
{
    private const string ProfilerTag = nameof(KuwaharaFilterPass);

    private readonly Material material;
    private float size;

    private RTHandle cameraColorTarget;
    private RTHandle bufferTarget;

    public KuwaharaFilterPass(Shader shader, float size)
    {
        material = CoreUtils.CreateEngineMaterial(shader);
        renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        this.size = size;
    }

    public void SetRenderTarget(RTHandle target)
    {
        cameraColorTarget = target;
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        RenderTextureDescriptor cameraTextureDescriptor = renderingData.cameraData.cameraTargetDescriptor;
        cameraTextureDescriptor.depthBufferBits = 0;
        RenderingUtils.ReAllocateIfNeeded(ref bufferTarget, cameraTextureDescriptor);
        ConfigureTarget(bufferTarget);
        ConfigureClear(ClearFlag.Color, new Color(0, 0, 0, 0));
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.isSceneViewCamera)
        {
            return;
        }

        var cmd = CommandBufferPool.Get(ProfilerTag);

        material.SetFloat("_Size", size);

        Blitter.BlitCameraTexture(cmd, cameraColorTarget, bufferTarget, material, 0);
        Blitter.BlitCameraTexture(cmd, bufferTarget, cameraColorTarget);
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
}
