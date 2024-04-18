using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public static class MTNoise
{
    public static NativeArray<float> GetNoise2D(NoiseData noiseData)
    {
        NativeArray<float> noiseResult = new NativeArray<float>(noiseData.resolution * noiseData.resolution, Allocator.Persistent);
        NoiseData.SNoiseData sNoiseData = noiseData.GetAsStruct();

        int noiseFilterCount = noiseData.octaveNoiseFilters.Count;
        NativeArray<NoiseFilter.SNoiseFilter> sNoiseFilters = new NativeArray<NoiseFilter.SNoiseFilter>(noiseFilterCount, Allocator.Persistent);

        // If normalization
        float normalizerSum = 1f;
        float normalizer = 0f;


        for (int i = 0; i < noiseFilterCount; i++)
        {
            sNoiseFilters[i] = noiseData.octaveNoiseFilters[i].GetAsStruct();
            if (noiseData.normalize01)
            {
                normalizer += sNoiseFilters[i].amplitude * normalizerSum;
                normalizerSum *= noiseData.persistence;
            }
        }

        NoiseJob noiseJob = new NoiseJob
        {
            noiseResult = noiseResult,
            noiseData = sNoiseData,
            noiseFilters = sNoiseFilters,
            normalizer = normalizer
        };

        JobHandle jobHandle = noiseJob.ScheduleParallel(noiseData.resolution * noiseData.resolution, 1, default);
        jobHandle.Complete();
        sNoiseFilters.Dispose();

        return noiseResult;
    }


    public static Texture2D GetTextureFromNoiseArray(NativeArray<float> noiseArray, int resolution)
    {
        if (noiseArray == null) return null;

        NativeArray<Color> colors = new NativeArray<Color>(noiseArray.Length, Allocator.Persistent);

        ArrayToTextureJob arrayToTextureJob = new ArrayToTextureJob
        {
            colors = colors,
            noiseArray = noiseArray
        };

        JobHandle jobHandle = arrayToTextureJob.ScheduleParallel(colors.Length, 1, default);

        jobHandle.Complete();
        Texture2D noiseTexture = new Texture2D(resolution, resolution);
        noiseTexture.filterMode = FilterMode.Point;
        noiseTexture.SetPixels(colors.ToArray());
        colors.Dispose();
        Resources.UnloadUnusedAssets();

        return noiseTexture;
    }
}
