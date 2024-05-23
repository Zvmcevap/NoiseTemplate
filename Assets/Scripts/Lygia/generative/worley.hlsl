#include "random.hlsl"

/*
contributors: Patricio Gonzalez Vivo
description: Worley noise
use: <float2> worley(<float2|float3> pos)
*/

#ifndef FNC_WORLEY
#define FNC_WORLEY

float3 worley(float2 p, float jitter){
    float2 n = floor( p );
    float2 f = frac( p );

    float3 result = float3(1., 1., 0.); // Distance 1, Distance 2, Cell value dist 1
    for( int j= -1; j <= 1; j++ )
        for( int i=-1; i <= 1; i++ ) {	
                float2 g = float2(i, j);
                float2  o = random2( n + g ) * jitter;
                float2  delta = g + o - f;
                float d = length(delta);
                if (d < result.x)
                {
                    result.y = result.x;
                    result.x = d;
                    result.z = o.x;
                }
                else if (d < result.y)
                {
                    result.y = d;
                }
            }

    return result;
}


float3 worley(float3 p, float jitter){
    float3 n = floor( p );
    float3 f = frac( p );

    float3 result = float3(3., 3., 0.); // Distance 1, Distance 2, Cell value dist 1

    for( int k = -1; k <= 1; k++ )
        for( int j= -1; j <= 1; j++ )
            for( int i=-1; i <= 1; i++ ) {	
                float3  g = float3(i,j,k);
                float3  o = random3( n + g ) * jitter;
                float3  delta = g+o-f;
                float d = length(delta);
                if (d < result.x)
                {
                    result.y = result.x;
                    result.x = d;
                    result.z = o.x;
                }
                else if (d < result.y)
                {
                    result.y = d;
                }
            }

    return result;
}
#endif