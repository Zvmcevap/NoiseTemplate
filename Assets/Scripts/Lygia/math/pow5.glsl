/*
contributors: Patricio Gonzalez Vivo
description: power of 5
use: <float|vec2|vec3|vec4> pow5(<float|vec2|vec3|vec4> v)
*/

#ifndef FNC_POW5
#define FNC_POW5

float pow5(const in float v) {
    float v2 = v * v;
    return v2 * v2 * v;
}

vec2 pow5(const in vec2 v) {
    vec2 v2 = v * v;
    return v2 * v2 * v;
}

vec3 pow5(const in vec3 v) {
    vec3 v2 = v * v;
    return v2 * v2 * v;
}

vec4 pow5(const in vec4 v) {
    vec4 v2 = v * v;
    return v2 * v2 * v;
}

#endif
