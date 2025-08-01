using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;

/// <summary>
/// Manages HTTP requests, including GET, POST, PATCH, file uploads, and multimedia downloads.
/// </summary>
public static class HttpManager
{

    /// <summary>
    /// Sends a GET request to the specified URL and retrieves the response.
    /// </summary>
    /// <param name="url">The URL to which the request will be sent.</param>
    /// <param name="callback">A callback function that receives the server's response as a string.</param>
    public static void Get(string url, System.Action<string> callback)
    {
        CoroutineRunner.Start(GetRequest(URLdirectory.serverUrl + url, callback));
    }

    /// <summary>
    /// Sends a POST request to the specified URL with JSON data.
    /// </summary>
    /// <param name="url">The URL to which the request will be sent.</param>
    /// <param name="jsonData">The JSON-formatted data to be sent in the request body.</param>
    /// <param name="callback">A callback function that receives the server's response.</param>
    public static void Post(string url, string jsonData, System.Action<string> callback)
    {
        CoroutineRunner.Start(PostRequest(URLdirectory.serverUrl + url, jsonData, callback));
    }

    /// <summary>
    /// Sends a PATCH request to the specified URL with JSON data.
    /// </summary>
    /// <param name="url">The URL to which the request will be sent.</param>
    /// <param name="jsonData">The JSON-formatted data to be sent in the request body.</param>
    /// <param name="callback">A callback function that receives the server's response.</param>
    public static void Patch(string url, string jsonData, System.Action<string> callback)
    {
        CoroutineRunner.Start(PatchRequest(URLdirectory.serverUrl + url, jsonData, callback));
    }


    /// <summary>
    /// Sends a file via a POST request to the specified URL.
    /// </summary>
    /// <param name="url">The URL to which the file will be uploaded.</param>
    /// <param name="filePath">The local path of the file to upload.</param>
    /// <param name="contentType">The MIME type of the file.</param>
    /// <param name="callback">A callback function that receives the server's response.</param>
    public static void PostFile(string url, string filePath, string contentType, System.Action<string> callback)
    {
        CoroutineRunner.Start(PostFileCoroutine(URLdirectory.serverUrl + url, filePath, contentType, callback));
    }

    /// <summary>
    /// Downloads an image texture from the specified URL and saves it locally.
    /// </summary>
    /// <param name="url">The URL of the texture to download.</param>
    /// <param name="fileName">The name under which the texture file will be saved.</param>
    /// <param name="callback">A callback function that receives the downloaded Texture2D.</param>
    public static void GetTexture(string url, string fileName, System.Action<Texture2D> callback)
    {
        CoroutineRunner.Start(GetTextureRequest(url, fileName, callback));
    }

    /// <summary>
    /// Downloads a video from the specified URL and saves it as an MP4 file.
    /// </summary>
    /// <param name="url">The URL of the video file to download.</param>
    /// <param name="fileName">The name under which the video file will be saved.</param>
    /// <param name="callback">A callback function that receives the local file path of the saved video.</param>
    public static void GetVideo(string url, string fileName, System.Action<string> callback)
    {
        CoroutineRunner.Start(GetVideoRequest(url, fileName, callback));
    }

    /// <summary>
    /// Determines the appropriate audio type based on the file extension and starts the download request.
    /// </summary>
    /// <param name="url">The URL of the audio file to download.</param>
    /// <param name="fileName">The name under which the audio file will be saved.</param>
    /// <param name="callback">A callback function that receives the downloaded AudioClip.</param>
    public static void GetAudio(string url, string fileName, System.Action<AudioClip> callback)
    {
        AudioType audioType = url.EndsWith(".wav") ? AudioType.WAV : AudioType.MPEG;
        CoroutineRunner.Start(GetAudioRequest(url, fileName, callback, audioType));
    }

    private static Dictionary<string, string> customHeaders = new Dictionary<string, string>();

    /// <summary>
    /// Adds a custom header to the request. If the header already exists, its value is updated.
    /// </summary>
    /// <param name="key">The key (name) of the header.</param>
    /// <param name="value">The value of the header.</param>
    public static void AddRequestHeader(string key, string value)
    {
        if (!customHeaders.ContainsKey(key))
        {
            customHeaders.Add(key, value);
        }
        else
        {
            customHeaders[key] = value;
        }
    }

    /// <summary>
    /// Sends a PATCH request to the specified URL with JSON data.
    /// </summary>
    /// <param name="url">The URL to which the request will be sent.</param>
    /// <param name="jsonData">The JSON-formatted data to be sent in the request body.</param>
    /// <param name="callback">A callback function that receives the server's response.</param>
    public static IEnumerator PatchRequest(string url, string jsonData, System.Action<string> callback)
    {
        UnityWebRequest request = new UnityWebRequest(url, "PATCH");
        byte[] jsonToSend = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Retrieve custom headers from HttpManager (assuming they are stored in a dictionary)
        foreach (var header in customHeaders)
        {
            request.SetRequestHeader(header.Key, header.Value);
        }

        // Build the CURL command with custom headers
        string curlCommand = $"curl -X PATCH \"{url}\" -H \"Content-Type: application/json\" -d '{jsonData}'";

        foreach (var header in customHeaders)
        {
            curlCommand += $" -H \"{header.Key}: {header.Value}\"";
        }

        yield return request.SendWebRequest();
        // Handle response
        if (request.result == UnityWebRequest.Result.Success)
        {
            callback?.Invoke(request.downloadHandler.text);
        }
        else
        {
            Debug.LogError($"Error en PATCH: {request.error}");
        }
    }

    /// <summary>
    /// Sends a GET request to the specified URL and retrieves the response.
    /// </summary>
    /// <param name="url">The URL to which the request will be sent.</param>
    /// <param name="callback">A callback function that receives the server's response as a string.</param>
    private static IEnumerator GetRequest(string url, System.Action<string> callback)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            foreach (var header in customHeaders)
            {
                webRequest.SetRequestHeader(header.Key, header.Value);
            }

            string curlCommand = GenerateCurlCommand(url, customHeaders);
            //Debug.Log($"Comando CURL: {curlCommand}");
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(url + " Error: " + webRequest.error);
                callback?.Invoke(null);
            }
            else
            {
                callback?.Invoke(webRequest.downloadHandler.text);
            }
        }
    }

    /// <summary>
    /// Generates a CURL command for a GET request, including custom headers.
    /// </summary>
    /// <param name="url">The target URL for the request.</param>
    /// <param name="headers">A dictionary containing custom headers to be included in the request.</param>
    /// <returns>A string containing the formatted CURL command.</returns>
    private static string GenerateCurlCommand(string url, Dictionary<string, string> headers)
    {
        string curl = $"curl -X GET \"{url}\"";

        foreach (var header in headers)
        {
            curl += $" -H \"{header.Key}: {header.Value}\"";
        }

        return curl;
    }

    /// <summary>
    /// Generates a CURL command for a POST request, including headers and optional JSON data.
    /// </summary>
    /// <param name="url">The target URL for the request.</param>
    /// <param name="headers">A dictionary containing custom headers to be included in the request.</param>
    /// <param name="jsonData">Optional JSON-formatted data to be sent in the request body.</param>
    /// <returns>A string containing the formatted CURL command.</returns>
    private static string GenerateCurlCommand(string url, Dictionary<string, string> headers, string jsonData = null)
    {
        string curlCommand = $"curl -X POST \"{url}\"";

        foreach (var header in headers)
        {
            curlCommand += $" -H \"{header.Key}: {header.Value}\"";
        }

        if (!string.IsNullOrEmpty(jsonData))
        {
            curlCommand += $" -d '{jsonData}'";
        }

        return curlCommand;
    }

    /// <summary>
    /// Sends a POST request to the specified URL with JSON data.
    /// </summary>
    /// <param name="url">The URL to which the request will be sent.</param>
    /// <param name="jsonData">The JSON-formatted data to be sent in the request body.</param>
    /// <param name="callback">A callback function that receives the server's response.</param>
    private static IEnumerator PostRequest(string url, string jsonData, System.Action<string> callback)
    {
        using (UnityWebRequest webRequest = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST))
        {
            // Convert JSON body to bytes and configure the UploadHandler
            byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            // Configure custom headers
            foreach (var header in customHeaders)
            {
                webRequest.SetRequestHeader(header.Key, header.Value);
            }

            // Generate and log the CURL command for debugging
            string curlCommand = GenerateCurlCommand(url, customHeaders, jsonData);

            // Send the request
            yield return webRequest.SendWebRequest();

            // Handle the response
            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                callback?.Invoke(null);
            }
            else
            {
                callback?.Invoke(webRequest.downloadHandler.text);
            }
        }
    }

    /// <summary>
    /// Downloads an image texture from the specified URL and saves it as a PNG file.
    /// </summary>
    /// <param name="url">The URL of the texture to download.</param>
    /// <param name="fileName">The name under which the texture file will be saved.</param>
    /// <param name="callback">A callback function that receives the downloaded Texture2D.</param>
    private static IEnumerator GetTextureRequest(string url, string fileName, System.Action<Texture2D> callback)
    {
        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                callback?.Invoke(null);
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);

                // Save the texture as a PNG file
                byte[] bytes = texture.EncodeToPNG();
                string savePath = Path.Combine(Application.persistentDataPath, fileName + ".png");
                File.WriteAllBytes(savePath, bytes);
                callback?.Invoke(texture);
            }
        }
    }

    /// <summary>
    /// Downloads a video from the specified URL and saves it as an MP4 file.
    /// </summary>
    /// <param name="url">The URL of the video file to download.</param>
    /// <param name="fileName">The name under which the video file will be saved.</param>
    /// <param name="callback">A callback function that receives the local file path of the saved video.</param>
    private static IEnumerator GetVideoRequest(string url, string fileName, System.Action<string> callback)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + webRequest.error);
                callback?.Invoke(null);
            }
            else
            {
                string videoPath = Path.Combine(Application.persistentDataPath, fileName + ".mp4");
                File.WriteAllBytes(videoPath, webRequest.downloadHandler.data);
                callback?.Invoke(videoPath);
            }
        }
    }

    /// <summary>
    /// Downloads an audio file from a given URL and saves it locally.
    /// </summary>
    /// <param name="url">The URL of the audio file to download.</param>
    /// <param name="fileName">The name under which the audio file will be saved.</param>
    /// <param name="callback">A callback function that receives the downloaded AudioClip.</param>
    /// <param name="audioType">The type of audio format (default is MPEG).</param>
    private static IEnumerator GetAudioRequest(string url, string fileName, System.Action<AudioClip> callback, AudioType audioType = AudioType.MPEG)
    {
        using (UnityWebRequest webRequest = UnityWebRequestMultimedia.GetAudioClip(url, audioType))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + webRequest.error + " : " + fileName);
                callback?.Invoke(null);
            }
            else
            {
                AudioClip audioClip = DownloadHandlerAudioClip.GetContent(webRequest);

                // Get the correct file extension based on the audio format
                string extension = (audioType == AudioType.WAV) ? ".wav" : ".mp3";

                // Save the file locally with the correct extension
                byte[] audioBytes = webRequest.downloadHandler.data;
                string savePath = Path.Combine(Application.persistentDataPath, fileName + extension);
                File.WriteAllBytes(savePath, audioBytes);

                //Debug.Log($"Audio saved at: {savePath}");
                callback?.Invoke(audioClip);
            }
        }
    }

    /// <summary>
    /// Cooroutine to make a post of file to server.
    /// </summary>
    /// <param name="url">URL to set post call</param>
    /// <param name="filePath">The path of file to set</param>
    /// <param name="contentType">The type of file to send, on this occasion we used video/mp4</param>
    /// <param name="callback">The callback that will be called when receiving a response from the server</param>
    /// <returns></returns>
    private static IEnumerator PostFileCoroutine(string url, string filePath, string contentType, System.Action<string> callback)
    {
        if (!File.Exists(filePath))
        {
            yield break;
        }
        HttpManager.ClearRequestHeaders();
        HttpManager.AddRequestHeader("X-Machine-Key", GlobalVariables.machinesSecretKey);
        // Read the file as bytes each time the request is sent
        byte[] fileData = File.ReadAllBytes(filePath);

        // Create a new form for each request
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", fileData, Path.GetFileName(filePath), contentType);

        // Create a new POST request for each attempt
        UnityWebRequest webRequest = UnityWebRequest.Post(url, form);

        // Configure custom headers
        foreach (var header in customHeaders)
        {
            webRequest.SetRequestHeader(header.Key, header.Value);
        }

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            callback?.Invoke(null);
        }
        else
        {
            callback?.Invoke(webRequest.downloadHandler.text);
        }
    }

    /// <summary>
    /// Clear custom headers.
    /// </summary>
    public static void ClearRequestHeaders()
    {
        customHeaders.Clear();
    }

    // RETRY METHODS WITH EXPONENTIAL BACKOFF

    /// <summary>
    /// Downloads a texture with retry logic and exponential backoff.
    /// </summary>
    /// <param name="url">The URL of the texture to download.</param>
    /// <param name="fileName">The name under which the texture file will be saved.</param>
    /// <param name="callback">A callback function that receives the downloaded Texture2D.</param>
    /// <param name="maxRetries">Maximum number of retry attempts (default: 3).</param>
    /// <param name="baseDelaySeconds">Base delay for exponential backoff (default: 1 second).</param>
    public static void GetTextureWithRetries(string url, string fileName, System.Action<Texture2D> callback, int maxRetries = 3, float baseDelaySeconds = 1f)
    {
        CoroutineRunner.Start(GetTextureWithRetriesCoroutine(url, fileName, callback, maxRetries, baseDelaySeconds));
    }

    /// <summary>
    /// Downloads a video with retry logic and exponential backoff.
    /// </summary>
    /// <param name="url">The URL of the video to download.</param>
    /// <param name="fileName">The name under which the video file will be saved.</param>
    /// <param name="callback">A callback function that receives the local file path of the saved video.</param>
    /// <param name="maxRetries">Maximum number of retry attempts (default: 3).</param>
    /// <param name="baseDelaySeconds">Base delay for exponential backoff (default: 1 second).</param>
    public static void GetVideoWithRetries(string url, string fileName, System.Action<string> callback, int maxRetries = 3, float baseDelaySeconds = 1f)
    {
        CoroutineRunner.Start(GetVideoWithRetriesCoroutine(url, fileName, callback, maxRetries, baseDelaySeconds));
    }

    /// <summary>
    /// Downloads an audio file with retry logic and exponential backoff.
    /// </summary>
    /// <param name="url">The URL of the audio file to download.</param>
    /// <param name="fileName">The name under which the audio file will be saved.</param>
    /// <param name="callback">A callback function that receives the downloaded AudioClip.</param>
    /// <param name="maxRetries">Maximum number of retry attempts (default: 3).</param>
    /// <param name="baseDelaySeconds">Base delay for exponential backoff (default: 1 second).</param>
    public static void GetAudioWithRetries(string url, string fileName, System.Action<AudioClip> callback, int maxRetries = 3, float baseDelaySeconds = 1f)
    {
        AudioType audioType = url.EndsWith(".wav") ? AudioType.WAV : AudioType.MPEG;
        CoroutineRunner.Start(GetAudioWithRetriesCoroutine(url, fileName, callback, audioType, maxRetries, baseDelaySeconds));
    }

    /// <summary>
    /// Coroutine that downloads a texture with retry logic and exponential backoff.
    /// </summary>
    private static IEnumerator GetTextureWithRetriesCoroutine(string url, string fileName, System.Action<Texture2D> callback, int maxRetries, float baseDelaySeconds)
    {
        int attempt = 0;
        
        while (attempt < maxRetries)
        {
            Debug.Log($"Downloading texture attempt {attempt + 1}/{maxRetries}: {fileName}");
            
            bool completed = false;
            Texture2D result = null;
            
            // Start the download
            CoroutineRunner.Start(GetTextureRequest(url, fileName, (texture) => {
                result = texture;
                completed = true;
            }));
            
            // Wait for completion
            yield return new WaitUntil(() => completed);
            
            // Check if successful
            if (result != null)
            {
                Debug.Log($"Successfully downloaded texture: {fileName}");
                callback?.Invoke(result);
                yield break;
            }
            
            attempt++;
            
            // If this was the last attempt, fail
            if (attempt >= maxRetries)
            {
                Debug.LogError($"Failed to download texture after {maxRetries} attempts: {fileName}");
                callback?.Invoke(null);
                yield break;
            }
            
            // Calculate exponential backoff delay
            float delay = baseDelaySeconds * Mathf.Pow(2, attempt - 1);
            Debug.LogWarning($"Texture download failed, retrying in {delay:F1} seconds: {fileName}");
            yield return new WaitForSeconds(delay);
        }
    }

    /// <summary>
    /// Coroutine that downloads a video with retry logic and exponential backoff.
    /// </summary>
    private static IEnumerator GetVideoWithRetriesCoroutine(string url, string fileName, System.Action<string> callback, int maxRetries, float baseDelaySeconds)
    {
        int attempt = 0;
        
        while (attempt < maxRetries)
        {
            Debug.Log($"Downloading video attempt {attempt + 1}/{maxRetries}: {fileName}");
            
            bool completed = false;
            string result = null;
            
            // Start the download
            CoroutineRunner.Start(GetVideoRequest(url, fileName, (path) => {
                result = path;
                completed = true;
            }));
            
            // Wait for completion
            yield return new WaitUntil(() => completed);
            
            // Check if successful
            if (!string.IsNullOrEmpty(result))
            {
                Debug.Log($"Successfully downloaded video: {fileName}");
                callback?.Invoke(result);
                yield break;
            }
            
            attempt++;
            
            // If this was the last attempt, fail
            if (attempt >= maxRetries)
            {
                Debug.LogError($"Failed to download video after {maxRetries} attempts: {fileName}");
                callback?.Invoke(null);
                yield break;
            }
            
            // Calculate exponential backoff delay
            float delay = baseDelaySeconds * Mathf.Pow(2, attempt - 1);
            Debug.LogWarning($"Video download failed, retrying in {delay:F1} seconds: {fileName}");
            yield return new WaitForSeconds(delay);
        }
    }

    /// <summary>
    /// Coroutine that downloads an audio file with retry logic and exponential backoff.
    /// </summary>
    private static IEnumerator GetAudioWithRetriesCoroutine(string url, string fileName, System.Action<AudioClip> callback, AudioType audioType, int maxRetries, float baseDelaySeconds)
    {
        int attempt = 0;
        
        while (attempt < maxRetries)
        {
            Debug.Log($"Downloading audio attempt {attempt + 1}/{maxRetries}: {fileName}");
            
            bool completed = false;
            AudioClip result = null;
            
            // Start the download
            CoroutineRunner.Start(GetAudioRequest(url, fileName, (audio) => {
                result = audio;
                completed = true;
            }, audioType));
            
            // Wait for completion
            yield return new WaitUntil(() => completed);
            
            // Check if successful
            if (result != null)
            {
                Debug.Log($"Successfully downloaded audio: {fileName}");
                callback?.Invoke(result);
                yield break;
            }
            
            attempt++;
            
            // If this was the last attempt, fail
            if (attempt >= maxRetries)
            {
                Debug.LogError($"Failed to download audio after {maxRetries} attempts: {fileName}");
                callback?.Invoke(null);
                yield break;
            }
            
            // Calculate exponential backoff delay
            float delay = baseDelaySeconds * Mathf.Pow(2, attempt - 1);
            Debug.LogWarning($"Audio download failed, retrying in {delay:F1} seconds: {fileName}");
            yield return new WaitForSeconds(delay);
        }
    }
}