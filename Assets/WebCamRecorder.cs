using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;

/// <summary>
/// Class in charge of recording the player through the webcam and saving the video on the machine.
/// </summary>
public class WebCamRecorder : MonoBehaviour
{
    /// <summary>
    /// Displays the real-time webcam feed on the screen.
    /// </summary>
    public RawImage rawImage;

    /// <summary>
    /// Total duration of the recording session, obtained from the theme settings.
    /// </summary>
    public int recordingDuration = 5;

    /// <summary>
    /// Captures and streams live video from the webcam.
    /// </summary>
    private WebCamTexture webCamTexture;

    /// <summary>
    /// A render texture used for processing and displaying the webcam feed.
    /// </summary>
    public RenderTexture renderTexture;

    /// <summary>
    /// Manages content for Panel 5, handling UI and interactions.
    /// </summary>
    public Panel5Content panelContent;

    /// <summary>
    /// Stores and manages all resources required by the application.
    /// </summary>
    public GetAllResources allResources;


    string timestamp = "";

    /// <summary>
    /// Check if webcam exist prepared them and call the RecordVideo.
    /// </summary>
    public void RecorderStart()
    {
        // Prepare the webcam
        if (WebCamTexture.devices.Length > 0)
        {
            webCamTexture = new WebCamTexture();
            rawImage.texture = webCamTexture;
        }
        else
        {
            UnityEngine.Debug.LogError("No available cameras were detected.");
        }
        timestamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        panelContent.nextPanelObj.gameObject.SetActive(true);
        StartCoroutine(RecordVideo());
    }


    /// <summary>
    /// Stop using the webcam to avoid errors
    /// </summary>
    void OnApplicationQuit()
    {
        // Liberar recursos al cerrar la aplicación
        webCamTexture?.Stop();
        StopCoroutine(RecordVideo());
        if (webCamTexture != null)
        {
            webCamTexture.Stop();
        }
    }

    private Process ffmpegProcess;

    private string ffmpegPath;

    private string outputFolderPath;

    private int offsetX;

    private int offsetY;


    /// <summary>
    /// Initialize and play the texture of webcam, create the directory to contain the video.
    /// </summary>
    public void initNewRecord()
    {
        webCamTexture.Play();
        ffmpegPath = Path.Combine(Application.streamingAssetsPath, "ffmpeg.exe");
        string PathName = $"output_{timestamp}";
        outputFolderPath = Path.Combine(Application.persistentDataPath, PathName);//Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "VideoFolder");
        if (!Directory.Exists(outputFolderPath))
        {
            Directory.CreateDirectory(outputFolderPath);
        }
        panelContent.ActivateNext();
    }

    /// <summary>
    /// Send the necessary parameters to ffmpegProcess to configure webcam recording.
    /// Initialize the ffmpegProcess.
    /// </summary>
    public void StartRecordingNew()
    {
        initNewRecord();
        Cursor.visible = true;
        Resolution currentResolution = Screen.currentResolution;
        string arg = $"{currentResolution.width}x{currentResolution.height}";
        string videoFileName = $"output_{timestamp}.mp4";
        string path = videoFileName;
        string text = Path.Combine(outputFolderPath, path);
        //Set the parameter of webcam, framerate, compression, quality and name to save.
        string arguments = $"-y -f gdigrab -framerate 30 -offset_x {offsetX} -offset_y {offsetY} -video_size {arg} " + "-i desktop -draw_mouse 0 -b:v 50000k -c:v libx264 -preset ultrafast -crf 12 -pix_fmt yuv420p -profile:v high -g 120 -tune film \"" + text + "\"";
        ffmpegProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = ffmpegPath,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                CreateNoWindow = true
            }
        };
        ffmpegProcess.Start();
        ffmpegProcess.BeginOutputReadLine();
        ffmpegProcess.BeginErrorReadLine();
    }

    /// <summary>
    /// Stop recording, call the initialization of the next panel, save the video and call the UploadToServer function.
    /// </summary>
    public void StopRecording()
    {
        panelContent.nextPanel();
        panelContent.gameObject.SetActive(false);
        if (ffmpegProcess == null || ffmpegProcess.HasExited)
        {
            return;
        }
        ffmpegProcess.StandardInput.WriteLine("q");
        ffmpegProcess.StandardInput.Close();
        ffmpegProcess.WaitForExit();
        if (ffmpegProcess.ExitCode == 0)
        {
            string path = $"output_{timestamp}.mp4";
            string text = Path.Combine(outputFolderPath, path);
            if (File.Exists(text))
            {
                if (!Application.isEditor)
                {
                    UnityEngine.Debug.Log("Recording stopped and video saved successfully at: " + text);
                }
                Cursor.visible = true;
                GetComponent<VideoUpload>().UploadToServer(text);
                webCamTexture?.Stop();
                StopCoroutine(RecordVideo());
            }
            else
            {
                UnityEngine.Debug.LogError("Recording stopped, but the video file was not found. " + text);
            }
        }
        else
        {
            UnityEngine.Debug.LogError($"Recording stopped, but there was an error saving the video. Exit code: {ffmpegProcess.ExitCode}");
        }
    }

    /// <summary>
    /// If the path exist delete the directory and video from machine.
    /// </summary>
    /// <param name="path">The path form video to delete</param>
    public void DeleteVideoFile(string path)
    {
        string text = Path.Combine(outputFolderPath, path);
        if (File.Exists(text))
        {
            try
            {
                string aux = text.Substring(0, text.LastIndexOf("\\"));
                File.Delete(text);
                Directory.Delete(aux, true);
                return;
            }
            catch (IOException ex)
            {
                UnityEngine.Debug.LogError("Error deleting video file: " + ex.Message);
                return;
            }
        }
        UnityEngine.Debug.LogWarning("Video file not found, cannot delete: " + text);
    }

    /// <summary>
    /// Coorutine asign the duration of video, start recording, await the duration and stop recording.
    /// </summary>
    /// <returns></returns>
    private IEnumerator RecordVideo()
    {
        float seconds = allResources.themeData.step_5_camera_record.video_duration_seconds;
        StartRecordingNew();
        yield return new WaitForSeconds(seconds);

        StopRecording();
    }

    /// <summary>
    /// On disable call the QuitCam funtion.
    /// </summary>
    private void OnDisable()
    {
        QuitCam();
    }

    /// <summary>
    /// Stop record and stop the webcam to prevent errors.
    /// </summary>
    public void QuitCam()
    {
        StopCoroutine(RecordVideo());
        if (webCamTexture != null)
        {
            webCamTexture.Stop();
        }
    }
}
