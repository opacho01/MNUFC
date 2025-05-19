using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents metrics data related to player activity during an event.
/// </summary>
[System.Serializable]
public class MetricsObj
{
    /// <summary>
    /// Timestamp marking the start of the event.
    /// </summary>
    public string time_start;

    /// <summary>
    /// Timestamp marking when the event was stopped.
    /// </summary>
    public string time_stoped;

    /// <summary>
    /// Timestamp marking when the event was completed.
    /// </summary>
    public string time_completed;

    /// <summary>
    /// Identifier of the selected player.
    /// </summary>
    public string player_selected;

    /// <summary>
    /// Indicates whether the player was allowed to participate.
    /// </summary>
    public bool allowed;

    /// <summary>
    /// Unique identifier for the event.
    /// </summary>
    public string event_id;

    /// <summary>
    /// Contains step-by-step tracking data for the event.
    /// </summary>
    public StepsWrapper steps;
}

/// <summary>
/// Wraps step data for tracking player interactions throughout different stages of the event.
/// </summary>
[System.Serializable]
public class StepsWrapper
{
    /// <summary>
    /// Step 1 data.
    /// </summary>
    public StepData _1;

    /// <summary>
    /// Step 2 data.
    /// </summary>
    public StepData _2;

    /// <summary>
    /// Step 3 data.
    /// </summary>
    public StepData _3;

    /// <summary>
    /// Step 4 data.
    /// </summary>
    public StepData _4;

    /// <summary>
    /// Step 5 data.
    /// </summary>
    public StepData _5;

    /// <summary>
    /// Step 6 data.
    /// </summary>
    public StepData _6;

    /// <summary>
    /// Step 7 data.
    /// </summary>
    public StepData _7;

    /// <summary>
    /// Step 8 data.
    /// </summary>
    public StepData _8;

    /// <summary>
    /// Step 9 data.
    /// </summary>
    public StepData _9;

    /// <summary>
    /// Initializes step data from a provided dictionary.
    /// </summary>
    /// <param name="stepDict">A dictionary containing step numbers as keys and corresponding step data as values.</param>
    public StepsWrapper(Dictionary<string, StepData> stepDict)
    {
        _1 = stepDict.ContainsKey("1") ? stepDict["1"] : new StepData();
        _2 = stepDict.ContainsKey("2") ? stepDict["2"] : new StepData();
        _3 = stepDict.ContainsKey("3") ? stepDict["3"] : new StepData();
        _4 = stepDict.ContainsKey("4") ? stepDict["4"] : new StepData();
        _5 = stepDict.ContainsKey("5") ? stepDict["5"] : new StepData();
        _6 = stepDict.ContainsKey("6") ? stepDict["6"] : new StepData();
        _7 = stepDict.ContainsKey("7") ? stepDict["7"] : new StepData();
        _8 = stepDict.ContainsKey("8") ? stepDict["8"] : new StepData();
        _9 = stepDict.ContainsKey("9") ? stepDict["9"] : new StepData();
    }
}

/// <summary>
/// Represents data for a specific step in the event tracking process.
/// </summary>
[System.Serializable]
public class StepData
{
    /// <summary>
    /// The step number within the sequence.
    /// </summary>
    public int step_number;

    /// <summary>
    /// The heatmap points associated with this step, representing user interactions or activity locations.
    /// </summary>
    public List<HeatmapPoint> heatmap;
}

/// <summary>
/// Represents a point in a heatmap, used to track player movements or interactions in three-dimensional space.
/// </summary>
[System.Serializable]
public class HeatmapPoint
{
    /// <summary>
    /// The X-coordinate of the heatmap point.
    /// </summary>
    public float x;

    /// <summary>
    /// The Y-coordinate of the heatmap point.
    /// </summary>
    public float y;

    /// <summary>
    /// The Z-coordinate of the heatmap point.
    /// </summary>
    public float z;
}
