using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
public class NoiseData
{
    // Basic Data
    public string name;
    [Range(1, 4096)]
    public int resolution;
    public bool normalize01;
    public bool invert;
    public bool AutoOffset;
    public float3 autoOffsetAmount;
    [Min(1)]
    public uint seed;

    // Octave specific data
    public bool separateOctaveSeeds;
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
        public int resolution;
        public bool normalize01;
        public bool invert;
        public uint seed;

        // Octave specific data
        public bool separateOctaveSeeds;
        public float persistence;
        public float lacunarity;

        public SNoiseData(NoiseData noiseData)
        {
            this.resolution = noiseData.resolution;
            this.normalize01 = noiseData.normalize01;
            this.invert = noiseData.invert;
            this.seed = noiseData.seed;
            this.separateOctaveSeeds = noiseData.separateOctaveSeeds;
            this.persistence = noiseData.persistence;
            this.lacunarity = noiseData.lacunarity;
        }
    }
    public struct SNoiseDataGPU
    {
        public int resolution;
        public int normalize01;
        public int invert;
        public int seed;
        public int separateOctaveSeeds;

        public float persistence;
        public float lacunarity;

        public SNoiseDataGPU(NoiseData noiseData)
        {
            this.resolution = noiseData.resolution;
            this.normalize01 = noiseData.normalize01 ? 1 : 0;
            this.invert = noiseData.invert ? 1 : 0;
            this.seed = (int)noiseData.seed;
            this.separateOctaveSeeds = noiseData.separateOctaveSeeds ? 1 : 0;
            this.persistence = noiseData.persistence;
            this.lacunarity = noiseData.lacunarity;
        }
    }

}
