using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

/// <summary>
/// Class in charge of managing the Panel Video Preview.
/// </summary>
public class Panel3Content : PanelContent
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
    /// The VideoPlayer component that plays the selected player's preview video.
    /// </summary>
    public VideoPlayer videoPlayer;

    /// <summary>
    /// The title text displayed on the panel.
    /// </summary>
    public TMP_Text title;

    /// <summary>
    /// The celebration text displayed on the panel.
    /// </summary>
    public TMP_Text celebration;

    /// <summary>
    /// The selected player's index.
    /// </summary>
    private int numPlayer = 0;

    /// <summary>
    /// The background image for the disclaimer section.
    /// </summary>
    public RawImage DisclaimerBack;

    /// <summary>
    /// The background image for the video preview.
    /// </summary>
    public RawImage videoBack;

    /// <summary>
    /// Plays the background video when the panel is enabled.
    /// </summary>
    private void OnEnable()
    {
        background.Play();
    }

    /// <summary>
    /// Initialize panel, textures, video.
    /// </summary>
    /// <param name="allResourcesR"></param>
    public override void fillPanel(GetAllResources allResourcesR)
    {
        allResources = allResourcesR;
        houseButton.texture = allResources.houseButton;
        allResources.PlayVideo(allResources.backgroundP3, background, gameObject);//, !videoPrepared);
        background.prepareCompleted += OnBackPlaying;
        DisclaimerBack.texture = allResources.footerBanner;
    }

    /// <summary>
    /// Set text, video, and sounds depending player selected.
    /// </summary>
    /// <param name="numPlayerR">Player selected</param>
    public override void panelInit(int numPlayerR)
    {
        numPlayer = numPlayerR;
        
        videoPlayer.loopPointReached += OnVideoEnd;
        try
        {
            title.text = allResources.themeData.step_2_select_screen.player_video_previews[numPlayerR].main_banner_text;
            celebration.text = allResources.themeData.step_2_select_screen.player_video_previews[numPlayerR].secondary_banner_text;
        } catch (Exception e)
        {
            title.text = "Player " + numPlayer;
            celebration.text = "Celebration " + numPlayer;
        }
        title.color = GlobalVariables.colorSecondary;
        celebration.color = GlobalVariables.colorPrimary;
        GlobalVariables.ActualScreen = 3;
        audios.clip = allResources.screenAudioP3;
        audios.volume = 0.8f;
        audios.Play();
        audios.PlayOneShot(allResources.audioPlayers[numPlayer]);
        antPanelObj.gameObject.SetActive(false);
    }

    /// <summary>
    /// Adds behavior to initialize the next panel when the video ends.
    /// </summary>
    /// <param name="vp">Video player</param>
    void OnVideoEnd(VideoPlayer vp)
    {
        vp.Stop();
        nextPanelObj.gameObject.SetActive(true);
        nextPanelObj.panelInit();
    }

    /// <summary>
    /// Await the background video to initialize the coorutine awaitChangePanel;
    /// </summary>
    /// <param name="vp"></param>
    void OnBackPlaying(VideoPlayer vp)
    {
        StartCoroutine(awaitChangePanel());
    }

    /// <summary>
    /// Coorutine Initaialize this panel whit delay, and play the player video.
    /// </summary>
    IEnumerator awaitChangePanel()
    {
        allResources.PlayVideo(allResources.videoPlayers[GlobalVariables.playerSelected], videoPlayer);
        yield return new WaitForSeconds(GlobalVariables.generalAwait);
        panelInit(GlobalVariables.playerSelected);
    }
}
