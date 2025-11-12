using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using System; // Importante para System.GC

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
            // Detener el existente antes de crear uno nuevo si ya existía
            webCamTexture?.Stop();
            // Asegurarse de liberar la memoria de la textura anterior si se descartara.
            // En este caso se reutiliza, pero es buena práctica.
            if (webCamTexture != null) Destroy(webCamTexture);

            webCamTexture = new WebCamTexture();
            rawImage.texture = webCamTexture;
        }
        else
        {
            UnityEngine.Debug.LogError("No available cameras were detected.");
        }
        timestamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        GlobalVariables.videoName = Base62DateConverter.ConvertDateToBase36(timestamp);
        panelContent.nextPanelObj.gameObject.SetActive(true);
        StartCoroutine(RecordVideo());
    }


    /// <summary>
    /// Stop using the webcam and stop any active FFmpeg process to avoid errors when the application quits.
    /// </summary>
    void OnApplicationQuit()
    {
        // Liberar recursos al cerrar la aplicación
        QuitCam();

        // **[CRÍTICO]** Si el proceso sigue vivo al cerrar, límpialo.
        if (ffmpegProcess != null && !ffmpegProcess.HasExited)
        {
            try
            {
                ffmpegProcess.Kill();
                ffmpegProcess.Dispose();
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError("Error al intentar terminar y disponer el proceso FFmpeg en Quit: " + ex.Message);
            }
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
        string PathName = GlobalVariables.videoName;
        ExternalDriveSelector.EnsureValidPath();
        outputFolderPath = Path.Combine(GlobalVariables.pathHDD, PathName);//Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "VideoFolder");
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
        string videoFileName = GlobalVariables.videoName + ".mp4";
        GetComponent<VideoUpload>().filepathAux = videoFileName;
        string path = videoFileName;
        string text = Path.Combine(outputFolderPath, path);

        // Si hay un proceso anterior que no fue limpiado, liberarlo
        if (ffmpegProcess != null)
        {
            ffmpegProcess.Dispose();
            ffmpegProcess = null;
        }

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

        try
        {
            ffmpegProcess.Start();
            ffmpegProcess.BeginOutputReadLine();
            ffmpegProcess.BeginErrorReadLine();
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("Error al iniciar FFmpeg: " + e.Message);
        }
    }

    /// <summary>
    /// Stop recording, call the initialization of the next panel, save the video and call the UploadToServer function.
    /// </summary>
    public void StopRecording()
    {
        // Asegúrate de que no haya múltiples StopRecording activos si el usuario presiona algo varias veces
        StopCoroutine(RecordVideo());

        panelContent.nextPanel();
        panelContent.gameObject.SetActive(false);

        // Comprobación de proceso y limpieza de recursos
        if (ffmpegProcess == null || ffmpegProcess.HasExited)
        {
            // Si el proceso ya no existe, no hay nada que limpiar de FFmpeg.
            return;
        }

        // Asegura que cerramos el flujo de entrada antes de enviar 'q'
        try
        {
            ffmpegProcess.StandardInput.WriteLine("q");
            ffmpegProcess.StandardInput.Close(); // CRÍTICO: Cierra el System.IO.Stream
            ffmpegProcess.WaitForExit(5000); // Esperar 5 segundos máximo.
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError("Error al intentar detener FFmpeg: " + ex.Message);
        }


        if (ffmpegProcess.HasExited && ffmpegProcess.ExitCode == 0)
        {
            string path = GlobalVariables.videoName + ".mp4";
            string text = Path.Combine(outputFolderPath, path);

            // 1. LIMPIEZA DE C# (GARBAGE COLLECTION)
            // Realizamos la limpieza tras una operación pesada de I/O
            RunManualCleanupCycle();

            UnityEngine.Debug.Log("Recording stopped and video saved successfully at: " + text);

            if (File.Exists(text))
            {
                if (!Application.isEditor)
                {
                    UnityEngine.Debug.Log("Recording stopped and video saved successfully at: " + text);
                }
                Cursor.visible = true;
                if (!GlobalVariables.offline)
                {
                    GetComponent<VideoUpload>().UploadToServer(text);
                }
                else
                {
                    GetComponent<VideoUpload>().DontVideoUploaded();
                }
                webCamTexture?.Stop();
                // OJO: La corrutina se detuvo al inicio de StopRecording, no es necesario StopCoroutine aquí.
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

        // **[CRÍTICO]** 2. LIBERACIÓN DE RECURSOS NATIVOS DEL PROCESO
        // Dispose del objeto Process. Esto libera todos los manejadores del SO.
        try
        {
            ffmpegProcess.Dispose();
            ffmpegProcess = null; // Marcar como nulo para evitar uso accidental
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError("Error al disponer el proceso FFmpeg: " + ex.Message);
        }
    }

    /// <summary>
    /// Forzar el ciclo de limpieza de recursos gestionados y no gestionados.
    /// </summary>
    private void RunManualCleanupCycle()
    {
        // 1. Libera memoria de GPU/Unity (texturas, meshes, etc.)
        Resources.UnloadUnusedAssets();

        // 2. Fuerza la recolección de basura de C# para liberar la memoria gestionada.
        GC.Collect();
    }


    /// <summary>
    /// If the path exist delete the directory and video from machine.
    /// </summary>
    /// <param name="path">The path form video to delete</param>
    public void DeleteVideoFile(string path)
    {
        string text = "";
        if (GlobalVariables.offline)
        {

            string PathName = GlobalVariables.videoName;
            ExternalDriveSelector.EnsureValidPath();
            outputFolderPath = Path.Combine(GlobalVariables.pathHDD, PathName);
            text = Path.Combine(outputFolderPath, path);
            UnityEngine.Debug.Log("Offline " + text);
            UnityEngine.Debug.Log(outputFolderPath);
            UnityEngine.Debug.Log(PathName);
        }
        else
        {
            text = Path.Combine(outputFolderPath, path);
            UnityEngine.Debug.Log("Online " + text);
        }
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
        StopCoroutine("RecordVideo"); // Usar la versión de string para mayor seguridad al detener
        if (webCamTexture != null)
        {
            webCamTexture.Stop();
        }

        // También intentar limpiar el proceso si está activo
        if (ffmpegProcess != null && !ffmpegProcess.HasExited)
        {
            try
            {
                ffmpegProcess.Kill();
                ffmpegProcess.Dispose();
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError("Error al intentar terminar y disponer el proceso FFmpeg en QuitCam: " + ex.Message);
            }
        }
    }
}