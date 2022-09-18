#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using Com.RandomDudes.Debug;
using System;

namespace Com.RandomDudes.Events
{
    public partial class ActionEvent
    {
        public UnityEvent Interface;

        public void TryToAddListener(Action listener)
        {
            if (!IsActionAlreadyRegistered(listener))
                Action += listener;
        }

        private bool IsActionAlreadyRegistered(Action prospectiveHandler)
        {
            if (Action != null)
            {
                foreach (Action existingHandler in Action.GetInvocationList())
                {
                    if ((existingHandler == prospectiveHandler))
                        return true;
                }
            }

            return false;
        }
    }

    [CustomPropertyDrawer(typeof(ActionEvent))]
    public class ActionEventCustomEditor : PropertyDrawer
    {
        private static List<Action> oldAction = new List<Action>();

        public override float GetPropertyHeight(SerializedProperty pProperty, GUIContent label)
        {
            ActionEvent lEvent = fieldInfo.GetValue(pProperty.serializedObject.targetObject) as ActionEvent;

            return 130 + (Mathf.Clamp(lEvent.Interface.GetPersistentEventCount() - 1, 0, Mathf.Infinity)) * 47;
        }

        public static bool Button(GUIContent content, Rect position, GUIStyle style = null)
        {
            Rect rect = EditorGUI.IndentedRect(position);
            rect.x -= 12;

            return style == null ? GUI.Button(rect, content) : GUI.Button(rect, content, style);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property == null)
                return;

            ActionEvent actionEvent = fieldInfo.GetValue(property.serializedObject.targetObject) as ActionEvent;

            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.LabelField(new Rect(position.x, position.y, position.width, 18), label.text + " (ActionEvent): ");

            EditorGUI.indentLevel++;

            if (Button(new GUIContent("Invoke " + GetNameInParent(property)), new Rect(position.x, position.y + 20, position.width, 18)))
                actionEvent.Invoke();

            Rect interfaceRect = EditorGUI.IndentedRect(new Rect(position.x, position.y + 42, position.width, 18));
            interfaceRect.x -= 12;

            EditorGUI.PropertyField(interfaceRect, property.FindPropertyRelative("Interface"));

            for (int j = 0; j < oldAction.Count; j++)
                actionEvent.RemoveListener(oldAction[j]);

            oldAction = new List<Action>();

            UnityEvent eventInterface = actionEvent.Interface;

            for (int i = 0; i < eventInterface.GetPersistentEventCount(); i++)
            {
                UnityEngine.Object target = eventInterface.GetPersistentTarget(i);

                if (target == null)
                    continue;

                MethodInfo method = target.GetType().GetMethod(eventInterface.GetPersistentMethodName(i));

                if (method == null)
                    continue;

                Action action = new Action(() =>
                {
                    try
                    {
                        method.Invoke(Convert.ChangeType(target, target.GetType()), new object[] { });
                    }
                    catch (Exception)
                    {
                        DevLog.Warning("Trying to use method with wrong parameters type on ActionEvent: " + GetNameInParent(property));
                    }
                });

                oldAction.Add(action);
                actionEvent.TryToAddListener(action);
            }

            EditorGUI.indentLevel--;

            EditorGUI.EndProperty();
        }

        protected string GetNameInParent(SerializedProperty property)
        {
            object parent = GetParent(property);
            ActionEvent @event = fieldInfo.GetValue(property.serializedObject.targetObject) as ActionEvent;

            foreach (FieldInfo fieldInfo in parent.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
            {
                if (fieldInfo.GetValue(parent) == @event)
                    return fieldInfo.Name;
            }

            return "Unnamed Event";
        }

        protected object GetParent(SerializedProperty property)
        {
            string path = property.propertyPath.Replace(".Array.data[", "[");
            object target = property.serializedObject.targetObject;
            string[] elements = path.Split('.');

            foreach (string element in elements.Take(elements.Length - 1))
            {
                if (element.Contains("["))
                {
                    string elementName = element.Substring(0, element.IndexOf("["));
                    int index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));

                    target = GetValue(target, elementName, index);
                }
                else
                    target = GetValue(target, element);
            }

            return target;
        }

        protected object GetValue(object source, string name)
        {
            if (source == null)
                return null;

            Type type = source.GetType();
            FieldInfo field = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            if (field == null)
            {
                PropertyInfo property = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                if (property == null)
                    return null;
                return property.GetValue(source, null);
            }

            return field.GetValue(source);
        }

        protected object GetValue(object pSource, string pName, int index)
        {
            IEnumerable enumerable = GetValue(pSource, pName) as IEnumerable;
            IEnumerator enumerator = enumerable.GetEnumerator();

            while (index-- >= 0)
                enumerator.MoveNext();

            return enumerator.Current;
        }
    }
}

#endif