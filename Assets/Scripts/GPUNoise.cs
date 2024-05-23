using UnityEngine;

public static class GPUNoise
{
    public static ComputeShader computeShader = Resources.Load<ComputeShader>("NoiseMainCS");
    public static ComputeBuffer noiseDataBuffer = new ComputeBuffer(1, sizeof(float) * 2 + sizeof(int) * 4);
    public static ComputeBuffer noiseFilters = new ComputeBuffer(1, sizeof(float) * 20 + sizeof(int) * 10);

    public static void CalcNoise(NoiseData noiseData)
    {
        // Prepare Compute Shader and Buffers

        int octaves = noiseData.octaveNoiseFilters.Count;

        float normalizerSum = 1f;
        float normalizer = 0f;
        for (int i = 0; i < octaves; i++)
        {
            NoiseFilter nFilter = noiseData.octaveNoiseFilters[i];


            normalizer += nFilter.amplitude * normalizerSum;
            normalizerSum *= noiseData.persistence;
        }
        // Set Data
        string kernelName = "GenerateNoise";
        int kernelIndex = computeShader.FindKernel(kernelName);


        int octavesID = Shader.PropertyToID("octaves");
        int normalierID = Shader.PropertyToID("normalizer");

        computeShader.SetInt(octavesID, octaves);
        computeShader.SetFloat(normalierID, normalizer);
        // Run
        int groupSize = Mathf.CeilToInt(((float)noiseData.resolution) / 16f);
        computeShader.Dispatch(kernelIndex, groupSize, groupSize, 1);

    }

    public static void UpdateNoiseDataBufferData(NoiseData noiseData)
    {
        if (noiseDataBuffer == null) return;
        NoiseData.SNoiseDataGPU[] noiseDataArray = { noiseData.GetAsGPUStruct() };
        noiseDataBuffer.SetData(noiseDataArray);
    }

    public static void UpdateFiltersBufferData(NoiseData noiseData)
    {
        int octaves = noiseData.octaveNoiseFilters.Count;
        if (!noiseFilters.IsValid() || noiseFilters.count != octaves)
        {
            noiseFilters.Dispose();
            noiseFilters = new ComputeBuffer(octaves, sizeof(float) * 20 + sizeof(int) * 10);
            int kernelIndex = computeShader.FindKernel("GenerateNoise");

            computeShader.SetBuffer(kernelIndex, "noiseFilters", noiseFilters);
        }
        NoiseFilter.SNoiseFilterGPU[] noiseFilterArray = new NoiseFilter.SNoiseFilterGPU[octaves];

        for (int i = 0; i < octaves; i++)
        {
            NoiseFilter nFilter = noiseData.octaveNoiseFilters[i];
            noiseFilterArray[i] = nFilter.GetAsGPUStruct();
        }

        noiseFilters.SetData(noiseFilterArray);
    }

    public static void InitializeBuffers(ComputeBuffer noiseResultsBuffer, NoiseData noiseData)
    {
        DisposeOBuffers();
        int kernelIndex = computeShader.FindKernel("GenerateNoise");

        noiseDataBuffer = new ComputeBuffer(1, sizeof(float) * 2 + sizeof(int) * 4);
        UpdateNoiseDataBufferData(noiseData);
        UpdateFiltersBufferData(noiseData);

        computeShader.SetBuffer(kernelIndex, "noiseResults", noiseResultsBuffer);
        computeShader.SetBuffer(kernelIndex, "noiseDataBuffer", noiseDataBuffer);
    }

    public static void UpdateResultsBuffer(ComputeBuffer noiseResultsBuffer)
    {
        int kernelIndex = computeShader.FindKernel("GenerateNoise");
        computeShader.SetBuffer(kernelIndex, "noiseResults", noiseResultsBuffer);
    }

    public static void DisposeOBuffers()
    {
        noiseDataBuffer.Dispose();
        noiseFilters.Dispose();
    }
}
