using UnityEditor;

public class SettingsEditorWindow : ExtendedEditorWindow
{
    public static void Open(SettingsManager dataObject)
    {
        SettingsEditorWindow window = GetWindow<SettingsEditorWindow>("Noise Data Editor");
        window.serializedObject = new SerializedObject(dataObject);
    }

    private void OnGUI()
    {
        if (serializedObject != null)
        {
            // Update the serialized object
            serializedObject.Update();

            // Iterate through all serialized properties
            SerializedProperty property = serializedObject.GetIterator();
            property.Next(true);
            while (property.NextVisible(false))
            {
                EditorGUILayout.PropertyField(property, true);
            }

            // Apply modifications to the serialized object
            serializedObject.ApplyModifiedProperties();
        }
    }
}

