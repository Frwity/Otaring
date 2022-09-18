using UnityEditor;
using UnityEngine;

namespace Com.RandomDudes.Attributes
{
    [CustomPropertyDrawer(typeof(ClampVector3Attribute))]
    public class ClampVector3AttributeDrawer : PropertyDrawer
    {
        ClampVector3Attribute RangeAttribute { get { return (ClampVector3Attribute)attribute; } }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Color previous = GUI.color;

            GUI.color = !IsValid(property) ? Color.red : Color.white;

            Rect textFieldPosition = position;
            textFieldPosition.width = position.width;
            textFieldPosition.height = position.height;

            EditorGUI.BeginChangeCheck();
            Vector3 value = EditorGUI.Vector3Field(textFieldPosition, label, property.vector3Value);

            if (EditorGUI.EndChangeCheck())
            {
                if (RangeAttribute.isClamped)
                {
                    value.x = Mathf.Clamp(value.x, RangeAttribute.minX, RangeAttribute.maxX);
                    value.y = Mathf.Clamp(value.y, RangeAttribute.minY, RangeAttribute.maxY);
                    value.z = Mathf.Clamp(value.z, RangeAttribute.minZ, RangeAttribute.maxZ);
                }

                property.vector3Value = value;
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
        private void DrawHelpBox(Rect position, SerializedProperty property)
        {
            if (IsValid(property))
                return;

            EditorGUI.HelpBox(position, string.Format("Invalid Range X [{0}]-[{1}] Y [{2}]-[{3}]", RangeAttribute.minX, RangeAttribute.maxX, RangeAttribute.minY, RangeAttribute.maxY, RangeAttribute.minZ, RangeAttribute.maxZ), MessageType.Error);
        }

        private bool IsValid(SerializedProperty property)
        {
            Vector3 vector = property.vector3Value;

            return vector.x >= RangeAttribute.minX && vector.x <= RangeAttribute.maxX && vector.y >= RangeAttribute.minY && vector.y <= RangeAttribute.maxY && vector.z >= RangeAttribute.minZ && vector.z <= RangeAttribute.maxZ;
        }
    }
}