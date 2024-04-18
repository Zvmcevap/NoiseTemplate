using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;


// Single Threaded Noise Functions
public static class STNoise
{
    [BurstCompile(CompileSynchronously = true)]
    public static NativeArray<float> GetNoise2D(NoiseData noiseData)
    {

        NativeArray<float> noiseResult = new NativeArray<float>(noiseData.resolution * noiseData.resolution, Allocator.Persistent);

        // If normalization
        float normalizerSum = 1f;
        float normalizer = 0f;

        if (noiseData.normalize01)
        {
            foreach (NoiseFilter nFilter in noiseData.octaveNoiseFilters)
            {
                normalizer += nFilter.amplitude * normalizerSum;
                normalizerSum *= noiseData.persistence;
            }
        }

        // Randomizer
        Unity.Mathematics.Random prng = new Unity.Mathematics.Random(noiseData.seed);
        float3[] octaveOffsets = new float3[noiseData.octaveNoiseFilters.Count];
        for (int i = 0; i < noiseData.octaveNoiseFilters.Count; i++)
        {
            Unity.Mathematics.Random octavePrng = new Unity.Mathematics.Random(noiseData.octaveNoiseFilters[i].seed);
            octaveOffsets[i] = noiseData.separateOctaveSeeds ? octavePrng.NextInt(100000) : prng.NextInt(100000);
        }


        // Main Noise Builder
        for (int y = 0; y < noiseData.resolution; y++)
        {

            for (int x = 0; x < noiseData.resolution; x++)
            {
                float amplitude = 1f;
                float frequency = 1f;

                for (int i = 0; i < noiseData.octaveNoiseFilters.Count; i++)
                {
                    NoiseFilter noiseFilter = noiseData.octaveNoiseFilters[i];
                    float3 sample = new float3(x, y, 0f) / noiseFilter.scale * noiseFilter.frequency * frequency + noiseFilter.offset + octaveOffsets[i];

                    float noiseValue = 0f;
                    switch (noiseFilter.filterType)
                    {
                        case NoiseFilterType.Perlin:
                            noiseValue = noise.cnoise(sample);
                            break;
                        case NoiseFilterType.PerlinPeriodic:
                            noiseValue = noise.pnoise(sample, noiseFilter.repeatPeriod);
                            break;
                        case NoiseFilterType.Simplex:
                            noiseValue = noise.snoise(sample);
                            break;
                        case NoiseFilterType.WorleyF1:
                            noiseValue = noise.cellular(sample).x * 2f - 1f;
                            break;
                        case NoiseFilterType.WorleyF2:
                            noiseValue = noise.cellular(sample).y * 2f - 1f;
                            break;
                        default:
                            break;
                    }
                    if (noiseFilter.invert)
                    {
                        noiseValue *= -1;
                    }
                    noiseResult[y * noiseData.resolution + x] += noiseValue * noiseFilter.amplitude * amplitude;

                    frequency *= noiseData.lacunarity;
                    amplitude *= noiseData.persistence;
                }
                if (noiseData.normalize01)
                {
                    noiseResult[y * noiseData.resolution + x] = noiseResult[y * noiseData.resolution + x] / (normalizer * 2) + 0.5f;
                    Debug.Log(noiseResult[y * noiseData.resolution + x].ToString());
                }
                if (noiseData.invert)
                {
                    if (noiseData.normalize01)
                    {
                        noiseResult[y * noiseData.resolution + x] = 1 - noiseResult[y * noiseData.resolution + x];
                    }
                    else
                    {
                        noiseResult[y * noiseData.resolution + x] *= -1;
                    }
                }
            }
        }
        return noiseResult;
    }

    public static Texture2D GetTextureFromNoiseArray(NativeArray<float> noiseArray, int resolution)
    {
        if (noiseArray == null) return null;

        NativeArray<Color> colors = new NativeArray<Color>(noiseArray.Length, Allocator.Persistent);

        for (int i = 0; i < noiseArray.Length; i++)
        {
            float noiseValue = noiseArray[i];
            colors[i] = new Color(noiseValue, noiseValue, noiseValue);
        }


        Texture2D finalTexture = new Texture2D(resolution, resolution);
        finalTexture.filterMode = FilterMode.Point;
        finalTexture.SetPixels(colors.ToArray());
        colors.Dispose();

        return finalTexture;
    }

}
