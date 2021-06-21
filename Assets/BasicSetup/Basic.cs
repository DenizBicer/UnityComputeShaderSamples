using UnityEngine;

namespace BasicSetup
{
    /// <summary>
    /// Runs default Unity compute shader "Basic.compute"
    /// </summary>
    public class Basic : MonoBehaviour
    {
        [SerializeField]
        private ComputeShader shader = default;

        [SerializeField]
        private Vector2Int dimensions = default;

        [SerializeField] 
        private Material material = default;
        private void Start()
        {
            var renderTexture = new RenderTexture(dimensions.x, dimensions.y, 24){enableRandomWrite = true};
            renderTexture.Create();

            var kernel = shader.FindKernel("cs_main");
            shader.SetTexture(kernel, "result", renderTexture);
            shader.Dispatch(kernel, dimensions.x/ ThreadCount, dimensions.y/ThreadCount, 1);

            material.mainTexture = renderTexture;
        }
        private const int ThreadCount = 8;
    }
}
