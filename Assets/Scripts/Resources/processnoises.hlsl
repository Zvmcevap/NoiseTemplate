
#include "../Lygia/generative/noised.hlsl"
#include "../Lygia/generative/gnoise.hlsl"
#include "../Lygia/generative/cnoise.hlsl"
#include "../Lygia/generative/snoise.hlsl"
#include "../Lygia/generative/worley.hlsl"
#include "../Lygia/generative/wavelet.hlsl"

#include "dataobjects.hlsl"


static float PingPong(float t, float pingPongStr)
{
    t += 1;
    t *= pingPongStr;
    t -= (int) (t * 0.5f) * 2;
    return t < 1 ? t : 2 - t;
}

static float Ridge(float t)
{
    t = abs(t);
    return -2 * t + 1;
}

static inline float Power(float t, float power)
{
    t = 0.5 * t + 0.5; // wanna keep it in range between 1 and 0
    t = pow(abs(t), power);
    return 2 * t - 1;
}


static float EvaluateNoise(float3 sample, int noiseType, int cellReturnType = 1, float jitter = 1, int dimensions = 1)
{
    float noiseValue = 0;
    switch (noiseType)
    {
        case 0: // Value
            noiseValue = dimensions == 0 ? gnoise(sample.xy) : gnoise(sample);
            // noiseValue = -1. + 2 * noiseValue;
            break;
        case 1: // Perlin
            noiseValue = dimensions == 0 ? cnoise(sample.xy) : cnoise(sample);
            break;
        case 2: // Simplex
            noiseValue = dimensions == 0 ? snoise(sample.xy) : snoise(sample);
            break;
        case 3: // Cellülar
            switch (cellReturnType)
            {
                case 0:
                    noiseValue = dimensions == 0 ? worley(sample.xy, jitter).z : worley(sample, jitter).z; // Voronoi tiles
                    break;
                case 1:
                    noiseValue = dimensions == 0 ? worley(sample.xy, jitter).x : worley(sample, jitter).x; // Distance 1
                    break;
                case 2:
                    noiseValue = dimensions == 0 ? worley(sample.xy, jitter).y : worley(sample, jitter).y; // Distance 2
                    break;
                case 3:
                    noiseValue = dimensions == 0 ? worley(sample.xy, jitter).x / worley(sample.xy, jitter).y : worley(sample, jitter).x / worley(sample, jitter).y; // Divide, interesting pattern is all
                    break;
            }
            noiseValue = -1. + 2. * noiseValue;
            break;
        case 4: // Wavelet
            noiseValue = dimensions == 0 ? wavelet(sample.xy) : wavelet(sample);
            break;
        case 5: 
            noiseValue = dimensions == 0 ? sin(sample.x + sample.y) : sin(sample.x + sample.y + sample.z);
            break;
    }
    return noiseValue;
}

static float ProcessNoiseFilter(NoiseFilter noiseFilter, NoiseData noiseData, float3 sample) // Always return -1, 1 range
{
    // Step 1 EVALUATE the noise
    float noiseValue = EvaluateNoise(sample, noiseFilter.noiseType, noiseFilter.cellularReturnType, noiseFilter.jitter, noiseData.dimensions);


    // Step 2 MODIFIERS - do funky stuff to it
    switch (noiseFilter.modifiers)
    {
        case 1:
            noiseValue = Ridge(noiseValue);
            break;
        case 2:
            noiseValue = PingPong(noiseValue, noiseFilter.pingPongStrength);
            break;
        default:
            break;
    }
    
    if (noiseFilter.pow)
    {
        noiseValue = Power(noiseValue, noiseFilter.powAmount);
    }

    if (noiseFilter.invert)
    {
        noiseValue *= -1.0;
    }
    return noiseValue;
}


static float3 DoDomainWarp(float3 sample, int noiseType, int octaves, float3 scale, float amplitude, float frequency, float persistance, float lacunarity)
{   
    for (int oct = 0; oct < octaves; oct++)
    {
         // Generate random offset for domain warping
        float noiseValue = EvaluateNoise(sample * frequency, noiseType);
        //float dx = scale.x * EvaluateNoise(sample * frequency, noiseType);
        //float dy = scale.y * EvaluateNoise(sample * frequency, noiseType);
        //float dz = scale.z * EvaluateNoise(sample * frequency, noiseType);

        sample += amplitude * float3(noiseValue, noiseValue, noiseValue) * scale;
        
        amplitude *= persistance;
        frequency *= lacunarity;
    }

    return sample;
}
