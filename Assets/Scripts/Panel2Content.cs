using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

/// <summary>
/// Class in charge of managing the Panel Select Video.
/// </summary>
public class Panel2Content : PanelContent
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
    /// The background video player for the panel.
    /// </summary>
    public VideoPlayer background;
    /// <summary>
    /// The instructions image displayed on the panel.
    /// </summary>
    public RawImage instructions;
    /// <summary>
    /// The instructions text guiding the user.
    /// </summary>
    public TMP_Text instructionsTxt;
    /// <summary>
    /// Arrays of video players used for player previews.
    /// </summary>
    public VideoPlayer[] videoPlayersPre;
    /// <summary>
    /// Arrays of video players used for player previews in configurations of 2 players.
    /// </summary>
    public VideoPlayer[] videoPlayersPre2;
    /// <summary>
    /// Arrays of video players used for player previews in configurations of 3 players.
    /// </summary>
    public VideoPlayer[] videoPlayersPre3;
    /// <summary>
    /// Arrays of video players used for player previews in configurations of 4 players.
    /// </summary>
    public VideoPlayer[] videoPlayersPre4;
    /// <summary>
    /// Arrays of video players used for player previews in configurations of 5 players.
    /// </summary>
    public VideoPlayer[] videoPlayersPre5;
    /// <summary>
    /// Arrays of player name text elements.
    /// </summary>
    public TMP_Text[] playerNameTxt;
    /// <summary>
    /// Arrays of player name text elements for configuration of 2 players.
    /// </summary>
    public TMP_Text[] playerNameTxt2;
    /// <summary>
    /// Arrays of player name text elements for configuration of 3 players.
    /// </summary>
    public TMP_Text[] playerNameTxt3;
    /// <summary>
    /// Arrays of player name text elements for configuration of 4 players.
    /// </summary>
    public TMP_Text[] playerNameTxt4;
    /// <summary>
    /// Arrays of player name text elements for configuration of 5 players.
    /// </summary>
    public TMP_Text[] playerNameTxt5;
    /// <summary>
    /// Arrays of celebration text elements.
    /// </summary>
    public TMP_Text[] celebrationTxt;
    /// <summary>
    /// Arrays of celebration text elements for configurations of 2 players.
    /// </summary>
    public TMP_Text[] celebrationTxt2;
    /// <summary>
    /// Arrays of celebration text elements for configurations of 3 players.
    /// </summary>
    public TMP_Text[] celebrationTxt3;
    /// <summary>
    /// Arrays of celebration text elements for configurations of 4 players.
    /// </summary>
    public TMP_Text[] celebrationTxt4;
    /// <summary>
    /// Arrays of celebration text elements for configurations of 5 players.
    /// </summary>
    public TMP_Text[] celebrationTxt5;
    /// <summary>
    /// Array of background images for the video players.
    /// </summary>
    public Image[] videoPlayersBack;
    /// <summary>
    /// The main title text displayed on the panel.
    /// </summary>
    public TMP_Text title;

    /// <summary>
    /// The subtitle text displayed at the bottom of the panel.
    /// </summary>
    public TMP_Text titleDown;

    /// <summary>
    /// The disclaimer background image.
    /// </summary>
    public RawImage DisclaimerBack;

    /// <summary>
    /// Array of game objects representing different player configurations.
    /// </summary>
    public GameObject[] panelNumPlayers;

    /// <summary>
    /// The metrics handler for tracking user interactions.
    /// </summary>
    public Metrics metrics;

    /// <summary>
    /// The timestamp marking when the panel was initialized.
    /// </summary>
    public float initTime;

    /// <summary>
    /// The timestamp marking when the panel was deactivated.
    /// </summary>
    public float endTime;

    /// <summary>
    /// On enable play the background video and the players previsualization videos. 
    /// </summary>
    private void OnEnable()
    {
        background.Play();
        for (byte i = 0; i < videoPlayersPre.Length; i++)
        {
            videoPlayersPre[i].Play();
        }
    }

    /// <summary>
    /// Initialize panel, textures, video, audios, colors, texts.
    /// </summary>
    /// <param name="allResources"></param>
    public override void fillPanel(GetAllResources allResources)
    {
        try { title.text = allResources.themeData.step_2_select_screen.top_name_banner_text; } catch (Exception e) { title.text = "SELECT1"; }
        try { titleDown.text = allResources.themeData.step_2_select_screen.bottom_name_banner_text; } catch (Exception e) { title.text = "YOUR FLEX!1"; }
        title.color = GlobalVariables.colorPrimary;
        titleDown.color = GlobalVariables.colorSecondary;
        SetNumberCelebrations(allResources.numCelebrations);
        houseButton.texture = allResources.houseButton;
        instructions.texture = allResources.touchButton;
        allResources.PlayVideo(allResources.backgroundP2, background, gameObject);
        background.prepareCompleted += OnBackPlaying;
        for (byte i = 0; i < videoPlayersPre.Length; i++)
        {
            allResources.PlayVideo(allResources.videoPlayersPre[i], videoPlayersPre[i]);
            videoPlayersPre[i].loopPointReached += OnVideoEnd;
            try { playerNameTxt[i].text = allResources.themeData.step_2_select_screen.player_video_previews[i].main_banner_text; } catch (Exception e) { playerNameTxt[i].text = "player " + i; }
            try { celebrationTxt[i].text = allResources.themeData.step_2_select_screen.player_video_previews[i].secondary_banner_text; } catch (Exception e) { celebrationTxt[i].text = "celebration " + i; }
            playerNameTxt[i].color = GlobalVariables.colorText;
            celebrationTxt[i].color = GlobalVariables.colorText;
        }

        DisclaimerBack.texture = allResources.footerBanner;
        for (byte y = 0; y < videoPlayersBack.Length; y++)
        {
            videoPlayersBack[y].color = GlobalVariables.colorPrimary;
        }
    }

    /// <summary>
    ///  Select te object with the number of players in theme.
    /// </summary>
    /// <param name="numCelebrations">number of celebrations in theme</param>
    public void SetNumberCelebrations(byte numCelebrations)
    {
        videoPlayersPre = new VideoPlayer[numCelebrations];
        playerNameTxt = new TMP_Text[numCelebrations];
        celebrationTxt = new TMP_Text[numCelebrations];
        for(byte i = 0; i < panelNumPlayers.Length; i++)
        {
            panelNumPlayers[i].SetActive(false);
        }
        switch (numCelebrations)
        {
            case 2:
                for(byte i = 0; i < videoPlayersPre2.Length; i++)
                {
                    videoPlayersPre[i] = videoPlayersPre2[i];
                    playerNameTxt[i] = playerNameTxt2[i];
                    celebrationTxt[i] = celebrationTxt2[i];
                }
                panelNumPlayers[0].SetActive(true);
                break;

            case 3:
                for (byte i = 0; i < videoPlayersPre3.Length; i++)
                {
                    videoPlayersPre[i] = videoPlayersPre3[i];
                    playerNameTxt[i] = playerNameTxt3[i];
                    celebrationTxt[i] = celebrationTxt3[i];
                }
                panelNumPlayers[1].SetActive(true);
                break;

            case 4:
                for (byte i = 0; i < videoPlayersPre4.Length; i++)
                {
                    videoPlayersPre[i] = videoPlayersPre4[i];
                    playerNameTxt[i] = playerNameTxt4[i];
                    celebrationTxt[i] = celebrationTxt4[i];
                }
                panelNumPlayers[2].SetActive(true);
                break;

            case 5:
                for (byte i = 0; i < videoPlayersPre5.Length; i++)
                {
                    videoPlayersPre[i] = videoPlayersPre5[i];
                    playerNameTxt[i] = playerNameTxt5[i];
                    celebrationTxt[i] = celebrationTxt5[i];
                }
                panelNumPlayers[3].SetActive(true);
                break;
        }
    }

    /// <summary>
    /// Set actual screen to 2, play audio and stop the last panel.
    /// </summary>
    public override void panelInit()
    {
        if (GlobalVariables.iniciado && GlobalVariables.initiated2) { 
            GlobalVariables.ActualScreen = 2;
            audios.clip = allResources.screenAudioP2;
            audios.Play();
            initTime = Time.time;
            antPanelObj.panelStop();
        }
    }

    /// <summary>
    /// Enable next panel and send information of what player is selected.
    /// </summary>
    /// <param name="numPlayer"></param>
    public override void nextPanel(int numPlayer)
    {
        GlobalVariables.metricsObj.player_selected = playerNameTxt[numPlayer].text;
        nextPanelObj.gameObject.SetActive(true);
        GlobalVariables.playerSelected = (byte)numPlayer;
        //StartCoroutine(nextPanelWait(numPlayer));
    }

    /// <summary>
    /// Ensures the video continues playing after it ends
    /// </summary>
    /// <param name="vp"></param>
    void OnVideoEnd(VideoPlayer vp)
    {
        vp.Stop();
        vp.Play();
    }

    /// <summary>
    /// Call awaitChangePanel coorutine.
    /// </summary>
    /// <param name="vp"></param>
    void OnBackPlaying(VideoPlayer vp)
    {
        StartCoroutine(awaitChangePanel());
    }

    /// <summary>
    /// Call panelInit whit delay.
    /// </summary>
    /// <returns></returns>
    IEnumerator awaitChangePanel()
    {
        yield return new WaitForSeconds(GlobalVariables.generalAwait);
        panelInit();
    }
}
