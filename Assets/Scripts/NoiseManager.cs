using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class NoiseManager: SettingsManager
{
    // Timing Option
    // Display Options
    [Header("Display Options")]
    [SerializeField]
    private RawImage displayImage;
    [SerializeField]
    private Material noiseMaterial;
    [SerializeField]
    public NoiseDisplayData noiseDisplayData;

    [SerializeField, Header("Noise Options")]
    public bool timeExecutions;
    [SerializeField]
    public ExecutionType executionType;
    [SerializeField]
    public NoiseData noiseData;

    // Data that we calculate
    private Texture2D noiseTexture;
    private float[] noiseArray;
    private ComputeBuffer noiseResultsBuffer;

    private Stopwatch stopWatch = new Stopwatch();

    public bool trackTime;
    private int DeltaTimeCounter = 0;
    private DeltaTimeTracker[] timeTrackers;


    private void Awake()
    {
        noiseResultsBuffer = new ComputeBuffer(noiseData.resolution * noiseData.resolution, sizeof(float), ComputeBufferType.Default);
        noiseArray = new float[noiseResultsBuffer.count];
        GPUNoise.InitializeBuffers(noiseResultsBuffer, noiseData);
        noiseMaterial.SetBuffer("noiseResults", noiseResultsBuffer);
        noiseMaterial.SetInt("_Resolution", noiseData.resolution);

        if (displayImage == null) displayImage = FindAnyObjectByType<RawImage>();

        noiseDisplayData.OnValuesUpdated -= UpdateDisplayImage;
        noiseDisplayData.OnValuesUpdated += UpdateDisplayImage;
    }

    private void Start()
    {
        if (trackTime)
        {
            noiseData.resolution = 2;
            executionType = ExecutionType.MultiThread;
            noiseData.octaveNoiseFilters[0].filterType = NoiseType.Value;

            ReseteDeltaTimeTrackers();
        }
    }

    private void OnValidate()
    {
        if (autoUpdate) UpdateValues();
    }
    private void OnDestroy()
    {
        DestroyImmediate(noiseTexture);
        GPUNoise.DisposeOBuffers();
        noiseResultsBuffer.Dispose();
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;
        if (noiseData.AutoOffset)
        {
            foreach (NoiseFilter filter in noiseData.octaveNoiseFilters)
            {
                filter.offset += (noiseData.autoOffsetVector * deltaTime * noiseData.autoOffsetAmount);
                if (filter.autoOffset)
                {
                    filter.offset += (filter.autoOffsetAmount * deltaTime);
                }
            }
            UpdateValues();
        }

        if (trackTime)
        {

            DeltaTimeCounter += 1;
            if (DeltaTimeCounter >= 0)
            {
                if (!stopWatch.IsRunning)
                {
                    stopWatch.Start();
                }
                GenerateNoise();
                stopWatch.Stop();
                timeTrackers[(int)noiseData.octaveNoiseFilters[0].filterType].addTime((float)stopWatch.Elapsed.TotalSeconds);
                stopWatch.Restart();
            }
            if (DeltaTimeCounter >= 20) { UpdateDeltaTimeTrackers(); }
        }
    }

    public override void UpdateValues()
    {
        if (noiseResultsBuffer == null || noiseData.resolution * noiseData.resolution != noiseResultsBuffer.count)
        {
            if (noiseResultsBuffer != null)
            {
                noiseResultsBuffer.Dispose();
            }
            noiseResultsBuffer = new ComputeBuffer(noiseData.resolution * noiseData.resolution, sizeof(float), ComputeBufferType.Default);
            GPUNoise.UpdateResultsBuffer(noiseResultsBuffer);

            noiseMaterial.SetBuffer("noiseResults", noiseResultsBuffer);
            noiseMaterial.SetInt("_Resolution", noiseData.resolution);

            noiseArray = new float[noiseResultsBuffer.count];
        }
        GPUNoise.UpdateNoiseDataBufferData(noiseData);
        GPUNoise.UpdateFiltersBufferData(noiseData);
        GenerateNoise();
    }

    private void UpdateDeltaTimeTrackers()
    {
        DeltaTimeCounter = -1;
        NoiseType currentFilter = noiseData.octaveNoiseFilters[0].filterType;
        timeTrackers[(int)currentFilter].calculateAverage(noiseData.resolution.ToString());

        if (noiseData.resolution < 4096 * 4)
        {
            noiseData.resolution *= 2;
        }
        else
        {
            noiseData.resolution = 2;
            if ((int)currentFilter < 3)
            {
                int nextFilterIndex = (int)currentFilter + 1;
                noiseData.octaveNoiseFilters[0].filterType = (NoiseType)nextFilterIndex;
            }
            else
            {
                noiseData.octaveNoiseFilters[0].filterType = 0;
                SaveLoadSystem.SaveDeltaTimesToCsv(executionType.ToString(), timeTrackers);
                ReseteDeltaTimeTrackers();
                if (executionType == ExecutionType.ComputeWithoutGet)
                {
                    Application.Quit();
                }
                executionType = (ExecutionType)((int)executionType + 1);

            }

        }
        UpdateValues();
        stopWatch.Restart();
    }

    private void ReseteDeltaTimeTrackers()
    {
        timeTrackers = new DeltaTimeTracker[4];

        for (int i = 0; i < 4; i++)
        {
            NoiseType nt = (NoiseType)i;
            timeTrackers[i] = new DeltaTimeTracker(nt.ToString());
        }
    }

    public void GenerateNoise()
    {
        switch (executionType)
        {
            case ExecutionType.SingleThread:
                MTNoise.GetNoise2D(noiseData, noiseArray, false);
                noiseResultsBuffer.SetData(noiseArray);
                break;
            case ExecutionType.MultiThread:
                MTNoise.GetNoise2D(noiseData, noiseArray, true);
                noiseResultsBuffer.SetData(noiseArray);
                break;
            case ExecutionType.ComputeShader:
                GPUNoise.CalcNoise(noiseData);
                noiseResultsBuffer.GetData(noiseArray);
                break;
            case ExecutionType.ComputeWithoutGet:
                GPUNoise.CalcNoise(noiseData);
                break;
            default:
                break;
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
            UnityEngine.Debug.Log("TextureGeneration: " + elapsedTime + " seconds --- " + executionType.ToString());
        }


        noiseTexture.Apply();
    }

    public void UpdateDisplayImage()
    {
        noiseMaterial.SetInt("_Colorize", noiseDisplayData.Colorize ? 1 : 0);
        noiseMaterial.SetInt("_Interpolated", noiseDisplayData.Interpolated ? 1 : 0);

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
        GenerateTexture();
        SaveLoadSystem.SaveTexture(noiseData, noiseTexture);
    }
    public override void Load()
    {
        Destroy(noiseTexture);
        SaveLoadSystem.LoadData<NoiseData>(noiseData, "Noises", noiseData.name + ".json");

        noiseTexture = SaveLoadSystem.LoadTexture(noiseData);
        SetTextureToDisplay();
    }
}

public enum ExecutionType
{
    SingleThread,
    MultiThread,
    ComputeShader,
    ComputeWithoutGet
}