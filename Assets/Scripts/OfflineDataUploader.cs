using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;

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

            // Add the text file to upload list
            filesToUpload.Add(txtFile);

            // Check if there's a corresponding folder with video (not mandatory)
            string videoFolder = Path.Combine(persistentDataPath, fileNameWithoutExtension);
            if (Directory.Exists(videoFolder))
            {
                // Look for video file with same name in the folder
                string videoFile = Path.Combine(videoFolder, fileNameWithoutExtension + ".mp4");
                if (File.Exists(videoFile))
                {
                    filesToUpload.Add(videoFile);
                    Debug.Log($"Found video for {fileNameWithoutExtension}: {videoFile}");
                }
                else
                {
                    // If exact name not found, look for any .mp4 file in the folder
                    string[] videoFiles = Directory.GetFiles(videoFolder, "*.mp4");
                    if (videoFiles.Length > 0)
                    {
                        filesToUpload.Add(videoFiles[0]);
                        Debug.Log($"Found alternative video for {fileNameWithoutExtension}: {videoFiles[0]}");
                    }
                    else
                    {
                        Debug.Log($"No video found for {fileNameWithoutExtension}, uploading metrics only");
                    }
                }
            }
            else
            {
                Debug.Log($"No video folder found for {fileNameWithoutExtension}, uploading metrics only");
            }
        }

        Debug.Log($"Found {filesToUpload.Count} files to upload");
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

        UpdateStatus($"Uploading: {fileName}");

        if (fileExtension == ".txt")
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
    /// Uploads a metrics text file as JSON
    /// </summary>
    /// <param name="filePath">Path to the metrics file</param>
    private IEnumerator UploadMetricsFile(string filePath)
    {
        bool uploadCompleted = false;
        bool uploadSuccess = false;

        try
        {
            string jsonData = File.ReadAllText(filePath);
            string fileName = Path.GetFileName(filePath);

            // Create a custom callback to handle the response
            System.Action<string> callback = (response) =>
            {
                uploadCompleted = true;
                uploadSuccess = !string.IsNullOrEmpty(response);
                HandleMetricsUploadResponse(response, filePath, fileName);
            };

            // Send the metrics data
            HttpManager.AddRequestHeader("X-Machine-Key", GlobalVariables.machinesSecretKey);
            HttpManager.AddRequestHeader("Content-Type", "application/json");
            HttpManager.Post(URLdirectory.sendAnalitics + "?mode=offline", jsonData, callback);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error uploading metrics file {filePath}: {e.Message}");
            uploadCompleted = true;
            uploadSuccess = false;
        }

        // Wait for upload to complete or timeout
        float timeout = 10f; // 10 second timeout for metrics
        float timer = 0f;

        while (!uploadCompleted && timer < timeout)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if (!uploadCompleted)
        {
            Debug.LogWarning($"Metrics upload timeout: {Path.GetFileName(filePath)}");
        }
    }

    /// <summary>
    /// Handles the response from metrics upload - MODIFICADO PARA BORRAR ARCHIVOS TXT
    /// </summary>
    private void HandleMetricsUploadResponse(string response, string filePath, string fileName)
    {
        if (!string.IsNullOrEmpty(response))
        {
            Debug.Log($"Successfully uploaded metrics: {fileName}");

            // SIEMPRE borrar el archivo .txt después de un envío exitoso
            SafeDeleteFile(filePath);
            Debug.Log($"Deleted metrics file after successful upload: {fileName}");

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
            Debug.LogWarning($"Failed to upload metrics: {fileName}");
            // Mantener el archivo para reintentar más tarde
        }
    }

    /// <summary>
    /// Uploads a video file using VideoUpload component
    /// </summary>
    /// <param name="filePath">Path to the video file</param>
    private IEnumerator UploadVideoFile(string filePath)
    {
        if (videoUpload == null)
        {
            Debug.LogError("VideoUpload component not assigned");
            yield break;
        }

        string fileName = Path.GetFileName(filePath);

        // Create a flag to track upload completion
        bool uploadCompleted = false;
        bool uploadSuccess = false;

        // Store the original callback
        var originalCallback = videoUpload.GetType().GetField("HandleResponse",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(videoUpload) as System.Action<string>;

        // Create a new callback that sets our completion flag
        System.Action<string> uploadCallback = (response) =>
        {
            uploadCompleted = true;
            uploadSuccess = !string.IsNullOrEmpty(response);

            if (uploadSuccess)
            {
                Debug.Log($"Successfully uploaded video: {fileName}");
                // Delete video file and its folder after successful upload
                Debug.Log("Filepath " + filePath);
                SafeDeleteVideoAndFolder(filePath);
            }
            else
            {
                Debug.LogWarning($"Failed to upload video: {fileName}");
            }
        };

        // Use reflection to set the callback (since HandleResponse is private)
        var callbackField = videoUpload.GetType().GetField("HandleResponse",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (callbackField != null)
        {
            callbackField.SetValue(videoUpload, uploadCallback);
        }

        // Start the upload
        videoUpload.UploadToServer(filePath);

        // Wait for upload to complete
        float timeout = 30f; // 30 second timeout
        float timer = 0f;

        while (!uploadCompleted && timer < timeout)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // Restore original callback
        if (callbackField != null && originalCallback != null)
        {
            callbackField.SetValue(videoUpload, originalCallback);
        }

        if (!uploadCompleted)
        {
            Debug.LogWarning($"Video upload timeout: {fileName}");
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
    /// Deletes video file and its containing folder, and the corresponding metrics file if it exists
    /// </summary>
    /// <param name="videoFilePath">Path to the video file</param>
    private void SafeDeleteVideoAndFolder(string videoFilePath)
    {
        try
        {
            string videoFileName = Path.GetFileNameWithoutExtension(videoFilePath);
            string folderPath = Path.GetDirectoryName(videoFilePath);
            string metricsFilePath = Path.Combine(Application.persistentDataPath, videoFileName + ".txt");

            // Delete the video file
            SafeDeleteFile(videoFilePath);

            // El archivo .txt ya debería haber sido borrado en HandleMetricsUploadResponse
            // pero por si acaso, verificamos y lo borramos si todavía existe
            if (File.Exists(metricsFilePath))
            {
                SafeDeleteFile(metricsFilePath);
                Debug.Log($"Deleted corresponding metrics file: {Path.GetFileName(metricsFilePath)}");
            }

            // Delete the folder if it's empty
            if (Directory.Exists(folderPath))
            {
                // Check if folder is empty
                if (!Directory.GetFiles(folderPath).Any() && !Directory.GetDirectories(folderPath).Any())
                {
                    Directory.Delete(folderPath);
                    Debug.Log($"Deleted empty folder: {Path.GetFileName(folderPath)}");
                }
                else
                {
                    Debug.Log($"Folder not empty, keeping: {folderPath}");
                }
            }
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