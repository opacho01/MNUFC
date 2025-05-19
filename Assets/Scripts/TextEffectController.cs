using UnityEngine;
using EasyTextEffects;
using System.Collections;
using System;

/// <summary>
/// Controls text effects and manages transitions between panels with audio playback.
/// </summary>
public class TextEffectController : MonoBehaviour
{
    /// <summary>
    /// The text effect component responsible for managing visual effects.
    /// </summary>
    public TextEffect myText;

    /// <summary>
    /// Delay in seconds before transitioning to the next text sequence.
    /// </summary>
    public float secondsToWait = 1;

    /// <summary>
    /// Cached WaitForSeconds instance to optimize coroutine delays.
    /// </summary>
    private WaitForSeconds waitForSeconds;

    /// <summary>
    /// The currently active GameObject.
    /// </summary>
    public GameObject actual;

    /// <summary>
    /// The next GameObject to activate in the sequence.
    /// </summary>
    public GameObject next;

    /// <summary>
    /// Indicates whether this is the final text effect in the sequence.
    /// </summary>
    public bool lastText = false;

    /// <summary>
    /// Determines whether the current panel should remain active after the effect sequence.
    /// </summary>
    public bool stayPanel = false;

    /// <summary>
    /// The array of text effects to apply at the start.
    /// </summary>
    public string[] start;

    /// <summary>
    /// The array of text effects to apply during the intermediate phase.
    /// </summary>
    public string[] med;

    /// <summary>
    /// The array of text effects to apply at the end.
    /// </summary>
    public string[] end;

    /// <summary>
    /// The next panel to display after completing text effects.
    /// </summary>
    public PanelContent panelNext;

    /// <summary>
    /// The current panel associated with the text effects.
    /// </summary>
    public PanelContent panelAct;

    /// <summary>
    /// Audio clip associated with the text effect.
    /// </summary>
    public AudioClip audioClip;

    /// <summary>
    /// The audio source used to play sound effects.
    /// </summary>
    public AudioSource audioSource;

    /// <summary>
    /// Defines the type of text effect (e.g., "ready", "set", "flex").
    /// </summary>
    public string typeText = "";

    /// <summary>
    /// Initializes text effects and starts the appropriate audio when the object is enabled.
    /// </summary>
    void OnEnable()
    {
        assignClip();
        StopAllCoroutines();
        myText.StopManualEffects();
        waitForSeconds = new WaitForSeconds(secondsToWait);
        for (byte i = 0; i < start.Length; i++)
        {
            myText.StartManualEffect(start[i]);
        }
        if (audioClip != null)
        {
            audioSource.mute = false;
            audioSource.PlayOneShot(audioClip);
        }
    }

    /// <summary>
    /// Starts the coroutine for the next text sequence effect.
    /// </summary>
    public void SiguienteSecuencia()
    {
        StartCoroutine(WaitSeconds());
    }

    /// <summary>
    /// Executes a sequence of text effects with timed delays.
    /// </summary>
    IEnumerator WaitSeconds()
    {
        for (byte i = 0; i < med.Length; i++)
        {
            myText.StartManualEffect(med[i]);
        }
        yield return waitForSeconds;
        for (byte i = 0; i < end.Length; i++)
        {
            myText.StartManualEffect(end[i]);
        }
        yield return new WaitForSeconds(0.5f);
        if (!stayPanel)
        {
            NextText();
        }
        StopAllCoroutines();
    }

    /// <summary>
    /// Transitions to the next text effect or panel.
    /// </summary>
    public void NextText()
    {
        myText.StopManualEffects();
        actual.SetActive(false);
        if (lastText) {
            if (panelNext && !stayPanel)
            {
                panelNext.nextPanel();
            }
        }
        else
        {
            next.SetActive(true);
        }
    }

    /// <summary>
    /// Assigns the appropriate audio clip based on the text type.
    /// </summary>
    public void assignClip()
    {
        switch (typeText)
        {
            case "ready":
                try
                {
                    audioClip = panelAct.allResources.readyAudio;
                }
                catch (Exception e) { }
                break;

            case "set":
                try {
                audioClip = panelAct.allResources.setAudio;
                }
                catch (Exception e) { }
                break;

            case "flex":
                try {
                audioClip = panelAct.allResources.flexAudio;
                }
                catch (Exception e) { }
                break;
        }
    }
}
