using System;
using UnityEngine;

[System.Serializable]
public class UpdatableData: ScriptableObject
{
    [SerializeField]
    public event Action OnValuesUpdated;
    public bool autoUpdate;

    private void OnValidate()
    {
        if (autoUpdate)
        {
            NotifyOfUpdatedValues();
        }
    }

    public void NotifyOfUpdatedValues()
    {
        if (OnValuesUpdated != null)
        {
            OnValuesUpdated();
        }
    }
}
