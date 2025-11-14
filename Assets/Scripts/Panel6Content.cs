using System;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

/// <summary>
/// Class in charge of managing the Panel Celebration.
/// </summary>
public class Panel6Content : PanelContent
{
    /// <summary>
    /// The team logo displayed on the panel.
    /// </summary>
    public RawImage teamLogo;
    /// <summary>
    /// The home button displayed on the panel.
    /// </summary>
    public RawImage houseButton;
    /// <summary>
    /// The VideoPlayer component that plays the background video.
    /// </summary>
    public VideoPlayer background;
    /// <summary>
    /// The team puppets displayed on the panel.
    /// </summary>
    public Image[] teamPuppets;
    /// <summary>
    /// The title text displayed on the panel.
    /// </summary>
    public TMP_Text title;
    /// <summary>
    /// The background image for the disclaimer section.
    /// </summary>
    public RawImage DisclaimerBack;
    /// <summary>
    /// The animated puppet components used in the panel.
    /// </summary>
    public AnimatorImage[] puppets;
    /// <summary>
    /// Tracks whether the panel has been activated for the first time.
    /// </summary>
    public bool FirstActivated = false;

    /// <summary>
    /// On enable play the background video.
    /// </summary>
    private void OnEnable()
    {
        background.Play();
    }

    /// <summary>
    /// Initialize panel, textures, video, audios, colors, text, animation of puppets.
    /// </summary>
    /// <param name="allResources"></param>
    public override void fillPanel(GetAllResources allResources)
    {
        title.text = allResources.themeData.step_7_celebration_screen.celebration_message_text;
        title.color = GlobalVariables.colorSecondary;
        houseButton.texture = allResources.houseButton;
        DisclaimerBack.texture = allResources.footerBanner;
        audios.clip = allResources.screenAudioP7;
        allResources.PlayVideo(allResources.backgroundP7, background, gameObject);//, !videoPrepared);
        for (byte i = 0; i < puppets.Length; i++)
        {
            puppets[i].initAnim();
        }
    }

    /// <summary>
    /// Set actual screen to 7, call gerReward, play audio, animations, deactivate the last panel.
    /// </summary>
    public override void panelInit()
    {
        GlobalVariables.ActualScreen = 7;
        getReward();
        audios.Play();
        title.gameObject.SetActive(true);
        for (byte i = 0; i < puppets.Length; i++)
        {
            puppets[i].animPuppet();
        }
        antPanelObj.gameObject.SetActive(false);
    }
    public PrizeManager prizeManager;
    /// <summary>
    /// Assemble json to make a getReward post to server.
    /// </summary>
    public void getReward()
    {
        string uploadUrl = URLdirectory.rewardUrl;

        //Debug.Log("------" + jsonBody);
        if (GlobalVariables.offline)
        {
            Prize selectedPrize = prizeManager.GetRandomPrizeAndDecrement();
            GlobalVariables.selectedPrize = selectedPrize;
            /*if (selectedPrize != null)
            {
                Debug.Log($"Selected prize: {selectedPrize.showName}");
                Debug.Log($"Slot ID: {selectedPrize.slot_id}");
                Debug.Log($"Probability: {selectedPrize.probabilityWeight}");
                Debug.Log($"Stock: {selectedPrize.inStock}");
                Debug.Log($"Quantity: {selectedPrize.quantity}");

                // Use the selected prize...
            }*/

            // Print only the fields that are not commented in the JSON
            /*Debug.Log("\"start_time\": \"" + GlobalVariables.metricsObj.time_start + "\",\n");
            Debug.Log("\"end_time\": \"" + DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz") + "\",\n");
            Debug.Log("\"total_game_time\": " + 1 + ",\n");
            Debug.Log("\"total_screen_time\": " + 1 + ",\n");
            Debug.Log("\"date\": \"" + DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz") + "\",\n");
            Debug.Log("\"event_id\": \"" + GlobalVariables.machineData.event_id + "\",\n");
            Debug.Log("\"machine_id\": \"" + GlobalVariables.machineData._id + "\",\n");
            Debug.Log("\"total_clicks\": " + 0 + ",\n");
            Debug.Log("\"access_code\": \"" + GlobalVariables.videoName + "\",\n");
            Debug.Log("\"offline_game_id\": \"" + GlobalVariables.videoName + "\",\n");
            Debug.Log("\"upload_mode\": \"" + "offline" + "\",\n");
            Debug.Log("\"is_highlight\":false," + "\n");
            Debug.Log("\"prize_info\":{");
            Debug.Log("\"slot_id\": \"" + selectedPrize.slot_id + "\",\n");
            Debug.Log("\"name\":\"r0c1\",");
            Debug.Log("\"showName\": \"" + selectedPrize.showName + "\",\n");
            Debug.Log("\"price\":0.0,");
            Debug.Log("\"rewardName\":\"Reward Name\",");
            Debug.Log("\"probabilityWeight\": " + selectedPrize.probabilityWeight + ",\n");
            Debug.Log("\"inStock\": " + selectedPrize.inStock.ToString().ToLower() + ",\n");
            Debug.Log("\"quantity\": " + selectedPrize.quantity + "},\n");
            Debug.Log("}");*/

            string jsonBodyOffline =
           "{\n"
           + "\"start_time\": \"" + GlobalVariables.metricsObj.time_start + "\",\n"
           + "\"end_time\": \"" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") + "\",\n"
           + "\"total_game_time\": " + 1 + ",\n"
           + "\"total_screen_time\": " + 1 + ",\n"
           + "\"date\": \"" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") + "\",\n"
           + "\"event_id\": \"" + GlobalVariables.machineData.event_id + "\",\n"
           + "\"machine_id\": \"" + GlobalVariables.machineData._id + "\",\n"
           + "\"total_clicks\": " + 0 + ",\n"
           + "\"access_code\": \"" + GlobalVariables.videoName + "\",\n"
           + "\"offline_game_id\": \"" + GlobalVariables.videoName + "\",\n"
           + "\"upload_mode\": \"" + "offline" + "\",\n"
           + "\"is_highlight\":false," + "\n"
           + "\"s3_video_url\": \"" + GlobalVariables.videoUpload.s3_url + "\",\n"
           + "\"public_web_url\": " + "\"https://app.myvendingmachine.com/game/77980a15-4684-4b7c-b61d-9a990cc8eaa3" + "\",\n"
           + "\"share_allowance\":false," + "\n"
           + "\"final_url\":" + "\"https://www.fandomprizemachine.com/" + "\",\n"
           + "\"_id\": \"" + GlobalVariables.machineData.event_id + "\",\n"
           + "\"prize_info\":{"
           + "\"slot_id\": \"" + selectedPrize.slot_id + "\",\n"
           + "\"name\": \"" + selectedPrize.name + "\",\n"
           + "\"showName\": \"" + selectedPrize.showName + "\",\n"
           + "\"price\":0.0,"
           + "\"rewardName\":\"Reward Name\","
           + "\"probabilityWeight\": " + selectedPrize.probabilityWeight + ",\n"
           + "\"inStock\": " + selectedPrize.inStock.ToString().ToLower() + ",\n"
           + "\"quantity\": " + selectedPrize.quantity + "}\n"
        + "}";

            //Debug.Log(jsonBodyOffline);

            // Save JSON to text file
            SaveJsonToFile(jsonBodyOffline);

            HandlePostResponse(jsonBodyOffline);
        }
        else
        {
            string jsonBody =
            "{\n"
            + "\"start_time\": \"" + GlobalVariables.metricsObj.time_start + "\",\n"
            + "\"end_time\": \"" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") + "\",\n"
            + "\"total_game_time\": " + 1 + ",\n"
            + "\"total_screen_time\": " + 1 + ",\n"
            + "\"date\": \"" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") + "\",\n"
            + "\"event_id\": \"" + GlobalVariables.machineData.event_id + "\",\n"
            + "\"machine_id\": \"" + GlobalVariables.machineData._id + "\",\n"
            + "\"total_clicks\": " + 0 + ",\n"
            + "\"s3_video_url\": \"" + GlobalVariables.videoUpload.s3_url + "\"\n"
        + "}";
            HttpManager.AddRequestHeader("X-Machine-Key", GlobalVariables.machinesSecretKey);
            HttpManager.AddRequestHeader("Content-Type", "application/json");
            HttpManager.Post(uploadUrl, jsonBody, HandlePostResponse);
        }
    }

    /// <summary>
    /// Saves JSON to a text file with the name GlobalVariables.videoName + "prize.txt"
    /// </summary>
    /// <param name="jsonContent">JSON content to save</param>
    private void SaveJsonToFile(string jsonContent)
    {
        try
        {
            // Create file name
            string fileName = GlobalVariables.videoName + "prize.txt";
            ExternalDriveSelector.EnsureValidPath();
            string filePath = Path.Combine(GlobalVariables.pathHDD, fileName);

            // Write JSON content to file
            File.WriteAllText(filePath, jsonContent);

            //Debug.Log($"File saved successfully: {filePath}");
            //Debug.Log($"Saved content: {jsonContent}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error saving JSON file: {e.Message}");
        }
    }

    /// <summary>
    /// Handles socket communication with the server, managing requests and responses.
    /// </summary>
    public SocketClient socket;
    /// <summary>
    /// Manages QR code generation for displaying event-related links.
    /// </summary>
    public QRGenerator qrGenerator;

    /// <summary>
    /// Await the response of getReward post, and generate QR, sendPrize to vending machine, and call waitToChange.
    /// </summary>
    /// <param name="response"></param>
    void HandlePostResponse(string response)
    {
       // Debug.Log(response);
        if (!string.IsNullOrEmpty(response))
        {
            InfoPrize infoPrize = JsonUtility.FromJson<InfoPrize>(response);
            qrGenerator.setURLtoQR(infoPrize.final_url);
            GlobalVariables._id = infoPrize._id;
            GlobalVariables.infoPrize = infoPrize;
            socket.SendPrize(infoPrize.prize.name, infoPrize.prize.slot_id);
            StartCoroutine(waitToChange());
        }
        else
        {
            Debug.LogError("The POST request failed or received no response.");
        }
    }

    /// <summary>
    /// Cooroutine call nextPanel whit delay.
    /// </summary>
    private IEnumerator waitToChange()
    {
        nextPanelObj.gameObject.SetActive(true);
        yield return new WaitForSeconds(5);
        nextPanel();
    }

    /// <summary>
    /// Init the next panel and disable this panel.
    /// </summary>
    public override void nextPanel()
    {
        nextPanelObj.panelInit();
        gameObject.SetActive(false);
    }
}
