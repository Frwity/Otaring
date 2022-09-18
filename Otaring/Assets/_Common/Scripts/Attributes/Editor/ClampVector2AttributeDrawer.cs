using UnityEditor;
using UnityEngine;

namespace Com.RandomDudes.Attributes
{
    [CustomPropertyDrawer(typeof(ClampVector2Attribute))]
    public class ClampVector2AttributeDrawer : PropertyDrawer
    {
        ClampVector2Attribute RangeAttribute { get { return (ClampVector2Attribute)attribute; } }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Color previous = GUI.color;
            GUI.color = !IsValid(property) ? Color.red : Color.white;

            Rect textFieldPosition = position;
            textFieldPosition.width = position.width;
            textFieldPosition.height = position.height;

            EditorGUI.BeginChangeCheck();
            Vector2 value = EditorGUI.Vector2Field(textFieldPosition, label, property.vector2Value);

            if (EditorGUI.EndChangeCheck())
            {
                if (RangeAttribute.isClamped)
                {
                    value.x = Mathf.Clamp(value.x, RangeAttribute.minX, RangeAttribute.maxX);
                    value.y = Mathf.Clamp(value.y, RangeAttribute.minY, RangeAttribute.maxY);
                }

                property.vector2Value = value;
            }

            Rect helpPosition = position;
            helpPosition.y += 16;
            helpPosition.height = 16;

            DrawHelpBox(helpPosition, property);

            GUI.color = previous;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!IsValid(property))
                return 32;

            return base.GetPropertyHeight(property, label);
        }

        private void DrawHelpBox(Rect position, SerializedProperty prop)
        {
            if (IsValid(prop))
                return;

            EditorGUI.HelpBox(position, string.Format("Invalid Range X [{0}]-[{1}] Y [{2}]-[{3}]", RangeAttribute.minX, RangeAttribute.maxX, RangeAttribute.minY, RangeAttribute.maxY), MessageType.Error);
        }

        private bool IsValid(SerializedProperty prop)
        {
            Vector2 vector = prop.vector2Value;
            return vector.x >= RangeAttribute.minX && vector.x <= RangeAttribute.maxX && vector.y >= RangeAttribute.minY && vector.y <= RangeAttribute.maxY;
        }
    }
}