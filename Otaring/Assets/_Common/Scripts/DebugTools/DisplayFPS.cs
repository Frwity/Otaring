using UnityEngine;

namespace Com.RandomDudes.Debug
{
    public class DisplayFPS : MonoBehaviour
    {
        private float deltaTime = 0.0f;

        private void Update()
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        }

        private void OnGUI()
        {
            int width = Screen.width, height = Screen.height;

            GUIStyle style = new GUIStyle();
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = height * 2 / 100;
            style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);

            float fps = 1.0f / deltaTime;
            float ms = deltaTime * 1000.0f;
            string text = string.Format("{0:0.0} ms ({1:0.} fps)", ms, fps);

            Rect rect = new Rect(0, 0, width, height * 2 / 100);
            GUI.Label(rect, text, style);
        }
    }
}