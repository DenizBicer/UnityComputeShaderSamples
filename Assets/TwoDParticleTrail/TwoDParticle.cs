using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

namespace TwoDParticleTrail
{
    /// <summary>
    /// Renders a trail texture that is filled by the compute shader "TwoDParticle.compute"
    /// </summary>
    public class TwoDParticle : MonoBehaviour
    {    
        [SerializeField]
        private ComputeShader shader = default;

        [SerializeField]
        private Vector2Int dimensions = default;

        [SerializeField] 
        private RawImage image = default;

        [SerializeField] 
        private int numberOfParticles = 800;

        private void Start()
        {
            // common parameters
            shader.SetVector("trail_dimension", new Vector4(dimensions.x, dimensions.y));
            shader.SetInt("number_of_particles", numberOfParticles);
            
            // initialize particles
            var data = new Particle[numberOfParticles];
            
            _particleBuffer = new ComputeBuffer(data.Length, Marshal.SizeOf(typeof(Particle)));
            _particleBuffer.SetData(data);

            var initKernel = shader.FindKernel("initialize");

            shader.SetBuffer(initKernel, "particle_buffer", _particleBuffer); 
            shader.Dispatch(initKernel, numberOfParticles / ThreadCount, 1, 1);
            
            // prepare trail texture 
            var trailRenderTexture = new RenderTexture(dimensions.x, dimensions.y, 24){enableRandomWrite = true};
            trailRenderTexture.Create();

            _moveKernel = shader.FindKernel("move");
            shader.SetTexture(_moveKernel, "trail_texture", trailRenderTexture);
            shader.SetBuffer(_moveKernel, "particle_buffer", _particleBuffer);
            image.texture = trailRenderTexture;
        }

        private void Update()
        {
            shader.Dispatch(_moveKernel, numberOfParticles/ThreadCount, 1, 1);
        }

        private void OnDestroy()
        {
            _particleBuffer.Release();
        }

        private struct Particle
        {
            private Vector2 _point;
            private float _angle;
        };

        private ComputeBuffer _particleBuffer;
        private int _moveKernel;
        private const int ThreadCount = 8;
    }
}
