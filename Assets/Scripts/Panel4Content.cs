using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;
using System;
using System.Collections;

/// <summary>
/// Class in charge of managing the Panel Countdown.
/// </summary>
public class Panel4Content : PanelContent
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
    /// The countdown text effect for the "Ready" stage.
    /// </summary>
    public TextEffectController ready;

    /// <summary>
    /// The countdown text effect for the "Set" stage.
    /// </summary>
    public TextEffectController set;

    /// <summary>
    /// The countdown text effect for the "Flex" stage.
    /// </summary>
    public TextEffectController flex;

    /// <summary>
    /// The VideoPlayer component that plays the background video.
    /// </summary>
    public VideoPlayer background;

    /// <summary>
    /// The background image for the disclaimer section.
    /// </summary>
    public RawImage DisclaimerBack;

    /// <summary>
    /// Delay before starting the countdown sequence.
    /// </summary>
    public float delayStart = 0.4f;

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
    /// Cooroutine to start countdown whit delay.
    /// </summary>
    private IEnumerator waitToStart()
    {
        yield return new WaitForSeconds(delayStart);
        ready.gameObject.SetActive(true);
    }

    /// <summary>
    /// Initialize panel, textures, video, audios, texts.
    /// </summary>
    /// <param name="allResources"></param>
    public override void fillPanel(GetAllResources allResources)
    {
        houseButton.texture = allResources.houseButton;
        DisclaimerBack.texture = allResources.footerBanner;
        allResources.PlayVideo(allResources.backgroundP4, background, gameObject);//, !videoPrepared);
        ready.gameObject.GetComponent<TMP_Text>().text = allResources.themeData.step_4_countdown.countdown_banner_text_1;
        ready.gameObject.GetComponent<TMP_Text>().color = GlobalVariables.colorSecondary;
        ready.audioClip = allResources.readyAudio;
        set.gameObject.GetComponent<TMP_Text>().text = allResources.themeData.step_4_countdown.countdown_banner_text_2;
        set.gameObject.GetComponent<TMP_Text>().color = GlobalVariables.colorSecondary;
        set.audioClip = allResources.setAudio;
        flex.gameObject.GetComponent<TMP_Text>().text = allResources.themeData.step_4_countdown.countdown_banner_text_3;
        flex.gameObject.GetComponent<TMP_Text>().color = GlobalVariables.colorSecondary;
        flex.audioClip = allResources.flexAudio;
    }

    /// <summary>
    /// Set actual sceen to 4, disable gameobjects ready, set, flex, last panel,
    /// enable this panel, and call cooroutine waitToStart.
    /// </summary>
    public override void panelInit()
    {
        GlobalVariables.ActualScreen = 4;
        ready.gameObject.SetActive(false);
        set.gameObject.SetActive(false);
        flex.gameObject.SetActive(false);
        StartCoroutine(waitToStart());
        antPanelObj.gameObject.SetActive(false);
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Enable next panel and disable this panel.
    /// </summary>
    public override void nextPanel()
    {
        nextPanelObj.gameObject.SetActive(true);
        gameObject.gameObject.SetActive(false);
    }
}
