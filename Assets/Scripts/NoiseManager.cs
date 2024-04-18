using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NoiseManager: SettingsManager
{
    // Timing Option
    [SerializeField]
    public bool timeExecutions;
    // Display Options
    [SerializeField]
    private RawImage displayImage;
    [SerializeField]
    NoiseDisplayData noiseDisplayData;
    [SerializeField]
    public ExecutionType executionType;
    [SerializeField]
    NoiseData noiseData;
    [SerializeField]
    public Texture2D noiseTexture;

    [SerializeField]
    public NativeArray<float> noiseArray;

    private void Awake()
    {
        if (displayImage == null) displayImage = FindAnyObjectByType<RawImage>();
        noiseDisplayData.OnValuesUpdated -= UpdateDisplayImage;
        noiseDisplayData.OnValuesUpdated += UpdateDisplayImage;
        noiseTexture = SaveLoadSystem.LoadTexture(noiseData);
    }

    private void OnValidate()
    {
        if (autoUpdate) UpdateValues();
    }

    private void OnDestroy()
    {
        if (noiseArray.IsCreated) { noiseArray.Dispose(); }
        DestroyImmediate(noiseTexture);

    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;
        if (noiseData.AutoOffset)
        {
            foreach (NoiseFilter filter in noiseData.octaveNoiseFilters)
            {
                filter.offset += (noiseData.autoOffsetAmount * deltaTime);
                if (filter.autoOffset)
                {
                    filter.offset += (filter.autoOffsetAmount * deltaTime);
                }
            }
            UpdateValues();
        }
    }

    public override void UpdateValues()
    {
        GenerateNoise();
        GenerateTexture();
        SetTextureToDisplay();
    }

    public void GenerateNoise()
    {
        if (noiseArray.IsCreated) { noiseArray.Dispose(); Resources.UnloadUnusedAssets(); }

        float startTime = 0f;
        if (timeExecutions)
        {
            startTime = Time.realtimeSinceStartup;
        }

        switch (executionType)
        {
            case ExecutionType.SingleThread:
                GPUNoise.DisposeOfResultsBuffer();
                noiseArray = STNoise.GetNoise2D(noiseData);
                break;
            case ExecutionType.MultiThread:
                GPUNoise.DisposeOfResultsBuffer();
                noiseArray = MTNoise.GetNoise2D(noiseData);
                break;
            case ExecutionType.ComputeShader:
                noiseArray = GPUNoise.GetNoise2D(noiseData);
                break;
            default:
                break;
        }
        if (timeExecutions)
        {
            float elapsedTime = Time.realtimeSinceStartup - startTime;
            Debug.Log("NoiseGeneration: " + elapsedTime + " seconds --- " + executionType.ToString());
        }
    }

    public void GenerateTexture()
    {
        if (noiseArray == null) return;

        DestroyImmediate(noiseTexture);
        Resources.UnloadUnusedAssets();


        float startTime = 0f;
        if (timeExecutions)
        {
            startTime = Time.realtimeSinceStartup;
        }

        switch (executionType)
        {
            case ExecutionType.SingleThread:
                noiseTexture = STNoise.GetTextureFromNoiseArray(noiseArray, noiseData.resolution);
                break;
            case ExecutionType.MultiThread:
                noiseTexture = MTNoise.GetTextureFromNoiseArray(noiseArray, noiseData.resolution);
                break;
            case ExecutionType.ComputeShader:
                noiseTexture = MTNoise.GetTextureFromNoiseArray(noiseArray, noiseData.resolution);
                break;
            default:
                break;
        }
        if (timeExecutions)
        {
            float elapsedTime = Time.realtimeSinceStartup - startTime;
            Debug.Log("TextureGeneration: " + elapsedTime + " seconds --- " + executionType.ToString());
        }


        noiseTexture.Apply();
    }

    public void UpdateDisplayImage()
    {
        noiseDisplayData.UpdateDisplay(displayImage);
    }

    public void SetTextureToDisplay()
    {
        if (noiseTexture == null) return;
        displayImage.texture = noiseTexture;
    }

    public override void Save()
    {
        SaveLoadSystem.SaveData<NoiseData>(noiseData, "Noises", noiseData.name);
        SaveLoadSystem.SaveTexture(noiseData, noiseTexture);
    }
    public override void Load()
    {
        Destroy(noiseTexture);
        if (noiseArray.IsCreated) { noiseArray.Dispose(); }
        SaveLoadSystem.LoadData<NoiseData>(noiseData, "Noises", noiseData.name + ".json");

        noiseTexture = SaveLoadSystem.LoadTexture(noiseData);
        SetTextureToDisplay();
    }
}

public enum ExecutionType
{
    SingleThread,
    MultiThread,
    ComputeShader
}