using System;
using UnityEditor;
using UnityEngine;

[Serializable]
public class TileComponent : MonoBehaviour
{
    public Vector2 xAs = new();
    public Vector2 yAs = new();
    public Vector2 zAs = new();
}

[Serializable]
public class MultiTileComponent : MonoBehaviour {
    public Vector2Int size = new();

    public TileComponent[] tileComponents;
}

[CustomPropertyDrawer(typeof(MultiTileComponent))]
public class MultiTileComponentDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        var amountRect = new Rect(position.x, position.y, 30, position.height);
        var unitRect = new Rect(position.x + 35, position.y, 50, position.height);
        var nameRect = new Rect(position.x + 90, position.y, position.width - 90, position.height);

        // Draw fields - pass GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("amount"), GUIContent.none);
        EditorGUI.PropertyField(unitRect, property.FindPropertyRelative("unit"), GUIContent.none);
        EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("name"), GUIContent.none);

        var tmp = property.FindPropertyRelative("size").vector2IntValue;
        int amount = tmp.x * tmp.y;

        SerializedProperty listProperty = property.FindPropertyRelative("tileComponents");
        if (listProperty.CountInProperty() == amount)
            listProperty
        
        foreach (var item in listProperty) {

        }

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }

    public void DrawTileComponents(Rect position, SerializedProperty property) {

    }
}