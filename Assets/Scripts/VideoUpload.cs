using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Collections;

/// <summary>
/// Class responsible for uploading the player's generated video to the server
/// </summary>
public class VideoUpload : MonoBehaviour
{
    /// <summary>
    /// The main panel content manager.
    /// </summary>
    public PanelContent panelContent;

    /// <summary>
    /// Handles webcam recording functionality.
    /// </summary>
    public WebCamRecorder webCamRec;

    /// <summary>
    /// Auxiliary file path used for temporary storage or processing.
    /// </summary>
    public string filepathAux = "";

    /// <summary>
    /// The number of attempts made for a specific operation.
    /// </summary>
    private byte attemps = 0;


    /// <summary>
    /// call the coorutine PostVideo and reinitialize attemps to upload to server
    /// </summary>
    /// <param name="path">Path of the video to upload to server</param>
    public void UploadToServer(string path)
    {
        Debug.Log("Uploading video to server: " + path);
        StartCoroutine(PostVideo(path));
        attemps = 0;
    }

    /// <summary>
    /// Routine to upload the player's video to the server via PostFile
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    IEnumerator PostVideo(string filePath) {
        attemps++;
        videoUp = new VideoUploadResponse();
        filepathAux = filePath;
        if (!File.Exists(filePath))
        {
            Debug.LogError($"The file does not exist in the path: {filePath}");
            yield break;
        }

        byte[] videoData = File.ReadAllBytes(filePath);
        string uploadUrl = URLdirectory.videoUploadUrl;
        HttpManager.PostFile(uploadUrl, filePath, "video/mp4", HandleResponse);
    }
    /// <summary>
    /// Handles the generation and management of QR codes.
    /// </summary>
    public QRGenerator qrGenerator;

    /// <summary>
    /// Stores the response details after a video upload operation.
    /// </summary>
    public VideoUploadResponse videoUp;



    /// <summary>
    /// Callback from PostVideo if upload successfull delete video and call the next panel,
    /// else it makes 3 attempts to upload the video before advancing to the next panel.
    /// </summary>
    /// <param name="response"></param>
    void HandleResponse(string response)
    {
        Debug.Log("-----------" + response);
        if (!string.IsNullOrEmpty(response))
        {
            videoUp = JsonUtility.FromJson<VideoUploadResponse>(response);
            GlobalVariables.videoUpload = videoUp;
            webCamRec.DeleteVideoFile(filepathAux);
            panelContent.nextPanel();
        }
        else
        {
            if (attemps <= 3)
            {
                StopCoroutine(PostVideo(filepathAux));
                StartCoroutine(PostVideo(filepathAux));
            } else
            {
                DontVideoUploaded();
            }
        }
    }


    public void DontVideoUploaded()
    {
        videoUp = new VideoUploadResponse();
        videoUp.s3_url = "https://fandomprizemachine.com/?eventId=68c08cc6aa5a1fc56fdf684b&url=https://vendingmachine-assets-archive.s3.us-east-1.amazonaws.com/uploads/20251010_012215_" + filepathAux;
        videoUp.message = "Video no subido después de múltiples intentos";
        videoUp.file_key = "local_fallback";
        videoUp.asset_id = "fallback_asset";

        GlobalVariables.videoUpload = videoUp;
        panelContent.nextPanel();
    }
}
