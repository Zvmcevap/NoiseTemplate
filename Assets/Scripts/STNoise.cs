using Unity.Burst;
using Unity.Collections;
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
        return noiseResult;
    }

    public static Texture2D GetTextureFromNoiseArray(float[] noiseArray, int resolution)
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
