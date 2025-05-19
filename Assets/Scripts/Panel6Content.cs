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

    /// <summary>
    /// Assemble json to make a getReward post to server.
    /// </summary>
    public void getReward()
    {
        string uploadUrl = URLdirectory.rewardUrl;
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

        //Debug.Log("------" + jsonBody);
        HttpManager.AddRequestHeader("X-Machine-Key", GlobalVariables.machinesSecretKey);
        HttpManager.AddRequestHeader("Content-Type", "application/json");
        HttpManager.Post(uploadUrl, jsonBody, HandlePostResponse);
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
