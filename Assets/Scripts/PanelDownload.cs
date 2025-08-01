using UnityEngine;

/// <summary>
/// Class in charge of managing the PanelDownload
/// </summary>
public class PanelDownload : PanelContent
{
    private float initTime;
    private float endTime;
    public Metrics metrics;

    /// <summary>
    /// On enable panel set the ActualScreen to 0, and take the init time to metrics.
    /// </summary>
    private void OnEnable()
    {
        GlobalVariables.ActualScreen = 0;
        initTime = Time.time;
    }

    /// <summary>
    /// On disable take the end time to metrics.
    /// </summary>
    private void OnDisable()
    {
        endTime = Time.time;
        //Debug.Log("Tiempo total en primer pantalla :" + (endTime - initTime));
    }

    /// <summary>
    /// Send total time in screen to metrics.
    /// </summary>
    /// <param name="metric">Variable name</param>
    /// <param name="time">total time</param>
    public void sendTime(string metric, float time)
    {
        metrics.timeInScreen(metric, time);
    }
}
