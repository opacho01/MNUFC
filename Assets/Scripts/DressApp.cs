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
        int totalSteps = 15;
        int currentStep = 0;
        
        try
        {
            // Step 1: Fill loading panel
            currentStep++;
            Debug.Log($"[DressApp] Preparing assets... Step {currentStep}/{totalSteps} (Loading Panel) - {(currentStep * 100f / totalSteps):F1}%");
            try
            {
                panelLoading.fillPanel(allResources);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[DressApp] ❌ ERROR in Step {currentStep}: Failed to fill Loading Panel. Error: {e.Message}");
                Debug.LogException(e);
            }
            
            // Step 2: Fill panel 1
            currentStep++;
            Debug.Log($"[DressApp] Preparing assets... Step {currentStep}/{totalSteps} (Panel 1) - {(currentStep * 100f / totalSteps):F1}%");
            try
            {
                panel1.fillPanel(allResources);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[DressApp] ❌ ERROR in Step {currentStep}: Failed to fill Panel 1. Error: {e.Message}");
                Debug.LogException(e);
            }
            
            // Step 3: Activate panel 1
            currentStep++;
            Debug.Log($"[DressApp] Preparing assets... Step {currentStep}/{totalSteps} (Activating Panel 1) - {(currentStep * 100f / totalSteps):F1}%");
            try
            {
                panel1.gameObject.SetActive(true);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[DressApp] ❌ ERROR in Step {currentStep}: Failed to activate Panel 1. Error: {e.Message}");
                Debug.LogException(e);
            }
            
            // Step 4: Fill panel 2
            currentStep++;
            Debug.Log($"[DressApp] Preparing assets... Step {currentStep}/{totalSteps} (Panel 2) - {(currentStep * 100f / totalSteps):F1}%");
            try
            {
                panel2.fillPanel(allResources);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[DressApp] ❌ ERROR in Step {currentStep}: Failed to fill Panel 2. Error: {e.Message}");
                Debug.LogException(e);
            }
            
            // Step 5: Fill panel 3
            currentStep++;
            Debug.Log($"[DressApp] Preparing assets... Step {currentStep}/{totalSteps} (Panel 3) - {(currentStep * 100f / totalSteps):F1}%");
            try
            {
                panel3.fillPanel(allResources);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[DressApp] ❌ ERROR in Step {currentStep}: Failed to fill Panel 3. Error: {e.Message}");
                Debug.LogException(e);
            }
            
            // Step 6: Fill panel 4
            currentStep++;
            Debug.Log($"[DressApp] Preparing assets... Step {currentStep}/{totalSteps} (Panel 4) - {(currentStep * 100f / totalSteps):F1}%");
            try
            {
                panel4.fillPanel(allResources);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[DressApp] ❌ ERROR in Step {currentStep}: Failed to fill Panel 4. Error: {e.Message}");
                Debug.LogException(e);
            }
            
            // Step 7: Fill panel 5
            currentStep++;
            Debug.Log($"[DressApp] Preparing assets... Step {currentStep}/{totalSteps} (Panel 5) - {(currentStep * 100f / totalSteps):F1}%");
            try
            {
                panel5.fillPanel(allResources);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[DressApp] ❌ ERROR in Step {currentStep}: Failed to fill Panel 5. Error: {e.Message}");
                Debug.LogException(e);
            }
            
            // Step 8: Fill panel 6
            currentStep++;
            Debug.Log($"[DressApp] Preparing assets... Step {currentStep}/{totalSteps} (Panel 6) - {(currentStep * 100f / totalSteps):F1}%");
            try
            {
                panel6.fillPanel(allResources);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[DressApp] ❌ ERROR in Step {currentStep}: Failed to fill Panel 6. Error: {e.Message}");
                Debug.LogException(e);
            }
            
            // Step 9: Fill panel 7
            currentStep++;
            Debug.Log($"[DressApp] Preparing assets... Step {currentStep}/{totalSteps} (Panel 7) - {(currentStep * 100f / totalSteps):F1}%");
            try
            {
                panel7.fillPanel(allResources);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[DressApp] ❌ ERROR in Step {currentStep}: Failed to fill Panel 7. Error: {e.Message}");
                Debug.LogException(e);
            }
            
            // Step 10: Fill panel 8
            currentStep++;
            Debug.Log($"[DressApp] Preparing assets... Step {currentStep}/{totalSteps} (Panel 8) - {(currentStep * 100f / totalSteps):F1}%");
            try
            {
                panel8.fillPanel(allResources);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[DressApp] ❌ ERROR in Step {currentStep}: Failed to fill Panel 8. Error: {e.Message}");
                Debug.LogException(e);
            }
            
            // Step 11: Initial wait
            currentStep++;
            Debug.Log($"[DressApp] Preparing assets... Step {currentStep}/{totalSteps} (Processing - Wait 1) - {(currentStep * 100f / totalSteps):F1}%");
            yield return new WaitForSeconds(2);
            
            // Step 12: Secondary wait
            currentStep++;
            Debug.Log($"[DressApp] Preparing assets... Step {currentStep}/{totalSteps} (Processing - Wait 2) - {(currentStep * 100f / totalSteps):F1}%");
            yield return new WaitForSeconds(1.0f);
            
            // Step 13: Set panel1 video prepared
            currentStep++;
            Debug.Log($"[DressApp] Preparing assets... Step {currentStep}/{totalSteps} (Panel 1 Video Ready) - {(currentStep * 100f / totalSteps):F1}%");
            try
            {
                panel1.videoPrepared = true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[DressApp] ❌ ERROR in Step {currentStep}: Failed to set Panel 1 video prepared. Error: {e.Message}");
                Debug.LogException(e);
            }
            
            // Step 14: Set panel5 video prepared
            currentStep++;
            Debug.Log($"[DressApp] Preparing assets... Step {currentStep}/{totalSteps} (Panel 5 Video Ready) - {(currentStep * 100f / totalSteps):F1}%");
            try
            {
                panel5.videoPrepared = true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[DressApp] ❌ ERROR in Step {currentStep}: Failed to set Panel 5 video prepared. Error: {e.Message}");
                Debug.LogException(e);
            }
            
            // Step 15: Set global initialization flag
            currentStep++;
            Debug.Log($"[DressApp] Preparing assets... Step {currentStep}/{totalSteps} (Global Variables Set) - {(currentStep * 100f / totalSteps):F1}%");
            try
            {
                GlobalVariables.initiated2 = true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[DressApp] ❌ ERROR in Step {currentStep}: Failed to set global variables. Error: {e.Message}");
                Debug.LogException(e);
            }
            
            Debug.Log("[DressApp] ✅ Asset preparation completed successfully! All panels ready.");
        }
        catch (System.Exception generalException)
        {
            Debug.LogError($"[DressApp] 💥 CRITICAL ERROR during asset preparation at step {currentStep}/{totalSteps}. Error: {generalException.Message}");
            Debug.LogException(generalException);
            Debug.Log("[DressApp] ⚠️ Asset preparation process interrupted due to critical error.");
        }
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
