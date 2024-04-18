using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

[CustomEditor(typeof(SettingsManager), true)]
public class SettingsManagerEditor: Editor
{
    [OnOpenAsset()]
    public static bool OpenEditor(int instanceId, int line)
    {
        SettingsManager settingsManager = EditorUtility.InstanceIDToObject(instanceId) as SettingsManager;
        if (settingsManager != null)
        {
            SettingsEditorWindow.Open(settingsManager);
        }
        return false;
    }
    public override void OnInspectorGUI()
    {
        SettingsManager data = (SettingsManager)target;

        if (GUILayout.Button("Open Editor"))
        {
            SettingsEditorWindow.Open((SettingsManager)target);
        }

        base.OnInspectorGUI();

        if (GUILayout.Button("Save"))
        {
            ((SettingsManager)target).Save();
        }
        if (GUILayout.Button("Load"))
        {
            ((SettingsManager)target).Load();
        }
        if (!data.autoUpdate)
        {
            if (GUILayout.Button("Update"))
            {
                data.UpdateValues();
            }
        }
    }
}
