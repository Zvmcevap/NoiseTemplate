using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
public class NoiseData
{
    // Basic Data
    public string name;
    public Dimensions dimensions;
    [Range(1, 4096 * 4)]
    public int resolution;
    public bool normalize01;
    public bool invert;
    public bool AutoOffset;
    [Range(0, 1000f)]
    public float autoOffsetAmount;
    public float3 autoOffsetVector;

    // Octave specific data
    [Range(0f, 1f)]
    public float persistence;
    [Range(1f, 10f)]
    public float lacunarity;

    public List<NoiseFilter> octaveNoiseFilters;

    public SNoiseData GetAsStruct()
    {
        return new SNoiseData(this);
    }

    public SNoiseDataGPU GetAsGPUStruct()
    {
        return new SNoiseDataGPU(this);
    }

    public struct SNoiseData
    {
        // Basic Data
        public int dimensions;
        public int resolution;
        public bool normalize01;
        public bool invert;

        // Octave specific data
        public float persistence;
        public float lacunarity;

        public SNoiseData(NoiseData noiseData)
        {
            this.dimensions = (int)noiseData.dimensions;
            this.resolution = noiseData.resolution;
            this.normalize01 = noiseData.normalize01;
            this.invert = noiseData.invert;
            this.persistence = noiseData.persistence;
            this.lacunarity = noiseData.lacunarity;
        }
    }
    public struct SNoiseDataGPU
    {
        public int dimensions;
        public int resolution;
        public int normalize01;
        public int invert;

        public float persistence;
        public float lacunarity;

        public SNoiseDataGPU(NoiseData noiseData)
        {
            this.dimensions = (int)noiseData.dimensions;
            this.resolution = noiseData.resolution;
            this.normalize01 = noiseData.normalize01 ? 1 : 0;
            this.invert = noiseData.invert ? 1 : 0;
            this.persistence = noiseData.persistence;
            this.lacunarity = noiseData.lacunarity;
        }
    }

    public enum Dimensions
    {
        TWO = 0,
        THREE = 1
    };

}
