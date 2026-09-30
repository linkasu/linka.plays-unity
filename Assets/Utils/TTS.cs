using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.Networking;
using System.IO;
using System.Text;


public class TTS {
    private const string DefaultApiUrl = "https://tts.linka.su/tts";
    private const string DefaultVoice = "jane";
    private static string audioSavePath = "Assets/Audio/";

    [Serializable]
    private class TtsRequest
    {
        public string text;
        public string voice;
    }

    public static void SpeakAndSaveAudio(string textToSpeak, string fileId)
    {
        if (string.IsNullOrEmpty(textToSpeak))
        {
            Debug.LogError("Text to speak is empty!");
            return;
        }

        string requestBody = JsonUtility.ToJson(new TtsRequest
        {
            text = textToSpeak,
            voice = DefaultVoice
        });
        var request = new UnityWebRequest(GetApiUrl(), UnityWebRequest.kHttpVerbPOST)
        {
            uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(requestBody)),
            downloadHandler = new DownloadHandlerBuffer()
        };
        request.SetRequestHeader("Content-Type", "application/json");

        string installationToken = Environment.GetEnvironmentVariable("LINKA_TTS_INSTALLATION_TOKEN");
        if (!string.IsNullOrEmpty(installationToken))
        {
            request.SetRequestHeader("X-TTS-Installation-Token", installationToken);
            request.SetRequestHeader("Idempotency-Key", Guid.NewGuid().ToString());
        }

        var operation = request.SendWebRequest();


        operation.completed += (operation) =>
            {
                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError("Error requesting TTS: " + request.error);
                }
                else
                {
                    byte[] audioData = request.downloadHandler.data;
                    SaveAudioClip(audioData, fileId);
                    Debug.Log("Text successfully spoken and audio saved!");
                }
            };
    }

    private static string GetApiUrl()
    {
        string configuredUrl = Environment.GetEnvironmentVariable("LINKA_TTS_URL");
        return string.IsNullOrEmpty(configuredUrl) ? DefaultApiUrl : configuredUrl;
    }

    private static void SaveAudioClip(byte[] audioData, string fileId)
    {
        string fileName = $"TTS_{fileId}.mp3";
        string fullPath = Path.Combine(audioSavePath, fileName);

        if (!Directory.Exists(audioSavePath))
        {
            Directory.CreateDirectory(audioSavePath);
        }

        File.WriteAllBytes(fullPath, audioData);
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
        Debug.Log($"Audio clip saved at: {fullPath}");
    }
}
