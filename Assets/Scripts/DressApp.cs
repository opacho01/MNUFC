using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

/// <summary>
/// Class responsible for dressing and initializing all the application screens.
/// </summary>
public class DressApp : MonoBehaviour
{
    //All panels in the app
    /// <summary>
    /// Stores and manages all resources required by the application.
    /// </summary>
    private GetAllResources allResources;

    /// <summary>
    /// Manages the content and interactions for Panel 1.
    /// </summary>
    public Panel1Content panel1;

    /// <summary>
    /// Manages the content and interactions for Panel 2.
    /// </summary>
    public Panel2Content panel2;

    /// <summary>
    /// Manages the content and interactions for Panel 3.
    /// </summary>
    public Panel3Content panel3;

    /// <summary>
    /// Manages the content and interactions for Panel 4.
    /// </summary>
    public Panel4Content panel4;

    /// <summary>
    /// Manages the content and interactions for Panel 5.
    /// </summary>
    public Panel5Content panel5;

    /// <summary>
    /// Manages the content and interactions for Panel 6.
    /// </summary>
    public Panel6Content panel6;

    /// <summary>
    /// Manages the content and interactions for Panel 7.
    /// </summary>
    public Panel7Content panel7;

    /// <summary>
    /// Manages the content and interactions for Panel 8.
    /// </summary>
    public Panel8Content panel8;

    /// <summary>
    /// Handles the loading screen processes and transitions.
    /// </summary>
    public PanelLoading panelLoading;


    /// <summary>
    /// Assign the variable GetAllResources.
    /// </summary>
    private void Start()
    {
        allResources = GetComponent<GetAllResources>();
    }

    /// <summary>
    /// Call the cooroutine PrepareAll.
    /// </summary>
    public void DressAll()
    {
        StartCoroutine(PrepareAll());
    }

    /// <summary>
    ///Prepare and fill all panels whit all resources from server.
    /// </summary>
    private IEnumerator PrepareAll()
    {
        panelLoading.fillPanel(allResources);
        panel1.fillPanel(allResources);
        panel1.gameObject.SetActive(true);
        panel2.fillPanel(allResources);
        panel3.fillPanel(allResources);
        panel4.fillPanel(allResources);
        panel5.fillPanel(allResources);
        panel6.fillPanel(allResources);
        panel7.fillPanel(allResources);
        panel8.fillPanel(allResources);
        yield return new WaitForSeconds(2);
        
        yield return new WaitForSeconds(1.0f);
        panel1.videoPrepared = true;
        panel5.videoPrepared = true;
        GlobalVariables.initiated2 = true;
        

    }

    /// <summary>
    /// Disable all the panels except panel1, to return home.
    /// </summary>
    public void DisableAllToMain()
    {
        panelLoading.gameObject.SetActive(false);
        panel2.gameObject.SetActive(false);
        panel3.gameObject.SetActive(false);
        panel4.gameObject.SetActive(false);
        panel6.gameObject.SetActive(false);
        panel7.gameObject.SetActive(false);
        panel8.gameObject.SetActive(false);
        allResources.panelDownloading.SetActive(false);
    }
}
