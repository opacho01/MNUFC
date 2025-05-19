using System.Collections;
using UnityEngine;

/// <summary>
/// Base class for all container panels, providing functionality for transitions, 
/// video playback, and resource assignment.
/// </summary>
public class PanelContent : MonoBehaviour
{
    /// <summary>
    /// The resource manager providing assets for the panel.
    /// </summary>
    public GetAllResources allResources;

    /// <summary>
    /// The next panel to transition to.
    /// </summary>
    public PanelContent nextPanelObj;

    /// <summary>
    /// The previous panel in the transition flow.
    /// </summary>
    public PanelContent antPanelObj;

    /// <summary>
    /// Manages the overall appearance and behavior of the application.
    /// </summary>
    public DressApp dressApp;

    /// <summary>
    /// The AudioSource used for playing sounds within the panel.
    /// </summary>
    public AudioSource audios;

    /// <summary>
    /// Indicates whether the video associated with this panel is preloaded and ready.
    /// </summary>
    public bool videoPrepared = false;

    /// <summary>
    /// A backup texture used for rendering purposes.
    /// </summary>
    public RenderTexture backupTexture;

    /// <summary>
    /// Assigns resources to the panel upon initialization.
    /// </summary>
    /// <param name="allResourcesR">The resource manager containing textures, videos, and sounds.</param>
    public virtual void fillPanel(GetAllResources allResourcesR)
    {
        allResources = allResourcesR;
    }

    /// <summary>
    /// Virtual method to stop video and sound playback for the panel.
    /// Can be overridden by derived classes for specific behavior.
    /// </summary>
    public virtual void panelStop()
    {
    }

    /// <summary>
    /// Virtual method to initialize videos and sounds within the panel.
    /// Can be overridden to customize behavior.
    /// </summary>
    public virtual void panelInit()
    {
    }

    /// <summary>
    /// Virtual method to initialize panel elements for a specific player.
    /// </summary>
    /// <param name="numPlayer">The player number for customization.</param>
    public virtual void panelInit(int numPlayer)
    {
    }

    /// <summary>
    /// Transitions to the next panel by invoking a coroutine.
    /// </summary>
    public virtual void nextPanel()
    {
        StartCoroutine(nextPanelWait());
    }

    /// <summary>
    /// Coroutine that activates and initializes the next panel after a delay.
    /// </summary>
    /// <returns>An IEnumerator for coroutine execution.</returns>
    private IEnumerator nextPanelWait()
    {
        nextPanelObj.gameObject.SetActive(true);
        nextPanelObj.panelInit();
        yield return new WaitForSeconds(0.5f);
        Debug.Log("Transitioning to " + nextPanelObj.name + " from " + name);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Virtual method for transitioning to the next panel with a specified player.
    /// </summary>
    /// <param name="numPlayer">The player number used in the transition.</param>
    public virtual void nextPanel(int numPlayer)
    {
    }

    /// <summary>
    /// Virtual method for loading the first video in the sequence.
    /// Can be overridden for custom behavior.
    /// </summary>
    public virtual void LoadVideoFirst()
    {
    }
}
