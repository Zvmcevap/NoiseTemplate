using Unity.Collections;
using UnityEngine;

public static class GPUNoise
{
    public static ComputeShader computeShader = Resources.Load<ComputeShader>("NoiseMainCS");
    public static ComputeBuffer noiseResultsBuffer = new ComputeBuffer(1, sizeof(float), ComputeBufferType.Default);

    public static NativeArray<float> GetNoise2D(NoiseData noiseData)
    {
        // Prepare Compute Shader and Buffers
        if (computeShader == null)
        {
            Debug.LogError("Failed To Load Compute Shader!");
            return new NativeArray<float>(0, Allocator.Temp);
        }
        if (noiseResultsBuffer.count != noiseData.resolution * noiseData.resolution) { UpdateResultsBuffer(noiseData); }

        int octaves = noiseData.octaveNoiseFilters.Count;
        ComputeBuffer noiseDataBuffer = new ComputeBuffer(1, sizeof(float) * 2 + sizeof(int) * 5);
        ComputeBuffer noiseFiltersBuffer = new ComputeBuffer(octaves, sizeof(float) * 11 + sizeof(int) * 3);

        // Prepare Data for Buffers and Variables
        NoiseData.SNoiseDataGPU[] noiseDataArray = { noiseData.GetAsGPUStruct() };
        NoiseFilter.SNoiseFilterGPU[] noiseFilterArray = new NoiseFilter.SNoiseFilterGPU[octaves];

        float normalizerSum = 1f;
        float normalizer = 0f;
        for (int i = 0; i < octaves; i++)
        {
            NoiseFilter nFilter = noiseData.octaveNoiseFilters[i];
            noiseFilterArray[i] = nFilter.GetAsGPUStruct();
            normalizer += nFilter.amplitude * normalizerSum;
            normalizerSum *= noiseData.persistence;
        }

        // Set Data
        int kernelIndex = computeShader.FindKernel("GenerateNoise2D");
        computeShader.SetBuffer(kernelIndex, "noiseResults", noiseResultsBuffer);

        noiseDataBuffer.SetData(noiseDataArray);
        noiseFiltersBuffer.SetData(noiseFilterArray);
        computeShader.SetBuffer(kernelIndex, "noiseDataBuffer", noiseDataBuffer);
        computeShader.SetBuffer(kernelIndex, "noiseFilters", noiseFiltersBuffer);

        int octavesID = Shader.PropertyToID("octaves");
        int normalierID = Shader.PropertyToID("normalizer");
        computeShader.SetInt(octavesID, octaves);
        computeShader.SetFloat(normalierID, normalizer);

        // Run and Get Data
        int groupSize = Mathf.CeilToInt(((float)noiseData.resolution) / 32f);
        computeShader.Dispatch(kernelIndex, groupSize, groupSize, 1);

        float[] noiseResultsArray = new float[noiseData.resolution * noiseData.resolution];
        noiseResultsBuffer.GetData(noiseResultsArray);
        NativeArray<float> noiseResultsNative = new NativeArray<float>(noiseResultsArray, Allocator.Persistent);

        noiseDataBuffer.Dispose();
        noiseFiltersBuffer.Dispose();

        return noiseResultsNative;
    }

    public static void UpdateResultsBuffer(NoiseData noiseData)
    {
        noiseResultsBuffer.Dispose();
        noiseResultsBuffer = new ComputeBuffer(noiseData.resolution * noiseData.resolution, sizeof(float), ComputeBufferType.Default);
    }

    public static void DisposeOfResultsBuffer() { noiseResultsBuffer.Dispose(); }
}
