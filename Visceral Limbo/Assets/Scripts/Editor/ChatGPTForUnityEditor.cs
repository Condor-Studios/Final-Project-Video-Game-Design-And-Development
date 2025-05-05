using AI.Utilities;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(ChatGptForUnity))]
    public class ChatGptForUnityEditor: UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var chatGpt = (ChatGptForUnity)target;
        
            GUILayout.Space(15);

            if (GUILayout.Button("Ask"))
            {
                chatGpt.SendRequest();
            }
        
            GUILayout.Space(15);
        
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save Script"))
            {
                chatGpt.SaveScript();
            }

            if (GUILayout.Button("Clear"))
            {
                chatGpt.Clear();
            }
            GUILayout.EndHorizontal();
        }
    }
}
