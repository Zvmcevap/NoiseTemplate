#include "random.hlsl"
#include "../math/rotate2d.hlsl"

/*
contributors: Martijn Steinrucken
description: Wavelet noise https://www.shadertoy.com/view/wsBfzK
use: <float> wavelet(<vec2|vec3> pos)
examples:
    - /shaders/generative_wavelet.frag
license:
    - The MIT License Copyright 2020 Martijn Steinrucken
*/

#ifndef FNC_WAVELET
#define FNC_WAVELET

float wavelet(float2 p, float phase, float k)
{
    float d = 0.0, s = 1.0, m = 0.0, a = 0.0;
    for (float i = 0.0; i < 4.0; i++)
    {
        float2 q = p * s;
        a = random(floor(q)) * 1e3;
#ifdef WAVELET_VORTICITY
        a += phase * random(floor(q)) * WAVELET_VORTICITY;
#endif
        q = mul(transpose(rotate2d(a)), (frac(q) - 0.5));
        d += sin(q.x * 10.0 + phase) * smoothstep(.25, 0.0, dot(q, q)) / s;
        p = mul(float2x2(0.54, 0.84, -0.84, 0.54), p) + i;
        m += 1.0 / s;
        s *= k;
    }
    return d / m;
}

float wavelet(float3 p, float k)
{
    return wavelet(p.xy, p.z, k);
}

float wavelet(float3 p)
{
    return wavelet(p, 1.24);
}

float wavelet(float2 p, float phase)
{
    return wavelet(p, phase, 1.24);
}

float wavelet(float2 p)
{
    return wavelet(p, 0.0, 1.24);
}

#endif