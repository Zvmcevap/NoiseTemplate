using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomPropertyDrawer(typeof(List<>), true)] // Specify List<> as the target type
public class ListDrawer : PropertyDrawer
{
    private ReorderableList list;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (list == null)
        {
            list = new ReorderableList(property.serializedObject, property, true, true, true, true);
            list.drawHeaderCallback += rect => EditorGUI.LabelField(rect, label);
            list.drawElementCallback += (rect, index, isActive, isFocused) =>
            {
                var elementProperty = list.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, elementProperty, GUIContent.none);
            };
        }

        list.DoList(position);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return list != null ? list.GetHeight() : EditorGUIUtility.singleLineHeight;
    }
}
