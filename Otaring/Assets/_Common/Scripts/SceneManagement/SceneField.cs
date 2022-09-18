using UnityEngine;
using UnityEditor;

namespace Com.RandomDudes.SceneManagement
{
    [System.Serializable]
    public class SceneField
    {
#pragma warning disable CS0414
        [SerializeField] private Object sceneAsset = default;
        [SerializeField] private string sceneName = "";
#pragma warning restore CS0414

        public string SceneName { get { return sceneName; } }

        public static implicit operator string(SceneField sceneField)
        {
            return sceneField.SceneName;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(SceneField))]
    public class SceneFieldPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, GUIContent.none, property);

            SerializedProperty sceneAsset = property.FindPropertyRelative("sceneAsset");
            SerializedProperty sceneName = property.FindPropertyRelative("sceneName");

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            if (sceneAsset != null)
            {
                sceneAsset.objectReferenceValue = EditorGUI.ObjectField(position, sceneAsset.objectReferenceValue, typeof(SceneAsset), false);

                if (sceneAsset.objectReferenceValue != null)
                    sceneName.stringValue = ((SceneAsset)sceneAsset.objectReferenceValue).name;
            }

            EditorGUI.EndProperty();
        }
    }
#endif
}