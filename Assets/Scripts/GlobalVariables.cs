using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores global variables used throughout the application.
/// This class contains persistent configuration settings, game state data, and system-wide variables.
/// </summary>
public static class GlobalVariables
{
    /// <summary>
    /// Indicates the currently active screen index.
    /// </summary>
    public static byte ActualScreen = 0;

    /// <summary>
    /// Tracks the number of fully completed games.
    /// </summary>
    public static int fullGameFinished = 0;

    /// <summary>
    /// Tracks the number of unfinished games.
    /// </summary>
    public static int gameUnfinished = 0;

    /// <summary>
    /// Determines whether the user has granted necessary permissions.
    /// </summary>
    public static bool permission = false;

    /// <summary>
    /// Stores the selected player index.
    /// </summary>
    public static byte playerSelected = 0;

    /// <summary>
    /// Stores information about the player's prize.
    /// </summary>
    public static InfoPrize infoPrize;

    /// <summary>
    /// Secret key for machine authentication or secure requests.
    /// </summary>
    public static string machinesSecretKey = "GIm3gPiGsZiJiog8oZK32N4vorNq35HWNkC3j8Ktk9G6IvmU8ehbur5U36GH7ySfj7+o+27FU1DQ6woM1KyXtQ==";

    /// <summary>
    /// Primary UI color used in the application.
    /// </summary>
    public static Color colorPrimary;

    /// <summary>
    /// Secondary UI color used in the application.
    /// </summary>
    public static Color colorSecondary;

    /// <summary>
    /// Background UI color used in the application.
    /// </summary>
    public static Color colorBackground;

    /// <summary>
    /// Text UI color used in the application.
    /// </summary>
    public static Color colorText;

    /// <summary>
    /// Dictionary containing step data used for process tracking or navigation.
    /// </summary>
    public static Dictionary<string, StepData> stepDictionary;

    /// <summary>
    /// Indicates whether the game/system has started.
    /// </summary>
    public static bool iniciado = false;

    /// <summary>
    /// Secondary initialization flag.
    /// </summary>
    public static bool initiated2 = false;

    /// <summary>
    /// Identifies the machine in use.
    /// </summary>
    public static string machineId = "HW-001";

    /// <summary>
    /// Stores the unique ID for tracking purposes.
    /// </summary>
    public static string _id = "";

    /// <summary>
    /// Stores system-wide metrics data.
    /// </summary>
    public static MetricsObj metricsObj;

    /// <summary>
    /// Stores machine-related data configurations.
    /// </summary>
    public static MachineData machineData;

    /// <summary>
    /// Stores the response details for video uploads.
    /// </summary>
    public static VideoUploadResponse videoUpload;

    /// <summary>
    /// Defines a general waiting time for delays or cooldowns.
    /// </summary>
    public static float generalAwait = 1;

    /// <summary>
    /// Indicates whether a permission or setting has changed.
    /// </summary>
    public static bool allowedChanged = false;
}
