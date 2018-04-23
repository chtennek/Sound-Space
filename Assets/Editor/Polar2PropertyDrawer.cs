using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Polar2), true)]
[CustomPropertyDrawer(typeof(Cylindrical3), true)]
public class Polar2PropertyDrawer : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Use smaller label width
        float labelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = GUI.skin.label.CalcSize(new GUIContent("X")).x;

        string[] fields = new string[] { "r", "θ", "z" };
        string[] labels = new string[] { "R", "θ", "Z" };
        float fieldWidth = position.width / 3;

        for (int i = 0; i < fields.Length; i++)
        {
            SerializedProperty field = property.FindPropertyRelative(fields[i]);
            if (field == null)
                continue;

            Rect fieldPosition = new Rect(position.x + i * fieldWidth, position.y, fieldWidth, position.height);
            EditorGUI.PropertyField(fieldPosition, field, new GUIContent(labels[i]));
        }

        // Reset values
        EditorGUI.indentLevel = indent;
        EditorGUIUtility.labelWidth = labelWidth;
        EditorGUI.EndProperty();
    }
}