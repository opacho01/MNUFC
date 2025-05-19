using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

/// <summary>
/// Manages the Video Upload panel, handling video playback, UI elements, animations, and state transitions.
/// </summary>
public class PanelLoading : PanelContent
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
    /// The disclaimer background image.
    /// </summary>
    public RawImage DisclaimerBack;

    /// <summary>
    /// The title text displayed on the panel.
    /// </summary>
    public TMP_Text title;

    /// <summary>
    /// The loading animation component.
    /// </summary>
    public AnimatorImage loadingAnim;

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
    /// Initializes the panel with textures, audio, video, and animations.
    /// </summary>
    /// <param name="allResources">The asset manager containing resources for setup.</param>
    public override void fillPanel(GetAllResources allResources)
    {
        DisclaimerBack.texture = allResources.footerBanner;
        houseButton.texture = allResources.houseButton;
        allResources.PlayVideo(allResources.backgroundP6, background, gameObject);
        audios.clip = allResources.screenAudioP6;
        loadingAnim.initAnim();
    }

    /// <summary>
    /// Sets up the panel with animation, sound, and transitions to the next panel.
    /// </summary>
    public override void panelInit()
    {
        GlobalVariables.ActualScreen = 6;

        // Assigns the loading banner text, handling exceptions if unavailable.
        try
        {
            title.text = allResources.themeData.step_6_video_upload.loading_banner_text;
        }
        catch (Exception)
        {
            title.text = "PLEASE WAIT...";
        }

        audios.Play();
        loadingAnim.animPuppet();
        antPanelObj.gameObject.SetActive(false);
        nextPanelObj.gameObject.SetActive(true);
    }
}
