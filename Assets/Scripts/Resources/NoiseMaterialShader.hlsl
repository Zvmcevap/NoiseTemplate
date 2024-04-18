

#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
	StructuredBuffer<float> noiseResults;
#endif


void ShowNoise( float idk , out float3 color)
{
#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
		float noiseVal = noiseResults[unity_InstanceID];
		color = float3(noiseVal, noiseVal, noiseVal);
#endif
}