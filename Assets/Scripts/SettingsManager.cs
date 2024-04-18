using UnityEngine;

public abstract class SettingsManager: MonoBehaviour
{
    [SerializeField]
    public bool autoUpdate;

    public abstract void Save();
    public abstract void Load();

    public abstract void UpdateValues();
}
