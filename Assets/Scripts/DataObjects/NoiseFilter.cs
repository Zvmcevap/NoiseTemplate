using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
public class NoiseFilter
{
    [Header("Noise Filter")]
    public NoiseType filterType = NoiseType.Value;
    // Cellular
    [Header("Celular Only")]
    public CellReturnType cellReturnType = CellReturnType.Distance1;
    [Range(0.0f, 2f)]
    public float jitter;
    [Header("Scroll Through Time")]
    public bool autoOffset;
    public float3 autoOffsetAmount;

    [Header("Variables")]
    public uint seed = 0;

    public float3
        offset = new(0f, 0f, 0f),
        scale = new(1f, 1f, 1f);

    [Range(0.0f, 1.0f)]
    public float amplitude = 1f;
    [Range(0.001f, 100.0f)]
    public float frequency = 1f;

    [Header("Extra Modifiers")]
    public NoiseModifiers modifiers = NoiseModifiers.None;
    [Range(0f, 100f)]
    public float pingPongStrength = 2f;
    public bool usePowerFunction;
    [Range(0f, 10f)]
    public float powAmount = 0f;
    public bool invert = false;

    [Header("Fractal Settings")]
    // Fractal Settings
    [Range(1, 24)]
    public int octaves = 1;
    [Range(0f, 1f)]
    public float persistance = 0.5f;
    [Range(0.01f, 100f)]
    public float lacunarity = 2f;

    [Header("Domain Warp Settings")]
    public bool useDomainWarp = false;
    public NoiseType dWNoiseType = NoiseType.Simplex;
    public float3 dWScale;

    [Range(0f, 100f)]
    public float dWAmplitude;
    [Range(0f, 10f)]
    public float dWFrequency;

    // Domain Warp Fractals
    [Range(1, 8)]
    public int dWOctaves;
    [Range(0f, 1f)]
    public float dWPersistance = 0.5f;
    [Range(0.01f, 100f)]
    public float dWLacunarity = 2f;


    // Structs
    public SNoiseFilter GetAsStruct()
    {
        return new SNoiseFilter(this);
    }
    public SNoiseFilterGPU GetAsGPUStruct()
    {
        return new SNoiseFilterGPU(this);
    }

    public struct SNoiseFilterGPU
    {
        public int seed;

        public int noiseType;
        public int cellularReturnType;
        public float jitter;

        public float3
            offset,
            scale;

        public float amplitude;
        public float frequency;

        public int modifiers;
        public float pingPongStrength;
        public int pow;
        public float powAmount;
        public int invert;
        // Fractal
        public int octaves;
        public float persistance;
        public float lacunarity;

        // Domain Warp
        public int useDomainWarp;
        public int dWNoiseType;
        public float3 dWScale;

        public float dWAmplitude;
        public float dWFrequency;

        public int dWOctaves;
        public float dWPersistance;
        public float dWLacunarity;

        public SNoiseFilterGPU(NoiseFilter filterData)
        {
            this.seed = (int)filterData.seed;
            this.invert = filterData.invert ? 1 : 0;

            this.noiseType = (int)filterData.filterType;
            this.cellularReturnType = (int)filterData.cellReturnType;
            this.jitter = filterData.jitter;

            this.offset = filterData.offset;
            this.scale = filterData.scale;

            this.amplitude = filterData.amplitude;
            this.frequency = filterData.frequency;

            this.modifiers = (int)filterData.modifiers;
            this.pingPongStrength = filterData.pingPongStrength;
            this.pow = filterData.usePowerFunction ? 1 : 0;
            this.powAmount = filterData.powAmount;

            this.octaves = filterData.octaves;
            this.persistance = filterData.persistance;
            this.lacunarity = filterData.lacunarity;
            this.useDomainWarp = filterData.useDomainWarp ? 1 : 0;
            this.dWNoiseType = (int)filterData.dWNoiseType;
            this.dWScale = filterData.dWScale;

            this.dWAmplitude = filterData.dWAmplitude;
            this.dWFrequency = filterData.dWFrequency;

            this.dWOctaves = filterData.dWOctaves;
            this.dWPersistance = filterData.dWPersistance;
            this.dWLacunarity = filterData.dWLacunarity;
        }
    }
    public struct SNoiseFilter
    {
        public uint seed;
        public int filterType;
        public bool invert;
        public float3
            offset,
            scale;

        public float amplitude;
        public float frequency;

        public SNoiseFilter(NoiseFilter filterData)
        {
            this.seed = filterData.seed;
            this.filterType = (int)filterData.filterType;
            this.invert = filterData.invert;
            this.offset = filterData.offset;
            this.scale = filterData.scale * 100f;
            this.amplitude = filterData.amplitude;
            this.frequency = filterData.frequency;
        }
    }
}

public enum NoiseType
{
    Value, Perlin, Simplex, Cell, Wavelet, Sinus
}

public enum CellReturnType
{
    Tyle, Distance1, Distance2, DistDivided
}

public enum NoiseModifiers
{
    None, Ridge, PingPong
}


