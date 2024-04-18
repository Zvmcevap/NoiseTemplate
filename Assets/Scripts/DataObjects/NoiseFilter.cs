using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
public class NoiseFilter
{
    public uint seed = 0;
    public NoiseFilterType filterType = NoiseFilterType.Simplex;
    public bool invert = false;
    public bool autoOffset;
    public float3 autoOffsetAmount;

    public float3
        offset = new float3(0f, 0f, 0f),
        scale = new float3(1f, 1f, 1f),
        repeatPeriod = new float3(1f, 1f, 1f);

    [Range(0.0f, 1.0f)]
    public float amplitude = 1f;
    [Range(0.001f, 100.0f)]
    public float frequency = 1f;

    public SNoiseFilter GetAsStruct()
    {
        return new SNoiseFilter(this);
    }
    public SNoiseFilterGPU GetAsGPUStruct()
    {
        return new SNoiseFilterGPU(this);
    }


    public struct SNoiseFilter
    {
        public uint seed;
        public int filterType;
        public bool invert;
        public float3
            offset,
            scale,
            period;

        public float amplitude;
        public float frequency;

        public SNoiseFilter(NoiseFilter filterData)
        {
            this.seed = filterData.seed;
            this.filterType = (int)filterData.filterType;
            this.invert = filterData.invert;
            this.offset = filterData.offset;
            this.scale = filterData.scale;
            this.period = filterData.repeatPeriod;
            this.amplitude = filterData.amplitude;
            this.frequency = filterData.frequency;
        }
    }
    public struct SNoiseFilterGPU
    {
        public int seed;
        public int filterType;
        public int invert;
        public float3
            offset,
            scale,
            period;

        public float amplitude;
        public float frequency;

        public SNoiseFilterGPU(NoiseFilter filterData)
        {
            this.seed = (int)filterData.seed;
            this.filterType = (int)filterData.filterType;
            this.invert = filterData.invert ? 1 : 0;
            this.offset = filterData.offset;
            this.scale = filterData.scale / 100f;
            this.period = filterData.repeatPeriod;
            this.amplitude = filterData.amplitude;
            this.frequency = filterData.frequency;
        }
    }

}
[System.Serializable]
public enum NoiseFilterType
{
    Perlin,
    PerlinPeriodic,
    Simplex,
    WorleyF1,
    WorleyF2
}