using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

/// <summary>
/// Manages the Attract Video panel, handling video playback, UI elements, and metrics tracking.
/// </summary>
public class Panel1Content : PanelContent
{
    /// <summary>
    /// The VideoPlayer component that plays the background video.
    /// </summary>
    public VideoPlayer videoBack;

    /// <summary>
    /// The team logo displayed on the panel.
    /// </summary>
    public RawImage teamLogo;

    /// <summary>
    /// The disclaimer text displayed on the panel.
    /// </summary>
    public TMP_Text disclaimer;

    /// <summary>
    /// The instructions image displayed on the panel.
    /// </summary>
    public RawImage instructions;

    /// <summary>
    /// The text instructions displayed on the panel.
    /// </summary>
    public TMP_Text instructionsTxt;

    /// <summary>
    /// The initial timestamp when the panel is activated.
    /// </summary>
    public float initTime;

    /// <summary>
    /// The timestamp when the panel is deactivated.
    /// </summary>
    public float endTime;

    /// <summary>
    /// The metrics handler for tracking user interactions.
    /// </summary>
    public Metrics metrics;

    /// <summary>
    /// The background image for the disclaimer.
    /// </summary>
    public RawImage DisclaimerBack;

    /// <summary>
    /// Audio sources for various "Ready, Set, Flex" cues.
    /// </summary>
    public AudioSource[] audioSourcesReadySetFlex;

    /// <summary>
    /// Plays the background video when the panel is enabled.
    /// </summary>
    private void OnEnable()
    {
        videoBack.Play();
    }

    /// <summary>
    /// Initializes the panel with textures, video, audio, and colors.
    /// </summary>
    /// <param name="allResources">The asset manager containing resources for setup.</param>
    public override void fillPanel(GetAllResources allResources)
    {
        teamLogo.texture = allResources.appLogo;
        instructions.texture = allResources.touchButton;
        allResources.PlayVideo(allResources.appBackground, videoBack, gameObject);
        videoBack.prepareCompleted += OnBackPlaying;
        audios.clip = allResources.screenAudioP1;
        DisclaimerBack.color = GlobalVariables.colorText;
        disclaimer.color = GlobalVariables.colorSecondary;
    }

    /// <summary>
    /// Activates the next panel and records the stopped time in metrics.
    /// </summary>
    public override void nextPanel()
    {
        GlobalVariables.iniciado = true;
        GlobalVariables.metricsObj.time_stoped = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        nextPanelObj.gameObject.SetActive(true);
        StartCoroutine(nextPanelWait());
    }

    /// <summary>
    /// Coroutine to record time spent on the screen in metrics.
    /// </summary>
    /// <returns>An IEnumerator for coroutine execution.</returns>
    private IEnumerator nextPanelWait()
    {
        endTime = Time.time;
        GlobalVariables._id = "";
        metrics.timeInScreen("InitScreen", (endTime - initTime));
        GlobalVariables.metricsObj.time_start = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        yield return new WaitForSeconds(0.5f);
    }

    /// <summary>
    /// Stops the background video and disables the screen.
    /// </summary>
    public override void panelStop()
    {
        gameObject.SetActive(false);
        videoBack.Stop();
    }

    /// <summary>
    /// Initializes the panel when the home button is clicked or at startup.
    /// Resets metrics and step tracking, sets the active screen, and plays initial sounds.
    /// </summary>
    public override void panelInit()
    {
        GlobalVariables.ActualScreen = 1;
        initTime = Time.time;
        GlobalVariables.metricsObj = new MetricsObj();
        GlobalVariables.stepDictionary = new Dictionary<string, StepData>();
        GlobalVariables.allowedChanged = false;

        for (int i = 1; i <= 9; i++)
        {
            string stepKey = i.ToString();
            GlobalVariables.stepDictionary[stepKey] = new StepData
            {
                step_number = i,
                heatmap = new List<HeatmapPoint>()
            };
        }

        audios.Play();
        dressApp.DisableAllToMain();

        for (byte i = 0; i < audioSourcesReadySetFlex.Length; i++)
        {
            audioSourcesReadySetFlex[i].Stop();
            audioSourcesReadySetFlex[i].mute = true;
        }
    }

    /// <summary>
    /// Calls the coroutine to wait before changing the panel.
    /// </summary>
    /// <param name="vp">The VideoPlayer instance.</param>
    private void OnBackPlaying(VideoPlayer vp)
    {
        StartCoroutine(awaitChangePanel());
    }

    /// <summary>
    /// Delays panel initialization for a specified duration.
    /// </summary>
    /// <returns>An IEnumerator for coroutine execution.</returns>
    private IEnumerator awaitChangePanel()
    {
        yield return new WaitForSeconds(GlobalVariables.generalAwait);
        panelInit();
    }
}