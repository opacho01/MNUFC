using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;
using System;
using System.Collections;

/// <summary>
/// Manages the Prize panel, handling video playback, UI elements, animations, and prize display.
/// </summary>
public class Panel7Content : PanelContent
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
    /// The title text displayed on the panel.
    /// </summary>
    public TMP_Text title;

    /// <summary>
    /// The prize name displayed on the panel.
    /// </summary>
    public TMP_Text prize;

    /// <summary>
    /// The disclaimer text providing additional prize information.
    /// </summary>
    public TMP_Text DisclaimerTxt;

    /// <summary>
    /// The array of animated puppets displayed on the panel.
    /// </summary>
    public AnimatorImage[] puppets;

    /// <summary>
    /// The background image for the disclaimer.
    /// </summary>
    public RawImage DisclaimerBack;

    /// <summary>
    /// Stores prize-related information.
    /// </summary>
    public InfoPrize infoPrize;

    /// <summary>
    /// Tracks whether the panel has been activated for the first time.
    /// </summary>
    public bool FirstActivated = false;

    /// <summary>
    /// Plays the background video when the panel is enabled.
    /// </summary>
    private void OnEnable()
    {
        background.Play();
    }

    /// <summary>
    /// Initializes the panel with textures, video, audio, colors, texts, and animations.
    /// </summary>
    /// <param name="allResources">The asset manager containing resources for setup.</param>
    public override void fillPanel(GetAllResources allResources)
    {
        title.text = allResources.themeData.step_8_prize_screen.winner_message_text;
        title.color = GlobalVariables.colorPrimary;

        DisclaimerTxt.text = allResources.themeData.step_8_prize_screen.disclaimer_banner_text;
        DisclaimerTxt.color = GlobalVariables.colorSecondary;

        prize.color = GlobalVariables.colorSecondary;

        houseButton.texture = allResources.houseButton;
        audios.clip = allResources.screenAudioP8;
        DisclaimerBack.texture = allResources.footerBanner;

        allResources.PlayVideo(allResources.backgroundP8, background, gameObject);

        // Initializes puppet animations.
        for (byte i = 0; i < puppets.Length; i++)
        {
            puppets[i].initAnim();
        }
    }

    /// <summary>
    /// Sets up the panel by playing audio, triggering puppet animations, and displaying prize details.
    /// Calls a coroutine to transition after a delay.
    /// </summary>
    public override void panelInit()
    {
        GlobalVariables.ActualScreen = 8;

        // Activates puppet animations.
        for (byte i = 0; i < puppets.Length; i++)
        {
            puppets[i].animPuppet();
        }

        audios.Play();
        prize.text = GlobalVariables.infoPrize.prize.showName;

        antPanelObj.gameObject.SetActive(false);
        nextPanelObj.gameObject.SetActive(true);

        StartCoroutine(waitToChange());
    }

    /// <summary>
    /// Coroutine that transitions to the next panel after a delay.
    /// </summary>
    /// <returns>An IEnumerator for coroutine execution.</returns>
    private IEnumerator waitToChange()
    {
        yield return new WaitForSeconds(10);
        nextPanel();
    }

    /// <summary>
    /// Activates and initializes the next panel in the sequence.
    /// </summary>
    public override void nextPanel()
    {
        nextPanelObj.gameObject.SetActive(true);
        nextPanelObj.panelInit();
    }
}