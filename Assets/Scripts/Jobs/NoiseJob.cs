using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using static Unity.Mathematics.math;


[BurstCompile(CompileSynchronously = true)]
struct NoiseJob: IJobFor
{
    [WriteOnly]
    public NativeArray<float> noiseResult;
    [ReadOnly]
    public NoiseData.SNoiseData noiseData;
    [ReadOnly]
    public NativeArray<NoiseFilter.SNoiseFilterGPU> noiseFilters;

    [ReadOnly]
    public float normalizer;

    public void Execute(int i)
    {
        int x = i % noiseData.resolution;
        int y = i / noiseData.resolution;

        float amplitude = 1f;
        float frequency = 1f;

        float endValue = 0f;

        for (int oct = 0; oct < noiseFilters.Length; oct++)
        {
            NoiseFilter.SNoiseFilterGPU noiseFilter = noiseFilters[oct];

            float nfAmplitude = noiseFilter.amplitude;
            float nfFrequency = noiseFilter.frequency;
            float nfNormalizer = 0f;

            float nfValue = 0f;
            for (int fOct = 0; fOct < noiseFilter.octaves; fOct++)
            {

                Unity.Mathematics.Random octavePrng = new Unity.Mathematics.Random((uint)noiseFilter.seed);
                float3 octaveOffset = octavePrng.NextInt(100);

                float3 sample = new float3(x, y, 0f) / noiseFilter.scale * nfFrequency * frequency + noiseFilter.offset + octaveOffset;

                float noiseValue = 0f;
                switch (noiseFilter.noiseType)
                {
                    case 1: // Perlin
                        noiseValue = noise.cnoise(sample);
                        break;
                    case 2: // Simplex
                        noiseValue = noise.snoise(sample);
                        break;
                    case 3: // Cell
                        noiseValue = noise.cellular(sample).x;
                        noiseValue = noiseValue * 2f - 1f;
                        break;
                    case 5: // Sinus
                        noiseValue = noiseData.dimensions == 0 ? sin(sample.x + sample.y) : sin(sample.x + sample.y + sample.z);
                        break;
                    default:
                        break;
                }
                nfValue += noiseValue * nfAmplitude;

                nfNormalizer += nfAmplitude;
                nfFrequency *= noiseFilter.lacunarity;
                nfAmplitude *= noiseFilter.persistance;
            }
            if (nfNormalizer > 0)
            {
                nfValue /= nfNormalizer;
            }

            endValue += nfValue * amplitude;

            amplitude *= noiseData.persistence;
            frequency *= noiseData.lacunarity;
        }
        endValue /= normalizer;

        if (noiseData.invert)
        {
            endValue *= -1.0f;
        }
        if (noiseData.normalize01)
        {
            endValue = endValue * .5f + .5f;
        }
        noiseResult[x + y * noiseData.resolution] = endValue;
    }
}
