using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;
using System;

/// <summary>
/// Manages the QR Display panel, handling video playback, UI elements, permissions, and transitions.
/// </summary>
public class Panel8Content : PanelContent
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
    /// The instructions text guiding the user.
    /// </summary>
    public TMP_Text instructions;

    /// <summary>
    /// The disclaimer text providing additional information.
    /// </summary>
    public TMP_Text disclaimer;

    /// <summary>
    /// The toggle button for granting permission to share a video.
    /// </summary>
    public Toggle permission;

    /// <summary>
    /// The background image for the disclaimer.
    /// </summary>
    public RawImage DisclaimerBack;

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
    /// Initializes the panel with textures, audio, video, colors, and displayed texts.
    /// </summary>
    /// <param name="allResources">The asset manager containing resources for setup.</param>
    public override void fillPanel(GetAllResources allResources)
    {
        houseButton.texture = allResources.houseButton;

        // Assigns text values, using fallbacks in case resources are unavailable.
        try
        {
            title.text = allResources.themeData.step_9_qr_screen.thank_you_banner_text;
        }
        catch (Exception)
        {
            title.text = "Title";
        }

        try
        {
            instructions.text = allResources.themeData.step_9_qr_screen.instructions_banner_text;
        }
        catch (Exception)
        {
            instructions.text = "Instructions";
        }

        try
        {
            disclaimer.text = allResources.themeData.step_9_qr_screen.qr_code_placeholder;
        }
        catch (Exception)
        {
            disclaimer.text = "Disclaimer";
        }

        // Sets text colors according to global theme variables.
        title.color = GlobalVariables.colorPrimary;
        instructions.color = GlobalVariables.colorSecondary;
        disclaimer.color = GlobalVariables.colorSecondary;

        DisclaimerBack.texture = allResources.footerBanner;
        audios.clip = allResources.screenAudioP9;
        allResources.PlayVideo(allResources.backgroundP9, background, gameObject);
    }

    /// <summary>
    /// Initializes the panel, setting the active screen, playing audio, and resetting permission settings.
    /// </summary>
    public override void panelInit()
    {
        GlobalVariables.ActualScreen = 9;
        permission.isOn = false;
        audios.Play();
        GlobalVariables.permission = false;
        antPanelObj.gameObject.SetActive(false);
    }

    /// <summary>
    /// Updates the permission status for sharing a video.
    /// Called when the permission toggle button is interacted with.
    /// </summary>
    public void changePermission()
    {
        GlobalVariables.permission = permission.isOn;
        GlobalVariables.metricsObj.allowed = permission.isOn;
        GlobalVariables.allowedChanged = true;
    }
}
