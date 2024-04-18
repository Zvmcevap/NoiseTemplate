using UnityEditor;
using UnityEngine;

public class ExtendedEditorWindow : EditorWindow
{
    protected SerializedObject serializedObject;
    protected SerializedProperty currentProperty;

    protected void DrawProperties(SerializedProperty property, bool drawChildren)
    {
        string lastPropPath = string.Empty;
        foreach (SerializedProperty p in property)
        {
            if (p.isArray && p.propertyType == SerializedPropertyType.Generic)
            {
                EditorGUILayout.BeginHorizontal();
                p.isExpanded = EditorGUILayout.Foldout(p.isExpanded, p.displayName);
                EditorGUILayout.EndHorizontal();

                if (p.isExpanded)
                {
                    EditorGUI.indentLevel++;
                    DrawProperties(p, drawChildren);
                    EditorGUI.indentLevel--;
                }
            }
            else if (p.objectReferenceValue != null && p.objectReferenceValue.GetType().IsSubclassOf(typeof(ScriptableObject)))
            {
                SerializedObject scriptableObjectSerializedObject = new SerializedObject(p.objectReferenceValue);
                scriptableObjectSerializedObject.Update();
                EditorGUILayout.LabelField(p.displayName, EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                DrawProperties(scriptableObjectSerializedObject.GetIterator(), drawChildren);
                EditorGUI.indentLevel--;
                scriptableObjectSerializedObject.ApplyModifiedProperties();
            }
            else
            {
                if (!string.IsNullOrEmpty(lastPropPath) && p.propertyPath.Contains(lastPropPath)) { continue; }
                lastPropPath = p.propertyPath;
                EditorGUILayout.PropertyField(p, drawChildren);
            }
        }
    }
}
