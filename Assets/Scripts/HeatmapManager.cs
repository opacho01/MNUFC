using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class responsible for checking the touches on the screen and saving them in the object that contains the heatmaps. 
/// </summary>
public class HeatmapManager : MonoBehaviour
{
    public GameObject panelLoading;

    /// <summary>
    /// Saves the coordinates of the touches on the screen if you are not on the panelLoading.
    /// </summary>
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            try
            {
                if (!panelLoading.activeSelf)//Check if you are not in to panelLoading.
                {
                    Vector2 pixelPosition = Input.mousePosition;
                    GlobalVariables.stepDictionary[GlobalVariables.ActualScreen.ToString()].heatmap.Add(new HeatmapPoint { x = pixelPosition.x / Screen.width, y = pixelPosition.y / Screen.height, z = 0.0f });
                }
            }
            catch (Exception e) { Debug.Log(GlobalVariables.ActualScreen.ToString() + " Error Heatmap " + e); }
        }
    }
}
