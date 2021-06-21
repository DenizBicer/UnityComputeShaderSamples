Shader "Custom/Particle"
{
    Properties
    {
        point_size("Point size", Float) = 5.0
    }

    SubShader
    {
        Pass
        {
            Tags
            {
                "RenderType" = "Opaque"
            }
            LOD 200
            Blend SrcAlpha one

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag


            #include "UnityCG.cginc"

            #pragma target 4.5

            struct particle
            {
                float3 position;
            };

            struct ps_input
            {
                float4 position : SV_POSITION;
                float4 color : COLOR;
                float life : LIFE;
                float size : PSIZE;
            };

            StructuredBuffer<particle> particleBuffer;
            uniform float point_size;

            ps_input vert(uint vertex_id : SV_VertexID, const uint instance_id : SV_InstanceID)
            {
                ps_input o = (ps_input)0;

                o.color = fixed4(1.0, 1.0, 1.0, 1.0);
                o.position = UnityObjectToClipPos(float4(particleBuffer[instance_id].position, 1.0f));
                o.size = point_size;
                return o;
            }

            float4 frag(const ps_input i) : COLOR
            {
                return i.color;
            }
            ENDCG
        }
    }
    FallBack Off
}