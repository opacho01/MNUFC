using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class OfflineDataUploader : MonoBehaviour
{
    [Header("UI References")]
    public Button uploadButton;
    public TMP_Text statusText;
    public GameObject progressPanel;
    public Slider progressSlider;
    public TMP_Text progressText;

    [Header("Dependencies")]
    public VideoUpload videoUpload;
    public Metrics metrics;

    private List<string> filesToUpload = new List<string>();
    private int totalFiles = 0;
    private int uploadedFiles = 0;
    private bool isUploading = false;

    void Start()
    {
        // Hide progress panel initially
        if (progressPanel != null)
        {
            progressPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Starts the process of uploading all offline data
    /// </summary>
    public void StartUploadProcess()
    {
        if (isUploading)
        {
            Debug.LogWarning("Upload already in progress");
            return;
        }

        StartCoroutine(UploadAllOfflineData());
    }

    /// <summary>
    /// Coroutine to handle the complete upload process
    /// </summary>
    private IEnumerator UploadAllOfflineData()
    {
        isUploading = true;
        uploadedFiles = 0;
        filesToUpload.Clear();

        progressPanel.SetActive(true);

        // Find all files to upload
        FindFilesToUpload();

        totalFiles = filesToUpload.Count;

        if (totalFiles == 0)
        {
            UpdateStatus("No offline files found to upload");
            isUploading = false;
            if (progressPanel != null)
            {
                progressPanel.SetActive(false);
            }
            yield break;
        }

        UpdateStatus($"Found {totalFiles} files to upload");
        UpdateProgress(0, totalFiles);

        // Upload files one by one
        foreach (string filePath in filesToUpload)
        {
            yield return StartCoroutine(UploadFile(filePath));
            uploadedFiles++;
            UpdateProgress(uploadedFiles, totalFiles);
        }

        // Complete the process
        UpdateStatus("Upload completed successfully");
        yield return new WaitForSeconds(2f);

        // Hide progress panel
        if (progressPanel != null)
        {
            progressPanel.SetActive(false);
        }

        isUploading = false;
    }

    /// <summary>
    /// Finds all text files and their corresponding videos in the persistent data path
    /// </summary>
    private void FindFilesToUpload()
    {
        string persistentDataPath = Application.persistentDataPath;

        if (!Directory.Exists(persistentDataPath))
        {
            Debug.LogWarning("Persistent data path does not exist");
            return;
        }

        // Find all .txt files in the root directory
        string[] txtFiles = Directory.GetFiles(persistentDataPath, "*.txt");

        foreach (string txtFile in txtFiles)
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(txtFile);

            // Skip Player and Player-prev files
            if (fileNameWithoutExtension == "Player" || fileNameWithoutExtension == "Player-prev")
            {
                Debug.Log($"Skipping system file: {fileNameWithoutExtension}");
                continue;
            }

            // Skip prize files here - we'll handle them with their corresponding metrics files
            if (fileNameWithoutExtension.EndsWith("prize"))
            {
                continue;
            }

            // This is a regular metrics file
            string baseFileName = fileNameWithoutExtension;

            // FIRST: Add prize file if it exists (this ensures it's uploaded first)
            string prizeFilePath = Path.Combine(persistentDataPath, baseFileName + "prize.txt");
            if (File.Exists(prizeFilePath))
            {
                filesToUpload.Add(prizeFilePath);
                Debug.Log($"Found prize file: {prizeFilePath}");
            }

            // SECOND: Add the main metrics file
            filesToUpload.Add(txtFile);
            Debug.Log($"Found metrics file: {txtFile}");

            // THIRD: Check if there's a corresponding folder with video
            string videoFolder = Path.Combine(persistentDataPath, baseFileName);
            if (Directory.Exists(videoFolder))
            {
                // Look for video file with same name in the folder
                string videoFile = Path.Combine(videoFolder, baseFileName + ".mp4");
                if (File.Exists(videoFile))
                {
                    filesToUpload.Add(videoFile);
                    Debug.Log($"Found video for {baseFileName}: {videoFile}");
                }
                else
                {
                    // If exact name not found, look for any .mp4 file in the folder
                    string[] videoFiles = Directory.GetFiles(videoFolder, "*.mp4");
                    if (videoFiles.Length > 0)
                    {
                        filesToUpload.Add(videoFiles[0]);
                        Debug.Log($"Found alternative video for {baseFileName}: {videoFiles[0]}");
                    }
                    else
                    {
                        Debug.Log($"No video found for {baseFileName}, uploading metrics only");
                    }
                }
            }
            else
            {
                Debug.Log($"No video folder found for {baseFileName}, uploading metrics only");
            }
        }

        Debug.Log($"Found {filesToUpload.Count} files to upload");

        // Log the upload order for verification
        Debug.Log("Upload order:");
        for (int i = 0; i < filesToUpload.Count; i++)
        {
            string fileName = Path.GetFileName(filesToUpload[i]);
            Debug.Log($"{i + 1}. {fileName}");
        }
    }

    /// <summary>
    /// Uploads a single file and handles the response
    /// </summary>
    /// <param name="filePath">Path of the file to upload</param>
    private IEnumerator UploadFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"File does not exist: {filePath}");
            yield break;
        }

        string fileName = Path.GetFileName(filePath);
        string fileExtension = Path.GetExtension(filePath).ToLower();
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);

        UpdateStatus($"Uploading: {fileName}");

        // Check if this is a prize file
        bool isPrizeFile = fileNameWithoutExtension.EndsWith("prize");

        if (isPrizeFile)
        {
            // This is a prize file - send to specific endpoint
            yield return StartCoroutine(UploadPrizeFile(filePath));
        }
        else if (fileExtension == ".txt")
        {
            // This is a metrics file - send as JSON
            yield return StartCoroutine(UploadMetricsFile(filePath));
        }
        else if (fileExtension == ".mp4")
        {
            // This is a video file - use VideoUpload component
            yield return StartCoroutine(UploadVideoFile(filePath));
        }
        else
        {
            Debug.LogWarning($"Unsupported file type: {fileExtension}");
        }
    }

    /// <summary>
    /// Uploads a prize file to the specific endpoint using UnityWebRequest directly
    /// </summary>
    /// <param name="filePath">Path to the prize file</param>
    private IEnumerator UploadPrizeFile(string filePath)
    {
        bool uploadCompleted = false;
        bool uploadSuccess = false;
        string responseText = "";
        string jsonData = "";
        string fileName = "";

        try
        {
            jsonData = File.ReadAllText(filePath);
            fileName = Path.GetFileName(filePath);

            Debug.Log($"=== STARTING PRIZE FILE UPLOAD ===");
            Debug.Log($"Prize file: {fileName}");
            Debug.Log($"File path: {filePath}");
            Debug.Log($"JSON data length: {jsonData.Length}");
            Debug.Log($"JSON data: {jsonData}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error reading prize file {filePath}: {e.Message}");
            HandlePrizeUploadResponse(null, filePath, Path.GetFileName(filePath));
            yield break;
        }

        // Usar la URL completa directamente
        string uploadUrl2 = "https://whitelabelvendingmachine.com/api/v1/games/machineauth?mode=offline";
        string uploadUrl = URLdirectory.serverUrl + URLdirectory.rewardUrl + "?mode=offline";
        Debug.Log("ÑÑÑÑÑÑÑÑÑÑÑ " + uploadUrl);
        Debug.Log("ÑÑÑÑÑÑÑÑÑÑÑ2 " + uploadUrl2);
        // Print CURL command for debugging
        PrintPrizeCurlCommand(uploadUrl, jsonData);

        Debug.Log($"Sending prize file to: {uploadUrl}");

        // Usar UnityWebRequest directamente para evitar problemas con HttpManager
        using (UnityWebRequest webRequest = new UnityWebRequest(uploadUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("X-Machine-Key", GlobalVariables.machinesSecretKey);

            // Agregar logs de los headers
            Debug.Log($"Headers: X-Machine-Key: {GlobalVariables.machinesSecretKey}");
            Debug.Log($"Headers: Content-Type: application/json");

            // Enviar la request fuera del try-catch
            yield return webRequest.SendWebRequest();

            // Manejar la respuesta después del yield
            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Prize upload error: {webRequest.error}");
                Debug.LogError($"Response: {webRequest.downloadHandler?.text}");
                responseText = null;
            }
            else
            {
                responseText = webRequest.downloadHandler.text;
                Debug.Log($"Prize upload successful: {responseText}");
            }

            uploadCompleted = true;
            uploadSuccess = !string.IsNullOrEmpty(responseText);
        }

        HandlePrizeUploadResponse(responseText, filePath, fileName);
    }

    /// <summary>
    /// Prints a CURL command for prize file upload
    /// </summary>
    private void PrintPrizeCurlCommand(string url, string jsonData)
    {
        try
        {
            // Escapar comillas en el JSON para el comando CURL
            string escapedJson = jsonData.Replace("'", "'\\''");

            string curlCommand = $"curl -X POST \\\n";
            curlCommand += $"  -H \"X-Machine-Key: {GlobalVariables.machinesSecretKey}\" \\\n";
            curlCommand += $"  -H \"Content-Type: application/json\" \\\n";
            curlCommand += $"  -d '{escapedJson}' \\\n";
            curlCommand += $"  \"{url}\"";

            Debug.Log("=== PRIZE CURL COMMAND ===");
            Debug.Log(curlCommand);
            Debug.Log("=== END CURL COMMAND ===");

            // También imprimir una versión simplificada
            string simpleCurl = $"curl -X POST -H \"X-Machine-Key: {GlobalVariables.machinesSecretKey}\" -H \"Content-Type: application/json\" -d '{escapedJson}' \"{url}\"";
            Debug.Log($"Simplified CURL: {simpleCurl}");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Failed to generate prize CURL command: {e.Message}");
        }
    }


    /// <summary>
    /// Handles the response from prize upload - DELETES FILE ON SUCCESS
    /// </summary>
    private void HandlePrizeUploadResponse(string response, string filePath, string fileName)
    {
        Debug.Log($"=== HANDLE PRIZE UPLOAD RESPONSE ===");
        Debug.Log($"File: {fileName}");
        Debug.Log($"Response: {response}");

        if (!string.IsNullOrEmpty(response))
        {
            Debug.Log($"SUCCESS: Prize data uploaded: {fileName}");

            // Delete the prize file after successful upload
            SafeDeleteFile(filePath);
            Debug.Log($"SUCCESS: Prize file deleted: {fileName}");
        }
        else
        {
            Debug.LogWarning($"FAILED: Prize data upload: {fileName}");
            // Keep the file for retry later
        }
    }

    /// <summary>
    /// Uploads a metrics text file as JSON
    /// </summary>
    /// <param name="filePath">Path to the metrics file</param>
    private IEnumerator UploadMetricsFile(string filePath)
    {
        bool uploadCompleted = false;
        bool uploadSuccess = false;
        string responseText = "";
        string jsonData = "";
        string fileName = "";

        try
        {
            jsonData = File.ReadAllText(filePath);
            fileName = Path.GetFileName(filePath);

            Debug.Log($"=== STARTING METRICS FILE UPLOAD ===");
            Debug.Log($"Metrics file: {fileName}");
            Debug.Log($"File path: {filePath}");
            Debug.Log($"JSON data length: {jsonData.Length}");
            Debug.Log($"JSON data: {jsonData}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error reading metrics file {filePath}: {e.Message}");
            HandleMetricsUploadResponse(null, filePath, Path.GetFileName(filePath));
            yield break;
        }

        // Usar la URL completa directamente
        string uploadUrl2 = "https://whitelabelvendingmachine.com/api/v1/analytics/?mode=offline";
        string uploadUrl = URLdirectory.serverUrl + URLdirectory.sendAnalitics + "?mode=offline";
        Debug.Log("ÑÑÑÑÑÑÑÑÑÑÑ " + uploadUrl);
        Debug.Log("ÑÑÑÑÑÑÑÑÑÑÑ2 " + uploadUrl2);
        // Print CURL command for debugging
        PrintMetricsCurlCommand(uploadUrl, jsonData);

        Debug.Log($"Sending metrics file to: {uploadUrl}");

        // Usar UnityWebRequest directamente para evitar problemas con HttpManager
        using (UnityWebRequest webRequest = new UnityWebRequest(uploadUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("X-Machine-Key", GlobalVariables.machinesSecretKey);

            // Agregar logs de los headers
            Debug.Log($"Headers: X-Machine-Key: {GlobalVariables.machinesSecretKey}");
            Debug.Log($"Headers: Content-Type: application/json");

            // Enviar la request fuera del try-catch
            yield return webRequest.SendWebRequest();

            // Manejar la respuesta después del yield
            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Metrics upload error: {webRequest.error}");
                Debug.LogError($"Response: {webRequest.downloadHandler?.text}");
                responseText = null;
            }
            else
            {
                responseText = webRequest.downloadHandler.text;
                Debug.Log($"Metrics upload successful: {responseText}");
            }

            uploadCompleted = true;
            uploadSuccess = !string.IsNullOrEmpty(responseText);
        }

        HandleMetricsUploadResponse(responseText, filePath, fileName);
    }

    /// <summary>
    /// Prints a CURL command for metrics file upload
    /// </summary>
    private void PrintMetricsCurlCommand(string url, string jsonData)
    {
        try
        {
            // Escapar comillas en el JSON para el comando CURL
            string escapedJson = jsonData.Replace("'", "'\\''");

            string curlCommand = $"curl -X POST \\\n";
            curlCommand += $"  -H \"X-Machine-Key: {GlobalVariables.machinesSecretKey}\" \\\n";
            curlCommand += $"  -H \"Content-Type: application/json\" \\\n";
            curlCommand += $"  -d '{escapedJson}' \\\n";
            curlCommand += $"  \"{url}\"";

            Debug.Log("=== METRICS CURL COMMAND ===");
            Debug.Log(curlCommand);
            Debug.Log("=== END CURL COMMAND ===");

            // También imprimir una versión simplificada
            string simpleCurl = $"curl -X POST -H \"X-Machine-Key: {GlobalVariables.machinesSecretKey}\" -H \"Content-Type: application/json\" -d '{escapedJson}' \"{url}\"";
            Debug.Log($"Simplified CURL: {simpleCurl}");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Failed to generate metrics CURL command: {e.Message}");
        }
    }


    /// <summary>
    /// Handles the response from metrics upload - MODIFICADO PARA BORRAR ARCHIVOS TXT
    /// </summary>
    private void HandleMetricsUploadResponse(string response, string filePath, string fileName)
    {
        Debug.Log($"=== HANDLE METRICS UPLOAD RESPONSE ===");
        Debug.Log($"File: {fileName}");
        Debug.Log($"Response: {response}");

        if (!string.IsNullOrEmpty(response))
        {
            Debug.Log($"SUCCESS: Metrics uploaded: {fileName}");

            // SIEMPRE borrar el archivo .txt después de un envío exitoso
            SafeDeleteFile(filePath);
            Debug.Log($"SUCCESS: Metrics file deleted: {fileName}");

            // Verificar si existe una carpeta de video correspondiente
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            string videoFolder = Path.Combine(Application.persistentDataPath, fileNameWithoutExtension);

            // Si no existe carpeta de video o no hay archivos de video, el proceso termina aquí
            if (!Directory.Exists(videoFolder))
            {
                Debug.Log($"No video folder found for {fileNameWithoutExtension}, metrics cleanup completed");
                return;
            }

            // Si existe la carpeta de video, verificar si hay archivos de video
            string[] videoFiles = Directory.GetFiles(videoFolder, "*.mp4");
            if (videoFiles.Length == 0)
            {
                // Si no hay videos, borrar la carpeta vacía
                try
                {
                    Directory.Delete(videoFolder);
                    Debug.Log($"Deleted empty video folder: {videoFolder}");
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"Could not delete video folder {videoFolder}: {e.Message}");
                }
            }
            // Si hay videos, se dejarán para que se suban posteriormente
        }
        else
        {
            Debug.LogWarning($"❌ FAILED: Metrics upload: {fileName}");
            // Mantener el archivo para reintentar más tarde
        }
    }

    /// <summary>
    /// Uploads a video file using direct UnityWebRequest
    /// </summary>
    /// <param name="filePath">Path to the video file</param>
    private IEnumerator UploadVideoFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"Video file does not exist: {filePath}");
            yield break;
        }

        string fileName = Path.GetFileName(filePath);
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);

        Debug.Log($"=== STARTING VIDEO FILE UPLOAD ===");
        Debug.Log($"Video file: {fileName}");
        Debug.Log($"File path: {filePath}");

        long fileSize = 0;
        byte[] fileData = null;

        try
        {
            fileSize = new FileInfo(filePath).Length;
            fileData = File.ReadAllBytes(filePath);
            Debug.Log($"Video file read successfully: {fileData.Length} bytes");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error reading video file {filePath}: {e.Message}");
            yield break;
        }

        bool uploadCompleted = false;
        bool uploadSuccess = false;
        string responseText = "";

        // Usar la URL completa directamente para video upload
        string uploadUrl2 = "https://whitelabelvendingmachine.com/api/v1/assets/machine_auth/upload";
        string uploadUrl = URLdirectory.serverUrl + URLdirectory.videoUploadUrl;
        Debug.Log("ÑÑÑÑÑÑÑÑÑÑÑ " + uploadUrl);
        Debug.Log("ÑÑÑÑÑÑÑÑÑÑÑ2 " + uploadUrl2);

        Debug.Log($"Sending video file to: {uploadUrl}");

        // Crear WWWForm para el upload con todos los parámetros requeridos
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", fileData, fileName, "video/mp4");

        // Agregar todos los campos requeridos
        if (!string.IsNullOrEmpty(fileNameWithoutExtension))
        {
            form.AddField("offline_reference_id", fileNameWithoutExtension);
            Debug.Log($"Added offline_reference_id: {fileNameWithoutExtension}");
        }

        form.AddField("mode", "offline");
        form.AddField("entity_type", "game");

        Debug.Log($"Added fields: mode=offline, entity_type=game");

        // Print CURL command for debugging
        PrintVideoCurlCommand(uploadUrl, filePath, fileNameWithoutExtension);

        // Usar UnityWebRequest directamente - DEJAR QUE UNITY MANEJE LOS HEADERS AUTOMÁTICAMENTE
        using (UnityWebRequest webRequest = UnityWebRequest.Post(uploadUrl, form))
        {
            // SOLO agregar el header X-Machine-Key, NO Content-Type
            webRequest.SetRequestHeader("X-Machine-Key", GlobalVariables.machinesSecretKey);

            // Agregar logs de los headers
            Debug.Log($"Headers: X-Machine-Key: {GlobalVariables.machinesSecretKey}");
            Debug.Log($"Unity manejará automáticamente el Content-Type para multipart/form-data");

            // Configurar timeout más largo para videos
            webRequest.timeout = 120; // 2 minutos para videos grandes

            // Enviar la request
            yield return webRequest.SendWebRequest();

            // DEBUG: Verificar los headers que se enviaron realmente
            Debug.Log($"Request method: {webRequest.method}");
            Debug.Log($"Request content-type: {webRequest.uploadHandler?.contentType}");

            // Manejar la respuesta después del yield
            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Video upload error: {webRequest.error}");
                Debug.LogError($"Error response: {webRequest.downloadHandler?.text}");
                Debug.LogError($"Response code: {webRequest.responseCode}");
                responseText = null;
            }
            else
            {
                responseText = webRequest.downloadHandler.text;
                Debug.Log($"Video upload successful: {responseText}");
                Debug.Log($"Response code: {webRequest.responseCode}");
            }

            uploadCompleted = true;
            uploadSuccess = !string.IsNullOrEmpty(responseText);
        }

        // Manejar la respuesta del video
        if (uploadSuccess)
        {
            Debug.Log($"✅ SUCCESS: Video uploaded: {fileName}");
            SafeDeleteVideoAndFolder(filePath);
        }
        else
        {
            Debug.LogWarning($"❌ FAILED: Video upload: {fileName}");
        }

        Debug.Log($"=== VIDEO UPLOAD FINISHED ===");
        Debug.Log($"Completed: {uploadCompleted}, Success: {uploadSuccess}");
    }

    /// <summary>
    /// Prints a CURL command for video file upload
    /// </summary>
    private void PrintVideoCurlCommand(string url, string filePath, string offlineReferenceId)
    {
        try
        {
            string curlCommand = $"curl -X POST \\\n";
            curlCommand += $"  -H \"X-Machine-Key: {GlobalVariables.machinesSecretKey}\" \\\n";
            curlCommand += $"  -H \"Content-Type: multipart/form-data\" \\\n";
            curlCommand += $"  -F \"file=@\\\"{filePath}\\\";type=video/mp4\" \\\n";

            if (!string.IsNullOrEmpty(offlineReferenceId))
            {
                curlCommand += $"  -F \"offline_reference_id={offlineReferenceId}\" \\\n";
            }

            curlCommand += $"  -F \"mode=offline\" \\\n";
            curlCommand += $"  -F \"entity_type=game\" \\\n";
            curlCommand += $"  \"{url}\"";

            Debug.Log("=== VIDEO CURL COMMAND ===");
            Debug.Log(curlCommand);
            Debug.Log("=== END CURL COMMAND ===");

            // También imprimir una versión simplificada
            string simpleCurl = $"curl -X POST -H \"X-Machine-Key: {GlobalVariables.machinesSecretKey}\" -H \"Content-Type: multipart/form-data\" -F \"file=@{filePath};type=video/mp4\"";
            if (!string.IsNullOrEmpty(offlineReferenceId))
            {
                simpleCurl += $" -F \"offline_reference_id={offlineReferenceId}\"";
            }
            simpleCurl += $" -F \"mode=offline\" -F \"entity_type=game\" \"{url}\"";

            Debug.Log($"Simplified CURL: {simpleCurl}");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Failed to generate video CURL command: {e.Message}");
        }
    }

    /// <summary>
    /// Safely deletes a file
    /// </summary>
    /// <param name="filePath">Path to the file to delete</param>
    private void SafeDeleteFile(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Debug.Log($"Deleted file: {Path.GetFileName(filePath)}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error deleting file {filePath}: {e.Message}");
        }
    }

    /// <summary>
    /// Deletes video file and its containing folder, and the corresponding metrics and prize files
    /// </summary>
    /// <param name="videoFilePath">Path to the video file</param>
    private void SafeDeleteVideoAndFolder(string videoFilePath)
    {
        try
        {
            string videoFileName = Path.GetFileNameWithoutExtension(videoFilePath);
            string folderPath = Path.GetDirectoryName(videoFilePath);

            Debug.Log($"=== STARTING VIDEO CLEANUP ===");
            Debug.Log($"Video: {videoFileName}");
            Debug.Log($"Folder: {folderPath}");

            // Delete the video file
            SafeDeleteFile(videoFilePath);
            Debug.Log($"Video file deleted: {Path.GetFileName(videoFilePath)}");

            // Delete metrics file if it exists
            string metricsFilePath = Path.Combine(Application.persistentDataPath, videoFileName + ".txt");
            if (File.Exists(metricsFilePath))
            {
                SafeDeleteFile(metricsFilePath);
                Debug.Log($"Metrics file deleted: {Path.GetFileName(metricsFilePath)}");
            }
            else
            {
                Debug.Log($"Metrics file not found: {metricsFilePath}");
            }

            // Delete prize file if it exists
            string prizeFilePath = Path.Combine(Application.persistentDataPath, videoFileName + "prize.txt");
            if (File.Exists(prizeFilePath))
            {
                SafeDeleteFile(prizeFilePath);
                Debug.Log($"Prize file deleted: {Path.GetFileName(prizeFilePath)}");
            }
            else
            {
                Debug.Log($"Prize file not found: {prizeFilePath}");
            }

            // Delete the folder if it's empty
            if (Directory.Exists(folderPath))
            {
                // Check if folder is empty
                if (!Directory.GetFiles(folderPath).Any() && !Directory.GetDirectories(folderPath).Any())
                {
                    Directory.Delete(folderPath);
                    Debug.Log($"Empty folder deleted: {Path.GetFileName(folderPath)}");
                }
                else
                {
                    Debug.Log($"Folder not empty, keeping: {folderPath}");
                    string[] remainingFiles = Directory.GetFiles(folderPath);
                    string[] remainingDirs = Directory.GetDirectories(folderPath);
                    Debug.Log($"Remaining files: {remainingFiles.Length}, directories: {remainingDirs.Length}");
                }
            }
            else
            {
                Debug.Log($"Folder not found: {folderPath}");
            }

            Debug.Log($"=== VIDEO CLEANUP COMPLETED ===");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error deleting video and folder {videoFilePath}: {e.Message}");
        }
    }

    /// <summary>
    /// Updates the progress UI
    /// </summary>
    private void UpdateProgress(int current, int total)
    {
        if (progressSlider != null)
        {
            progressSlider.value = (float)current / total;
        }

        if (progressText != null)
        {
            progressText.text = $"{current}/{total} files uploaded";
        }
    }

    /// <summary>
    /// Updates the status text
    /// </summary>
    private void UpdateStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
        Debug.Log(message);
    }

    /// <summary>
    /// Checks if there are any offline files to upload (excluding system files)
    /// </summary>
    /// <returns>True if there are files to upload</returns>
    public bool HasOfflineFiles()
    {
        string persistentDataPath = Application.persistentDataPath;
        if (!Directory.Exists(persistentDataPath)) return false;

        string[] txtFiles = Directory.GetFiles(persistentDataPath, "*.txt");

        // Filter out system files
        foreach (string txtFile in txtFiles)
        {
            string fileName = Path.GetFileNameWithoutExtension(txtFile);
            if (fileName != "Player" && fileName != "Player-prev")
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Gets the count of offline files pending upload (excluding system files)
    /// </summary>
    /// <returns>Number of files to upload</returns>
    public int GetOfflineFileCount()
    {
        FindFilesToUpload();
        return filesToUpload.Count;
    }

    /// <summary>
    /// Gets a list of offline file names (for debugging or display)
    /// </summary>
    /// <returns>List of file names</returns>
    public List<string> GetOfflineFileNames()
    {
        List<string> fileNames = new List<string>();
        string persistentDataPath = Application.persistentDataPath;

        if (!Directory.Exists(persistentDataPath)) return fileNames;

        string[] txtFiles = Directory.GetFiles(persistentDataPath, "*.txt");

        foreach (string txtFile in txtFiles)
        {
            string fileName = Path.GetFileNameWithoutExtension(txtFile);
            if (fileName != "Player" && fileName != "Player-prev")
            {
                fileNames.Add(fileName);
            }
        }

        return fileNames;
    }
}