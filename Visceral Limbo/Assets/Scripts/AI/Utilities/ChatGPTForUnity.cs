using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using Newtonsoft.Json;
namespace AI.Utilities
{
     public class ChatGptForUnity : MonoBehaviour
        {
            [FormerlySerializedAs("APIKey")]
            [Header("Configuration")]
            [SerializeField] private string apiKey;
            [SerializeField] private int selectedModelIndex = 0;
            [SerializeField] private string model = ChatGptModels.Models[0];
            [TextArea(3, 10)]
            [SerializeField] private string prompt;

            [Header("Response")]
            [TextArea(3, 40)]
            [SerializeField] private string response;

            private const string ChatGpturl = "https://api.openai.com/v1/chat/completions";
            [FormerlySerializedAs("_scriptsFolder")] [SerializeField] private string scriptsFolder = "Assets/Scripts/AI";
            private const string Directory = "ChatGPT";

            private RequestWrapper _request;
            private ResponseWrapper _responseBody;

            public void SendRequest()
            {
                response = "Loading...";

                _request = new RequestWrapper
                {
                    model = model,
                    messages = new List<Message>
                    {
                        new Message
                        {
                            role = "user",
                            content = prompt
                            // name, tool_call_id, function_call intentionally left null
                        }
                    },
                    max_tokens = 2048,
                    temperature = 0.7f,
                    stream = false
                };

                StartCoroutine(SendRequestCoroutine());
            }

            private IEnumerator SendRequestCoroutine()
            {
                var json = BuildJsonRequest(_request);
                var rawData = Encoding.UTF8.GetBytes(json);

                using var apiRequest = new UnityWebRequest(ChatGpturl, "POST")
                {
                    uploadHandler = new UploadHandlerRaw(rawData),
                    downloadHandler = new DownloadHandlerBuffer()
                };
                apiRequest.SetRequestHeader("Content-Type", "application/json");
                apiRequest.SetRequestHeader("Authorization", "Bearer " + apiKey);
                apiRequest.SetRequestHeader("OpenAI-Organization", "org-XlU6AT7uxAJZFI3u6fOGl90w");
                apiRequest.SetRequestHeader("OpenAI-Project", "proj_93dtjeCyqpuWeegBTF36k3Pj");
        
                Debug.Log("Request URL: " + ChatGpturl);
                Debug.Log("Request Body: " + json);

                int retryCount = 0;
                int maxRetries = 5;

                while (true)
                {
                    yield return apiRequest.SendWebRequest();

                    if (apiRequest.result == UnityWebRequest.Result.Success)
                    {
                        var jsonResponse = apiRequest.downloadHandler.text;
                        _responseBody = JsonConvert.DeserializeObject<ResponseWrapper>(jsonResponse);
                        response = _responseBody.choices[0].message.content;
                        yield break;
                    }
                    else if ((int)apiRequest.responseCode == 429 && retryCount < maxRetries)
                    {
                        int waitTime = (int)Mathf.Pow(2, retryCount);
                        Debug.LogWarning($"Rate limited. Retrying in {waitTime} seconds...");
                        response = $"Rate limited. Retrying in {waitTime} seconds...";
                        yield return new WaitForSeconds(waitTime);
                        retryCount++;
                    }
                    else
                    {
                        response = $"Error: {apiRequest.responseCode} - {apiRequest.error}";
                        yield break;
                    }
                }
            }
    
            private string BuildJsonRequest(RequestWrapper request)
            {
                return JsonConvert.SerializeObject(request, Formatting.Indented, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            }

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

            private void OnValidate()
            {
                if (selectedModelIndex >= 0 && selectedModelIndex < ChatGptModels.Models.Length)
                {
                    model = ChatGptModels.Models[selectedModelIndex];
                }
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