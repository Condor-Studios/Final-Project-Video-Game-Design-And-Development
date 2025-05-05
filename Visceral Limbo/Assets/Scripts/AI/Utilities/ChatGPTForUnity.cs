using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using Newtonsoft.Json;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace AI.Utilities
{
     public class ChatGptForUnity : MonoBehaviour
        {
            public enum MaxTokensOptions
            {
                Max128 = 128,
                Max256 = 256,
                Max512 = 512,
                Max1024 = 1024,
                Max2048 = 2048
            }

            [Header("Debug Settings")]
            [SerializeField] private bool debug = true;

            [FormerlySerializedAs("APIKey")]
            [Header("Configuration")]
            [SerializeField] private string apiKey;
            [HideInInspector]
            public int selectedModelIndex = 19;
            [HideInInspector]
            public string model = ChatGptModels.Models[0];
            
            [Header("Context Settings")]
            [SerializeField]
            public bool useContext = true;
            [SerializeField] public GameObject contextTarget;
            [SerializeField] public bool includeDependencies = true;
            [TextArea(5, 20)]
            [SerializeField, HideInInspector]
            private string generatedContextPreview = "";
            public bool showContextPreview = false;
            
            [Header("Token Settings")]
            [SerializeField] private MaxTokensOptions maxTokens = MaxTokensOptions.Max512;
            [SerializeField] private float estimatedPromptCost = 0f;
            private float tokenPricePerThousand;

            private Dictionary<int, float> modelTokenPrices;

            private const int AverageCharsPerToken = 4; // OpenAI standard estimation
            
            [TextArea(3, 10)]
            [SerializeField] public string prompt;

            [Header("Response")]
            [TextArea(3, 40)]
            [SerializeField] private string response;
            
            
            
            public string ResponseContent => response;

            private const string ChatGpturl = "https://api.openai.com/v1/chat/completions";
            [FormerlySerializedAs("_scriptsFolder")] [SerializeField] private string scriptsFolder = "Assets/Scripts/AI";
            private const string Directory = "ChatGPT";

            private RequestWrapper _request;
            private ResponseWrapper _responseBody;

            private void Awake()
            {
                InitializeModelTokenPrices();
                if (modelTokenPrices != null && modelTokenPrices.ContainsKey(selectedModelIndex))
                {
                    tokenPricePerThousand = modelTokenPrices[selectedModelIndex];
                }
                else
                {
                    tokenPricePerThousand = 0.0015f; // default fallback
                }
            }

            public string GeneratedContextPreview => generatedContextPreview;
            public bool UseContext => useContext;

            public void GenerateContextManually()
            {
                GenerateProjectContext();
            }

            public void SendRequest()
            {
                response = "Loading...";
#if UNITY_EDITOR
                EditorUtility.DisplayProgressBar("ChatGPT Request", "Sending request...", 0f);
#endif
                var finalPrompt = prompt;
                if (useContext)
                {
                    string contextSummary = GenerateProjectContext();
                    finalPrompt = $"{contextSummary}\n\nThen, considering the above context, answer the following prompt:\n\n{prompt}";
                }

                _request = new RequestWrapper
                {
                    model = model,
                    messages = new List<Message>
                    {
                        new Message
                        {
                            role = "user",
                            content = finalPrompt
                        }
                    },
                    max_tokens = (int)maxTokens, 
                    temperature = 0.7f,
                    stream = false
                };

                StartCoroutine(SendRequestCoroutine());
            }

            private IEnumerator SendRequestCoroutine()
{
    var json = BuildJsonRequest(_request);
    if (debug)
    {
        Debug.Log($"[ChatGPT Request JSON]:\n{json}");
    }
    var rawData = Encoding.UTF8.GetBytes(json);

    int retryCount = 0;
    int maxRetries = 5;

    while (retryCount <= maxRetries)
    {
        using var apiRequest = new UnityWebRequest(ChatGpturl, "POST")
        {
            uploadHandler = new UploadHandlerRaw(rawData),
            downloadHandler = new DownloadHandlerBuffer()
        };
        apiRequest.SetRequestHeader("Content-Type", "application/json");
        apiRequest.SetRequestHeader("Authorization", "Bearer " + apiKey);
        apiRequest.SetRequestHeader("OpenAI-Organization", "org-XlU6AT7uxAJZFI3u6fOGl90w");
        apiRequest.SetRequestHeader("OpenAI-Project", "proj_93dtjeCyqpuWeegBTF36k3Pj");

        if (debug)
        {
            Debug.Log($"[ChatGPT Attempt]: Sending request (attempt {retryCount + 1})...");
        }
        var requestOperation = apiRequest.SendWebRequest();

        bool userCancelled = false;

        while (!requestOperation.isDone)
        {
#if UNITY_EDITOR
            bool cancelPressed = EditorUtility.DisplayCancelableProgressBar(
                "ChatGPT Request",
                $"Sending request... Attempt {retryCount + 1}",
                0.1f + 0.15f * retryCount
            );
            if (cancelPressed)
            {
                userCancelled = true;
                break;
            }
#endif
            yield return null; // <- Clave: chequeamos cada frame
        }

#if UNITY_EDITOR
        EditorUtility.ClearProgressBar();
#endif

        if (userCancelled)
        {
            if (debug)
            {
                Debug.LogWarning("[ChatGPT]: Request cancelled by user.");
            }
            Debug.LogWarning("Request cancelled by user.");
            response = "Request cancelled by user.";
            yield break;
        }

        if (apiRequest.result == UnityWebRequest.Result.Success)
        {
            var jsonResponse = apiRequest.downloadHandler.text;
            if (debug)
            {
                Debug.Log($"[ChatGPT Response JSON]:\n{jsonResponse}");
            }
            _responseBody = JsonConvert.DeserializeObject<ResponseWrapper>(jsonResponse);
            response = _responseBody.choices[0].message.content;
            yield break;
        }
        else if ((int)apiRequest.responseCode == 429)
        {
            int waitTime = (int)Mathf.Pow(2, retryCount);
            Debug.LogWarning($"Rate limited. Retrying in {waitTime} seconds...");
            response = $"Rate limited. Retrying in {waitTime} seconds...";
            yield return new WaitForSeconds(waitTime);
            retryCount++;
        }
        else
        {
            if (debug)
            {
                Debug.LogError($"[ChatGPT Error]: Status {apiRequest.responseCode} - {apiRequest.error}");
            }
            response = $"Error: {apiRequest.responseCode} - {apiRequest.error}";
            yield break;
        }
    }

    Debug.LogError("Max retries reached. Request failed.");
}
            
    
            private string BuildJsonRequest(RequestWrapper request)
            {
                return JsonConvert.SerializeObject(request, Formatting.Indented, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            }
            
            private string GenerateProjectContext()
            {
#if UNITY_EDITOR
                StringBuilder contextBuilder = new StringBuilder();
                contextBuilder.AppendLine("Project Context Overview:");
                contextBuilder.AppendLine();
                var processedTypes = new HashSet<Type>();

                if (contextTarget != null)
                {
                    ProcessGameObject(contextTarget, contextBuilder, processedTypes);
                }
                else
                {
                    string[] scriptGuids = UnityEditor.AssetDatabase.FindAssets("t:Script");
                    foreach (string guid in scriptGuids)
                    {
                        string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                        MonoScript monoScript = UnityEditor.AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                        if (monoScript != null)
                        {
                            var scriptClass = monoScript.GetClass();
                            if (scriptClass != null && !processedTypes.Contains(scriptClass))
                            {
                                AppendClassInfo(scriptClass, contextBuilder);
                                processedTypes.Add(scriptClass);
                            }
                        }
                        // Limitar tamaño dinámicamente para no pasarnos
                        if ((EstimateTokens(contextBuilder.ToString()) + EstimateTokens(prompt)) > (int)maxTokens)
                            break;
                    }
                }
                generatedContextPreview = contextBuilder.ToString();
                return generatedContextPreview;
#else
                return string.Empty;
#endif
            }

#if UNITY_EDITOR
            private void ProcessGameObject(GameObject obj, StringBuilder contextBuilder, HashSet<Type> processedTypes)
            {
                if (obj == null) return;
                var monoBehaviours = obj.GetComponents<MonoBehaviour>();
                foreach (var comp in monoBehaviours)
                {
                    if (comp == null) continue;
                    var type = comp.GetType();
                    if (!processedTypes.Contains(type))
                    {
                        AppendClassInfo(type, contextBuilder);
                        processedTypes.Add(type);
                        if (includeDependencies)
                        {
                            ProcessDependencies(type, contextBuilder, processedTypes);
                        }
                    }
                    // Limitar tamaño dinámicamente para no pasarnos
                    if ((EstimateTokens(contextBuilder.ToString()) + EstimateTokens(prompt)) > (int)maxTokens)
                        return;
                }
            }

            private void AppendClassInfo(Type type, StringBuilder contextBuilder)
            {
                if (type == null) return;
                contextBuilder.AppendLine($"- Class: {type.Name}");
                var methods = type.GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.DeclaredOnly);
                foreach (var method in methods)
                {
                    if (!method.IsSpecialName)
                    {
                        contextBuilder.AppendLine($"    - Method: {method.Name}({string.Join(", ", method.GetParameters().Select(p => p.ParameterType.Name + " " + p.Name))})");
                    }
                }
                var properties = type.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.DeclaredOnly);
                foreach (var prop in properties)
                {
                    contextBuilder.AppendLine($"    - Property: {prop.PropertyType.Name} {prop.Name}");
                }
                contextBuilder.AppendLine();
            }

            private void ProcessDependencies(Type type, StringBuilder contextBuilder, HashSet<Type> processedTypes)
            {
                if (type == null) return;
                var fields = type.GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                foreach (var field in fields)
                {
                    // Only consider fields that are MonoBehaviour and are public or have [SerializeField]
                    bool isSerializable = field.IsPublic || Attribute.IsDefined(field, typeof(SerializeField));
                    if (!isSerializable) continue;
                    if (!typeof(MonoBehaviour).IsAssignableFrom(field.FieldType)) continue;
                    var depType = field.FieldType;
                    if (processedTypes.Contains(depType)) continue;
                    AppendClassInfo(depType, contextBuilder);
                    processedTypes.Add(depType);
                    // Recursive dependency search
                    ProcessDependencies(depType, contextBuilder, processedTypes);
                    // Limitar tamaño dinámicamente para no pasarnos
                    if ((EstimateTokens(contextBuilder.ToString()) + EstimateTokens(prompt)) > (int)maxTokens)
                        return;
                }
            }
#endif

            public void Clear()
            {
                prompt = string.Empty;
                response = string.Empty;
            }

            public void SaveScript()
            {
                if (!System.IO.Directory.Exists(Path.Combine(scriptsFolder, Directory)))
                {
                    System.IO.Directory.CreateDirectory(Path.Combine(scriptsFolder, Directory));
                }

                var className = ParseClassName(response);
                var scriptPath = Path.Combine(scriptsFolder, Directory, className + ".cs");

                using var fs = new FileStream(scriptPath, FileMode.Create);
                using var sw = new StreamWriter(fs);
                sw.Write(response);
            }

            private static string ParseClassName(string result)
            {
                var indexClass = result.IndexOf("class", StringComparison.Ordinal);
                if (indexClass == -1) return "UnnamedScript";

                var indexEnd = result.IndexOfAny(new[]{ ' ', ':', '\n', '{' }, indexClass + 6);
                if (indexEnd == -1) indexEnd = result.Length;

                return result.Substring(indexClass + 6, indexEnd - (indexClass + 6)).Trim();
            }
            
            private int EstimateTokens(string text)
            {
                if (string.IsNullOrEmpty(text))
                    return 0;

                return Mathf.CeilToInt((float)text.Length / AverageCharsPerToken);
            }
            
            public void UpdateEstimatedCost()
            {
                int promptTokens = EstimateTokens(prompt);

                int contextTokens = 0;
                if (useContext)
                {
                    if (string.IsNullOrEmpty(generatedContextPreview))
                        GenerateProjectContext();

                    contextTokens = EstimateTokens(generatedContextPreview);
                }

                int totalTokens = promptTokens + contextTokens;

                if (modelTokenPrices != null && modelTokenPrices.ContainsKey(selectedModelIndex))
                {
                    tokenPricePerThousand = modelTokenPrices[selectedModelIndex];
                }
                else
                {
                    tokenPricePerThousand = 0.0015f; // fallback default
                }

                estimatedPromptCost = (totalTokens / 1000f) * tokenPricePerThousand;
            }
            
            private void OnValidate()
            {
                if (selectedModelIndex >= 0 && selectedModelIndex < ChatGptModels.Models.Length)
                {
                    model = ChatGptModels.Models[selectedModelIndex];
                }

                if (modelTokenPrices != null && modelTokenPrices.ContainsKey(selectedModelIndex))
                {
                    tokenPricePerThousand = modelTokenPrices[selectedModelIndex];
                }
                else
                {
                    tokenPricePerThousand = 0.0015f;
                }
            }

            private void InitializeModelTokenPrices()
            {
                modelTokenPrices = new Dictionary<int, float>
                {
                    // Example mappings, adjust as needed for actual model costs
                    { 0, 0.002f },   // Model index 0
                    { 1, 0.003f },   // Model index 1
                    { 2, 0.0015f },  // Model index 2
                    { 19, 0.0015f }, // Default GPT-3.5 turbo at index 19
                    // Add other model indices and their costs here
                };
            }

            [Serializable]
            public class RequestWrapper
            {
                [JsonProperty("model")]
                public string model;

                [JsonProperty("messages")]
                public List<Message> messages;

                [JsonProperty("max_tokens")]
                public int max_tokens;

                [JsonProperty("temperature")]
                public float temperature;

                [JsonProperty("stream")]
                public bool stream = false;

                [JsonProperty("tools")]
                public List<object> tools = null; // Placeholder if needed later
            }

            [Serializable]
            public class Message
            {
                [JsonProperty("role")]
                public string role;

                [JsonProperty("content")]
                public string content;

                [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
                public string name;

                [JsonProperty("tool_call_id", NullValueHandling = NullValueHandling.Ignore)]
                public string tool_call_id;

                [JsonProperty("function_call", NullValueHandling = NullValueHandling.Ignore)]
                public FunctionCall function_call;
            }

            [Serializable]
            public class ResponseWrapper
            {
                [JsonProperty("id")]
                public string id;

                [JsonProperty("object")]
                public string @object;

                [JsonProperty("created")]
                public int created;

                [JsonProperty("model")]
                public string model;

                [JsonProperty("system_fingerprint")]
                public string system_fingerprint;

                [JsonProperty("choices")]
                public List<Choice> choices;

                [JsonProperty("usage")]
                public Usage usage;
            }

            [Serializable]
            public class Choice
            {
                [JsonProperty("index")]
                public int index;

                [JsonProperty("message")]
                public Message message;

                [JsonProperty("logprobs")]
                public object logprobs; // Nullable

                [JsonProperty("finish_reason")]
                public string finish_reason;
            }

            [Serializable]
            public class Usage
            {
                [JsonProperty("prompt_tokens")]
                public int prompt_tokens;

                [JsonProperty("completion_tokens")]
                public int completion_tokens;

                [JsonProperty("total_tokens")]
                public int total_tokens;
            }

            [Serializable]
            public class FunctionCall
            {
                [JsonProperty("name")]
                public string name;

                [JsonProperty("arguments")]
                public string arguments;
            }
        }
}