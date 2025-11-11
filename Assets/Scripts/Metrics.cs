using System;
using UnityEngine;
using System.IO;

/// <summary>
/// Class responsible for sending metrics to the server.
/// </summary>
public class Metrics : MonoBehaviour
{
    /// <summary>
    /// The GameObject representing Panel 1 in the UI.
    /// </summary>
    public GameObject Panel1;

    /// <summary>
    /// The GameObject used for testing purposes.
    /// </summary>
    public GameObject PanelPruebas;

    /// <summary>
    /// Reference to the DressApp component managing outfit configurations.
    /// </summary>
    public DressApp dressApp;


    /// <summary>
    /// Send finals metrics to server each game when user clicked the home button.
    /// </summary>
    public void ReturnToMain()
    {
        if (GlobalVariables.offline)
        {
            GlobalVariables.metricsObj.time_completed = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            GlobalVariables.metricsObj.event_id = GlobalVariables.machineData.event_id;
            GlobalVariables.metricsObj.upload_mode = "offline";
            GlobalVariables.metricsObj.offline_reference_id = GlobalVariables.videoName;
            GlobalVariables.metricsObj.steps = new StepsWrapper(GlobalVariables.stepDictionary);
            string jsonBody = JsonUtility.ToJson(GlobalVariables.metricsObj, true);
            jsonBody = jsonBody.Replace("\"_1\"", "\"1\"");
            jsonBody = jsonBody.Replace("\"_2\"", "\"2\"");
            jsonBody = jsonBody.Replace("\"_3\"", "\"3\"");
            jsonBody = jsonBody.Replace("\"_4\"", "\"4\"");
            jsonBody = jsonBody.Replace("\"_5\"", "\"5\"");
            jsonBody = jsonBody.Replace("\"_6\"", "\"6\"");
            jsonBody = jsonBody.Replace("\"_7\"", "\"7\"");
            jsonBody = jsonBody.Replace("\"_8\"", "\"8\"");
            jsonBody = jsonBody.Replace("\"_9\"", "\"9\"");
            //Debug.Log("+++++++++++++++++++++++++++\n" + jsonBody + "\n++++++++++++++++++++++++++++++");
            SaveMetricsToFile(jsonBody);
        }
        else
        {
            GlobalVariables.metricsObj.time_completed = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            GlobalVariables.metricsObj.event_id = GlobalVariables.machineData.event_id;
            GlobalVariables.metricsObj.steps = new StepsWrapper(GlobalVariables.stepDictionary);
            string jsonBody = JsonUtility.ToJson(GlobalVariables.metricsObj, true);
            jsonBody = jsonBody.Replace("\"_1\"", "\"1\"");
            jsonBody = jsonBody.Replace("\"_2\"", "\"2\"");
            jsonBody = jsonBody.Replace("\"_3\"", "\"3\"");
            jsonBody = jsonBody.Replace("\"_4\"", "\"4\"");
            jsonBody = jsonBody.Replace("\"_5\"", "\"5\"");
            jsonBody = jsonBody.Replace("\"_6\"", "\"6\"");
            jsonBody = jsonBody.Replace("\"_7\"", "\"7\"");
            jsonBody = jsonBody.Replace("\"_8\"", "\"8\"");
            jsonBody = jsonBody.Replace("\"_9\"", "\"9\"");
            Debug.Log("+++++++++++++++++++++++++++\n" + jsonBody + "\n++++++++++++++++++++++++++++++");
            HttpManager.AddRequestHeader("X-Machine-Key", GlobalVariables.machinesSecretKey);
            HttpManager.AddRequestHeader("Content-Type", "application/json");
            HttpManager.Post(URLdirectory.sendAnalitics, jsonBody, AnalyticsComplete);
        }
        ForceGarbageCollection();
        dressApp.DisableAllToMain();
        Panel1.SetActive(true);
        Panel1.GetComponent<Panel1Content>().panelInit();
    }

    /// <summary>
    /// Saves metrics to a text file when in offline mode
    /// </summary>
    /// <param name="jsonBody">JSON data to save</param>
    private void SaveMetricsToFile(string jsonBody)
    {
        try
        {
            string fileName = GlobalVariables.videoName + ".txt";
            ExternalDriveSelector.EnsureValidPath();
            string filePath = Path.Combine(GlobalVariables.pathHDD, fileName);

            // Create directory if it doesn't exist
            string directory = Path.GetDirectoryName(filePath);
            //Debug.Log(directory);
            //Debug.Log(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Write JSON data to file
            File.WriteAllText(filePath, jsonBody);
            //Debug.Log($"Metrics saved to: {filePath}");

            // Call AnalyticsComplete to handle any additional offline logic
            AnalyticsComplete("OFFLINE_METRICS_SAVED");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving metrics to file: {e.Message}");
            // Even if saving fails, continue with the flow
            AnalyticsComplete("OFFLINE_METRICS_SAVE_ERROR");
        }
    }

    /// <summary>
    /// Sent to server if user allowed to use video.
    /// </summary>
    /// <param name="response"></param>
    private void AnalyticsComplete(string response)
    {
        //Debug.Log("--------------------------------\n" + response + "\n----------------------------------");

        // Only send allowance data if we're online and it has changed
        if (!GlobalVariables.offline && GlobalVariables.allowedChanged)
        {
            string jsonBody = "{\"share_allowance\": " + GlobalVariables.permission.ToString().ToLower() + "}";
            HttpManager.AddRequestHeader("X-Machine-Key", GlobalVariables.machinesSecretKey);
            HttpManager.AddRequestHeader("Content-Type", "application/json");
            HttpManager.Patch(URLdirectory.sendAllowed + GlobalVariables._id + "/share-allowance", jsonBody, AnalyticsFinish);
        }
        else if (GlobalVariables.offline)
        {
            //Debug.Log("Offline mode - Metrics saved locally: " + response);
            // Optionally handle any offline-specific completion logic here
            AnalyticsFinish("OFFLINE_OPERATION_COMPLETE");
        }
    }

    /// <summary>
    /// Callback Print to screen the response of server when send metrics.
    /// </summary>
    /// <param name="response"></param>
    private void AnalyticsFinish(string response)
    {
        //Debug.Log("--------------------------------\n" + response + "\n----------------------------------");
    }

    /// <summary>
    /// Get the total time in this screen.
    /// </summary>
    /// <param name="metric"></param>
    /// <param name="time"></param>
    public void timeInScreen(string metric, float time)
    {
        //string postUrl = "https://jsonplaceholder.typicode.com/posts";
        //string jsonData = "{\"title\":\"" + "Time in screen" + "\",\"body\":\"" + metric + "\",\"userId\":" + time + "}";
    }

    public void ForceGarbageCollection()
    {
        Debug.Log("Iniciando recolección de basura manual...");
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
        Debug.Log("Recolección de basura completada.");
    }
}