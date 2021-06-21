using System.Runtime.InteropServices;
using UnityEngine;

namespace ProceduralPoints
{
    /// <summary>
    /// Renders point instances that are positioned by the compute shader "Particle.compute"
    /// </summary>
    public class ProceduralPointsController : MonoBehaviour
    {    
        [SerializeField]
        private ComputeShader shader = default;

        [SerializeField]
        private Renderer instanceRenderer = default;
        
        [SerializeField] 
        private int numberOfParticles = 800;

        private void Start()
        {
            // common parameters
            shader.SetInt("number_of_particles", numberOfParticles);
            
            // initialize particles
            var data = new Particle[numberOfParticles];
            
            _particleBuffer = new ComputeBuffer(data.Length, Marshal.SizeOf(typeof(Particle)));
            _particleBuffer.SetData(data);

            var initKernel = shader.FindKernel("initialize");

            shader.SetBuffer(initKernel, "particle_buffer", _particleBuffer); 
            shader.Dispatch(initKernel, numberOfParticles / ThreadCount, 1, 1);
            
            instanceRenderer.material.SetBuffer("particleBuffer", _particleBuffer);
        }

        private void OnRenderObject()
        {
            instanceRenderer.material.SetPass(0);
            Graphics.DrawProceduralNow(MeshTopology.Points, 1, numberOfParticles);
        }

        private void OnDestroy()
        {
            _particleBuffer.Release();
        }

        private struct Particle
        {
            private Vector3 _position;
        };

        private ComputeBuffer _particleBuffer;
        private const int ThreadCount = 8;
    }
}
