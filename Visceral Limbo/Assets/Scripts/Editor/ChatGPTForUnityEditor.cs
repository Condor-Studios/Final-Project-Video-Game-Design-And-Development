using UnityEditor;
using UnityEngine;

namespace AI.Utilities.Editor
{
    [CustomEditor(typeof(ChatGptForUnity))]
    public class ChatGptForUnityEditor : UnityEditor.Editor
    {
        private ChatGptForUnity chatGpt;
        private string[] modelOptions;

        private void OnEnable()
        {
            chatGpt = (ChatGptForUnity)target;
            modelOptions = ChatGptModels.Models; // Traemos todos los modelos disponibles
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDefaultInspector();

            GUILayout.Space(10);

            EditorGUILayout.LabelField("ChatGPT Settings", EditorStyles.boldLabel);
            
            if (GUI.changed)
            {
                chatGpt.UpdateEstimatedCost();
                EditorUtility.SetDirty(chatGpt);
            }

            // Modelo Dropdown
            GUILayout.Label("Select Model", EditorStyles.label);
            int newSelectedIndex = EditorGUILayout.Popup(chatGpt.selectedModelIndex, modelOptions);

            if (newSelectedIndex != chatGpt.selectedModelIndex)
            {
                Undo.RecordObject(chatGpt, "Change Selected Model");
                chatGpt.selectedModelIndex = newSelectedIndex;
                chatGpt.model = modelOptions[newSelectedIndex];
                EditorUtility.SetDirty(chatGpt);
            }
            
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Context Target Settings", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");

            chatGpt.contextTarget = (GameObject)EditorGUILayout.ObjectField("Context Target", chatGpt.contextTarget, typeof(GameObject), true);
            chatGpt.includeDependencies = EditorGUILayout.Toggle("Include Dependencies", chatGpt.includeDependencies);
            chatGpt.useContext = EditorGUILayout.Toggle("Use Context", chatGpt.useContext);

            if (chatGpt.useContext)
            {
                chatGpt.showContextPreview = EditorGUILayout.Foldout(chatGpt.showContextPreview, "Context Preview");
                if (chatGpt.showContextPreview)
                {
                    EditorGUILayout.LabelField("", GUILayout.Height(5));
                    EditorGUILayout.TextArea(chatGpt.GeneratedContextPreview, GUILayout.MinHeight(100), GUILayout.ExpandHeight(true));
                }

                if (GUILayout.Button("Regenerate Context"))
                {
                    chatGpt.GenerateContextManually();
                }
            }

            EditorGUILayout.EndVertical();

            GUILayout.Space(10);

            EditorGUILayout.LabelField("ChatGPT Actions", EditorStyles.boldLabel);

            if (GUILayout.Button("Send Prompt"))
            {
                if (!string.IsNullOrEmpty(chatGpt.prompt))
                {
                    chatGpt.SendRequest();
                }
                else
                {
                    EditorUtility.DisplayDialog("Warning", "Prompt is empty. Please enter a prompt first.", "OK");
                }
            }

            if (GUILayout.Button("Clear Prompt and Response"))
            {
                chatGpt.Clear();
            }

            if (GUILayout.Button("Guardar como Script"))
            {
                if (!string.IsNullOrEmpty(chatGpt.ResponseContent))
                {
                    chatGpt.SaveScript();
                    EditorUtility.DisplayDialog("Script Saved", "The script was successfully saved to the Scripts folder.", "OK");
                }
                else
                {
                    EditorUtility.DisplayDialog("Warning", "No response available to save as script.", "OK");
                }
            }

            GUILayout.Space(10);

            EditorGUILayout.HelpBox("• Select Model: Choose which model to send prompts to.\n" +
                                    "• Send Prompt: Sends your input to ChatGPT.\n" +
                                    "• Clear: Clears the prompt and response fields.\n" +
                                    "• Save as Script: Saves the response as a .cs script if it contains valid C# code.\n" +
                                    "• You can cancel an ongoing request from the progress bar.", MessageType.Info);

            serializedObject.ApplyModifiedProperties();
        }
    }
}