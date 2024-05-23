#include "random.hlsl"
#include "srandom.hlsl"
#include "../math/cubic.hlsl"
#include "../math/quintic.hlsl"

/*
contributors: Patricio Gonzalez Vivo
description: gradient Noise 
use: gnoise(<float> x)
*/

#ifndef FNC_GNOISE
#define FNC_GNOISE



float gnoise(float x) {
    float i = floor(x);  // integer
    float f = frac(x);  // fraction
    return lerp(random(i), random(i + 1.0), smoothstep(0.,1.,f)); 
}

float gnoise(float2 st) {
    float2 i = floor(st);
    float2 f = frac(st);
    float a = random(i);
    float b = random(i + float2(1.0, 0.0));
    float c = random(i + float2(0.0, 1.0));
    float d = random(i + float2(1.0, 1.0));
    float2 u = cubic(f);
    return lerp( a, b, u.x) +
                (c - a)* u.y * (1.0 - u.x) +
                (d - b) * u.x * u.y;
}

float gnoise(float3 st)
{
    float3 i = floor(st);
    float3 fi = frac(st);
    float3 u = quintic(fi);
    
    float a = random(i);
    float b = random(i + float3(1.0, 0.0, 0.0));
    float c = random(i + float3(0.0, 1.0, 0.0));
    float d = random(i + float3(1.0, 1.0, 0.0));
    float e = random(i + float3(0.0, 0.0, 1.0));
    float f = random(i + float3(1.0, 0.0, 1.0));
    float g = random(i + float3(0.0, 1.0, 1.0));
    float h = random(i + float3(1.0, 1.0, 1.0));
    
    
    return lerp(
        lerp(
            lerp(a, b, u.x),
            lerp(c, d, u.x),
            u.y
        ),
        lerp(
            lerp(e, f, u.x),
            lerp(g, h, u.x),
            u.y
        ),
        u.z
    );
}


#endif