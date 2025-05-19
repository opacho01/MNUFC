using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class in charge of managing the Panel Cam recorder.
/// </summary>
public class Panel5Content : PanelContent
{
    /// <summary>
    /// The team logo displayed on the panel.
    /// </summary>
    public RawImage teamLogo;

    /// <summary>
    /// Manages webcam recording functionality, handling video capture and processing.
    /// </summary>
    public WebCamRecorder camRecorder;

    /// <summary>
    /// The background image for the disclaimer section.
    /// </summary>
    public RawImage DisclaimerBack;

    /// <summary>
    /// On enable set actual screen to 5 and call RecorderStart from WebCamRecorder.
    /// </summary>
    private void OnEnable()
    {
            GlobalVariables.ActualScreen = 5;
            camRecorder.RecorderStart();
        
    }

    /// <summary>
    /// Initialize panel, texture.
    /// </summary>
    /// <param name="allResources"></param>
    public override void fillPanel(GetAllResources allResources)
    {
        DisclaimerBack.texture = allResources.footerBanner;
    }

    /// <summary>
    /// Activate the next panel.
    /// </summary>
    public void ActivateNext()
    {
        nextPanelObj.gameObject.SetActive(true);
    }
}
