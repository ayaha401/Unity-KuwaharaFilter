using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class KuwaharaFilter : ScriptableRendererFeature
{
    [SerializeField]
    private Shader shader;

    [SerializeField, Range(0f, 25f)]
    private float size;

    private KuwaharaFilterPass kuwaharaFilterPass;

    public override void Create()
    {
        kuwaharaFilterPass = new KuwaharaFilterPass(shader, size);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(kuwaharaFilterPass);
    }

    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        kuwaharaFilterPass.SetRenderTarget(renderer.cameraColorTargetHandle);
    }
}
