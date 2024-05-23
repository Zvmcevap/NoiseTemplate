Shader "Unlit/noiseSurfaceShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color1 ("Color 1", Color ) = (1, 1, 1, 1)
        _Limit1 ("Limit 1", float) = 0.2

        _Color2 ("Color 2", Color) = (1, 1, 1, 1)
        _Limit2 ("Limit 2", float) = 0.4

        _Color3 ("Color 3", Color ) = (1, 1, 1, 1)
        _Limit3 ("Limit 3", float) = 0.6

        _Color4 ("Color 4", Color) = (1, 1, 1, 1)
        _Limit4 ("Limit 4", float) = 0.8

        _Color5 ("Color 5", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline"="UniversalPipeline" }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            // For ComputeBuffers
            #pragma prefer_hlslcc gles
            #pragma target 5.0

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            CBUFFER_START(UnityPerMaterial)
            half4 _Color1;
            half4 _Color2;
            half4 _Color3;
            half4 _Color4;
            half4 _Color5;

            float _Limit1;
            float _Limit2;
            float _Limit3;
            float _Limit4;
            CBUFFER_END 
           

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;


            StructuredBuffer<float> noiseResults;
            int _Resolution;
            bool _Colorize;
            bool _Interpolated;

            float4 InterpolateColors(float t, float4 color1, float4 color2, float4 color3, float4 color4, float4 color5)
            {       
                // Ensure t is within the range [0, 1]
                t = saturate(t);

                // Calculate segment index and fractional part within the segment
                float segment = t * 4.0; // Divide the range [0, 1] into four segments
                int segmentIndex = min(floor(segment), 3); // Clamp to the last segment index
                float tInSegment = frac(segment);

                // Define the colors of the segments
                float4 colors[5];
                colors[0] = color1;
                colors[1] = color2;
                colors[2] = color3;
                colors[3] = color4;
                colors[4] = color5;

                // Interpolate between the colors of the current segment and the next segment
                float4 interpolatedColor = lerp(colors[segmentIndex], colors[segmentIndex + 1], tInSegment);

                return interpolatedColor;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                int x = i.uv.x * (_Resolution - 1);
                int y = i.uv.y * (_Resolution - 1);
                float nVal = noiseResults[x + y * _Resolution];
                if (!_Colorize) {return fixed4(nVal, nVal, nVal, 1.0);}
                if (!_Interpolated) 
                {
                    fixed4 col = fixed4(1, 1, 1, 1);
                    if (nVal < _Limit1 ) { col = _Color1; }
                    else if (nVal < _Limit2) { col = _Color2; }
                    else if (nVal < _Limit3) { col = _Color3; }
                    else if (nVal < _Limit4) { col = _Color4; }
                    else { col = _Color5; }
                    return col;
                }
                return InterpolateColors(nVal, _Color1, _Color2, _Color3, _Color4, _Color5);
            }
            ENDHLSL
        }
    }
}
