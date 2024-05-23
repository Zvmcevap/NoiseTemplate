#ifndef NOISE_DATA
#define NOISE_DATA
struct NoiseData
{
        // Basic Data
    int dimensions;
    int resolution;
    int normalize01;
    int invert;
    float persistence;
    float lacunarity;
};
#endif

#ifndef NOISE_FILTER
#define NOISE_FILTER
struct NoiseFilter
{
    int seed;
        
    int noiseType;
    int cellularReturnType; // Tiles, Dist 1, Dist 2, Dis1/Dist2
    float jitter;

    float3 offset,
            scale;

    float amplitude;
    float frequency;
    
    int modifiers; // Nada Ridge Pingpong
    float pingPongStrength;
    int pow;
    float powAmount;
    int invert;
    
        // Fractal
    int octaves;
    float persistance;
    float lacunarity;
    
    // Domain Warp
    int useDomainWarp;
    int dWNoiseType;
    float3 dWScale;

    float dWAmplitude;
    float dWFrequency;

    int dWOctaves;
    float dWPersistance;
    float dWLacunarity;
};
#endif