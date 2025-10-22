using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;
using System;
using System.Collections;

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
                if (selectedPrize != null)
                {
                    Debug.Log($"Premio seleccionado: {selectedPrize.showName}");
                    Debug.Log($"Slot ID: {selectedPrize.slot_id}");
                    Debug.Log($"Probabilidad: {selectedPrize.probabilityWeight}");
                    Debug.Log($"Stock: {selectedPrize.inStock}");
                    Debug.Log($"Cantidad: {selectedPrize.quantity}");

                    // Usar el premio seleccionado...
                }
            Debug.Log("\"start_time\": \"" + GlobalVariables.metricsObj.time_start + "\",\n");
        Debug.Log("\"end_time\": \"" + DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz") + "\",\n");
            Debug.Log("\"total_game_time\": " + 1 + ",\n");
            Debug.Log("\"total_screen_time\": " + 1 + ",\n");
            Debug.Log("\"date\": \"" + DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz") + "\",\n");
            Debug.Log("\"event_id\": \"" + GlobalVariables.machineData.event_id + "\",\n");
            Debug.Log("\"machine_id\": \"" + GlobalVariables.machineData._id + "\",\n");
            Debug.Log("\"total_clicks\": " + 0 + ",\n");
            Debug.Log("\"s3_video_url\": \"" + GlobalVariables.videoUpload.s3_url + "\",\n");

            Debug.Log("\"public_web_url\": " + "\"https://app.myvendingmachine.com/game/77980a15-4684-4b7c-b61d-9a990cc8eaa3" + "\",\n");
            Debug.Log("\"is_highlight\":false," + "\n");
            Debug.Log("\"share_allowance\":false," + "\n");
            Debug.Log("\"prize\":{");
            Debug.Log("\"slot_id\": \"" + selectedPrize.slot_id + "\",\n");
            Debug.Log("\"name\":\"r0c1\",");
            Debug.Log("\"showName\": \"" + selectedPrize.showName + "\",\n");
            Debug.Log("\"price\":0.0,");
            Debug.Log("\"rewardName\":\"Reward Name\",");
            Debug.Log("\"probabilityWeight\": " + selectedPrize.probabilityWeight + ",\n");
            Debug.Log("\"inStock\": " + selectedPrize.inStock.ToString().ToLower() + ",\n");
            Debug.Log("\"quantity\": " + selectedPrize.quantity + "},\n");
            Debug.Log("\"final_url\":" + "\"https://fandomprizemachine.com/?eventId=68c08cc6aa5a1fc56fdf684b&url=https://fandomprizemachine.com/?eventId=68c08cc6aa5a1fc56fdf684b&url=https://vendingmachine-assets-archive.s3.us-east-1.amazonaws.com/uploads/20251010_012215_" + "\",\n");
            Debug.Log("\"_id\":\"68e86a1907146812d88cd88a" + "\"\n");
            Debug.Log("}");
            string jsonBodyOffline =
       "{\n"
       + "\"start_time\": \"" + GlobalVariables.metricsObj.time_start + "\",\n"
       + "\"end_time\": \"" + DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz") + "\",\n"
       + "\"total_game_time\": " + 1 + ",\n"
       + "\"total_screen_time\": " + 1 + ",\n"
       + "\"date\": \"" + DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz") + "\",\n"
       + "\"event_id\": \"" + GlobalVariables.machineData.event_id + "\",\n"
       + "\"machine_id\": \"" + GlobalVariables.machineData._id + "\",\n"
       + "\"total_clicks\": " + 0 + ",\n"
       + "\"s3_video_url\": \"" + GlobalVariables.videoUpload.s3_url + "\",\n"

       + "\"public_web_url\": " + "\"https://app.myvendingmachine.com/game/77980a15-4684-4b7c-b61d-9a990cc8eaa3" + "\",\n"
       + "\"is_highlight\":false," + "\n"
       + "\"share_allowance\":false," + "\n"
       + "\"prize\":{"
       + "\"slot_id\": \"" + selectedPrize.slot_id + "\",\n"
       + "\"name\":\"r0c1\","
        + "\"showName\": \"" + selectedPrize.showName + "\",\n"
       + "\"price\":0.0," 
       + "\"rewardName\":\"Reward Name\","
        + "\"probabilityWeight\": " + selectedPrize.probabilityWeight + ",\n"
        + "\"inStock\": " + selectedPrize.inStock.ToString().ToLower() + ",\n"
        + "\"quantity\": " + selectedPrize.quantity + "},\n"
       + "\"final_url\":" + "\"https://fandomprizemachine.com/?eventId=68c08cc6aa5a1fc56fdf684b&url=https://fandomprizemachine.com/?eventId=68c08cc6aa5a1fc56fdf684b&url=https://vendingmachine-assets-archive.s3.us-east-1.amazonaws.com/uploads/20251010_012215_" + "\",\n"
       + "\"_id\":\"68e86a1907146812d88cd88a" + "\"\n"
    + "}";
            Debug.Log(jsonBodyOffline);
            HandlePostResponse(jsonBodyOffline);
            //HandlePostResponse("{\"start_time\":\"2025-10-09T20:06:13.882000\",\"end_time\":\"2025-10-10T02:06:35\",\"total_game_time\":1.0,\"total_screen_time\":1.0,\"date\":\"2025-10-10T02:06:35\",\"machine_id\":\"6884f98a78ea05f217e24b73\",\"event_id\":\"68c08cc6aa5a1fc56fdf684b\",\"total_clicks\":0,\"s3_video_url\":\"https://fandomprizemachine.com/?eventId=68c08cc6aa5a1fc56fdf684b&url=https://vendingmachine-assets-archive.s3.us-east-1.amazonaws.com/uploads/20251010_012215_\",\"public_web_url\":\"https://app.myvendingmachine.com/game/77980a15-4684-4b7c-b61d-9a990cc8eaa3\",\"is_highlight\":false,\"share_allowance\":false,\"prize\":{\"slot_id\":\"s1\",\"name\":\"r0c1\",\"showName\":\"MNUFC T-Shirt\",\"price\":0.0,\"rewardName\":\"Reward Name\",\"probabilityWeight\":25.0,\"inStock\":true,\"quantity\":84},\"final_url\":\"https://fandomprizemachine.com/?eventId=68c08cc6aa5a1fc56fdf684b&url=https://fandomprizemachine.com/?eventId=68c08cc6aa5a1fc56fdf684b&url=https://vendingmachine-assets-archive.s3.us-east-1.amazonaws.com/uploads/20251010_012215_\",\"_id\":\"68e86a1907146812d88cd88a\"}"
//);
        }
        else
        {
            string jsonBody =
        "{\n"
        + "\"start_time\": \"" + GlobalVariables.metricsObj.time_start + "\",\n"
        + "\"end_time\": \"" + DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz") + "\",\n"
        + "\"total_game_time\": " + 1 + ",\n"
        + "\"total_screen_time\": " + 1 + ",\n"
        + "\"date\": \"" + DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz") + "\",\n"
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
        Debug.Log(response);
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
