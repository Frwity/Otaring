using Com.RandomDudes.Debug;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Based on: https://forum.unity.com/threads/draw-a-field-only-if-a-condition-is-met.448855/
/// Rework by Chakib Benssoum
/// </summary>

namespace Com.RandomDudes.Attributes
{
    [CustomPropertyDrawer(typeof(DrawIfAttribute))]
    public class DrawIfPropertyDrawer : PropertyDrawer
    {
        private DrawIfAttribute drawIfAttributd;
        private SerializedProperty comparedField;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!ShowMe(property) && drawIfAttributd.DisablingType == DrawIfAttribute.DisablingTypes.DontDraw)
                return 0f;

            return base.GetPropertyHeight(property, label);
        }

        private bool ShowMe(SerializedProperty property)
        {
            drawIfAttributd = (DrawIfAttribute)attribute;

            bool result = true;
            string path;
            string comparedPropertyName;
            object comparedValue;

            for (int i = 0; i < drawIfAttributd.ComparedPropertyNames.Length; i++)
            {
                comparedPropertyName = drawIfAttributd.ComparedPropertyNames[i];
                comparedValue = drawIfAttributd.ComparedValues[i];

                path = property.propertyPath.Contains(".") ? System.IO.Path.ChangeExtension(property.propertyPath, comparedPropertyName) : comparedPropertyName;

                comparedField = property.serializedObject.FindProperty(path);

                if (comparedField == null)
                {
                    DevLog.Error("Cannot find property with name: " + path);
                    continue;
                }

                switch (comparedField.type)
                {
                    case "int":
                        result = comparedField.intValue.Equals(comparedValue) && result;
                        break;
                    case "float":
                        result = comparedField.floatValue.Equals(comparedValue) && result;
                        break;
                    case "string":
                        result = comparedField.stringValue.Equals(comparedValue) && result;
                        break;
                    case "bool":
                        result = comparedField.boolValue.Equals(comparedValue) && result;
                        break;
                    case "Enum":
                        result = comparedField.enumValueIndex.Equals((int)comparedValue) && result;
                        break;
                    default:
                        DevLog.Error("Error: " + comparedField.type + " is not supported, path: " + path);
                        break;
                }
            }

            return result;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (ShowMe(property))
            {
                EditorGUI.PropertyField(position, property);
            }
            else if (drawIfAttributd.DisablingType == DrawIfAttribute.DisablingTypes.ReadOnly)
            {
                GUI.enabled = false;

                EditorGUI.PropertyField(position, property);

                GUI.enabled = true;
            }
        }
    }
}
