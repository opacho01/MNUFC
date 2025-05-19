using System;
using UnityEngine;

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
        
        dressApp.DisableAllToMain();
        Panel1.SetActive(true);
        Panel1.GetComponent<Panel1Content>().panelInit();
    }

    /// <summary>
    /// Sent to server if user allowe to use video.
    /// </summary>
    /// <param name="response"></param>
    private void AnalyticsComplete(string response)
    {
        //Debug.Log("--------------------------------\n" + response + "\n----------------------------------");
        if (GlobalVariables.allowedChanged)
        {
            string jsonBody = "{\"share_allowance\": " + GlobalVariables.permission.ToString().ToLower() + "}";
            HttpManager.AddRequestHeader("X-Machine-Key", GlobalVariables.machinesSecretKey);
            HttpManager.AddRequestHeader("Content-Type", "application/json");
            HttpManager.Patch(URLdirectory.sendAllowed + GlobalVariables._id + "/share-allowance", jsonBody, AnalyticsFinish);
        }
    }

    /// <summary>
    /// Callback Print to screen the response of server when send metrics.
    /// </summary>
    /// <param name="response"></param>
    private void AnalyticsFinish(string response)
    {
        Debug.Log("--------------------------------\n" + response + "\n----------------------------------");
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
}
