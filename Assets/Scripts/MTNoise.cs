using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public static class MTNoise
{
    public static void GetNoise2D(NoiseData noiseData, float[] noiseArray, bool scheduleParalel)
    {
        NativeArray<float> noiseResult = new NativeArray<float>(noiseData.resolution * noiseData.resolution, Allocator.Persistent);
        NoiseData.SNoiseData sNoiseData = noiseData.GetAsStruct();

        int noiseFilterCount = noiseData.octaveNoiseFilters.Count;
        NativeArray<NoiseFilter.SNoiseFilterGPU> sNoiseFilters = new NativeArray<NoiseFilter.SNoiseFilterGPU>(noiseFilterCount, Allocator.Persistent);

        // If normalization
        float normalizerSum = 1f;
        float normalizer = 0f;


        for (int i = 0; i < noiseFilterCount; i++)
        {
            sNoiseFilters[i] = noiseData.octaveNoiseFilters[i].GetAsGPUStruct();
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

        JobHandle jobHandle;
        if (scheduleParalel)
        {
            jobHandle = noiseJob.ScheduleParallel(noiseData.resolution * noiseData.resolution, 1, default);
        }
        else
        {
            jobHandle = noiseJob.Schedule(noiseData.resolution * noiseData.resolution, default);
        }
        jobHandle.Complete();

        noiseResult.CopyTo(noiseArray);

        noiseResult.Dispose();
        sNoiseFilters.Dispose();
    }


    public static Texture2D GetTextureFromNoiseArray(float[] noiseArray, int resolution)
    {
        if (noiseArray == null) return null;

        NativeArray<Color> colors = new NativeArray<Color>(noiseArray.Length, Allocator.Persistent);
        NativeArray<float> noiseNativeArray = new NativeArray<float>(noiseArray.Length, Allocator.Persistent);

        ArrayToTextureJob arrayToTextureJob = new ArrayToTextureJob
        {
            colors = colors,
            noiseArray = noiseNativeArray
        };

        JobHandle jobHandle = arrayToTextureJob.ScheduleParallel(colors.Length, 1, default);

        jobHandle.Complete();

        Texture2D noiseTexture = new Texture2D(resolution, resolution);
        noiseTexture.filterMode = FilterMode.Point;
        noiseTexture.SetPixels(colors.ToArray());
        colors.Dispose();
        noiseNativeArray.Dispose();
        Resources.UnloadUnusedAssets();

        return noiseTexture;
    }
}
