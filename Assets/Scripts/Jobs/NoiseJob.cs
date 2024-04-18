using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;


[BurstCompile(CompileSynchronously = true)]
struct NoiseJob: IJobFor
{
    [WriteOnly]
    public NativeArray<float> noiseResult;
    [ReadOnly]
    public NoiseData.SNoiseData noiseData;
    [ReadOnly]
    public NativeArray<NoiseFilter.SNoiseFilter> noiseFilters;

    [ReadOnly]
    public float normalizer;

    public void Execute(int i)
    {
        int x = i % noiseData.resolution;
        int y = i / noiseData.resolution;

        float amplitude = 1f;
        float frequency = 1f;

        float endValue = 0f;

        Unity.Mathematics.Random prng = new Unity.Mathematics.Random(noiseData.seed);
        for (int oct = 0; oct < noiseFilters.Length; oct++)
        {
            NoiseFilter.SNoiseFilter noiseFilter = noiseFilters[oct];
            // Randomizer
            Unity.Mathematics.Random octavePrng = new Unity.Mathematics.Random(noiseFilter.seed);
            float3 octaveOffset = noiseData.separateOctaveSeeds ? octavePrng.NextInt(100000) : prng.NextInt(100000);

            float3 sample = new float3(x, y, 0f) / noiseFilter.scale * noiseFilter.frequency * frequency + noiseFilter.offset + octaveOffset;

            float noiseValue = 0f;
            switch (noiseFilter.filterType)
            {
                case 0:
                    noiseValue = noise.cnoise(sample);
                    break;
                case 1:
                    noiseValue = noise.pnoise(sample, noiseFilter.period);
                    break;
                case 2:
                    noiseValue = noise.snoise(sample);
                    break;
                case 3:
                    noiseValue = noise.cellular(sample).x * 2f - 1f;
                    break;
                case 4:
                    noiseValue = noise.cellular(sample).y * 2f - 1f;
                    break;
                default:
                    break;
            }
            if (noiseFilter.invert)
            {
                noiseValue *= -1;
            }
            endValue += noiseValue * noiseFilter.amplitude * amplitude;

            frequency *= noiseData.lacunarity;
            amplitude *= noiseData.persistence;
        }
        if (noiseData.normalize01)
        {
            endValue = endValue / (normalizer * 2f) + 0.5f;
        }
        if (noiseData.invert)
        {
            if (noiseData.normalize01)
            {
                endValue = 1 - endValue;
            }
            else
            {
                endValue *= -1;
            }
        }
        noiseResult[i] = endValue;
    }
}
