using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[BurstCompile(CompileSynchronously = true)]
public struct ArrayToTextureJob: IJobFor
{
    [WriteOnly]
    public NativeArray<Color> colors;

    [ReadOnly]
    public NativeArray<float> noiseArray;

    void IJobFor.Execute(int index)
    {
        float noiseValue = noiseArray[index];
        colors[index] = new Color(noiseValue, noiseValue, noiseValue);
    }
}
