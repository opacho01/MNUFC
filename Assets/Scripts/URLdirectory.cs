using UnityEngine;

/// <summary>
/// Directory of URL for comunicate whit server.
/// </summary>
public static class URLdirectory
{
    /// <summary>
    /// URL from general server.
    /// </summary>
    public static string serverUrl = "https://whitelabelvendingmachine.com/";
    /// <summary>
    /// URL from videoUpload.
    /// </summary>
    public static string videoUploadUrl = "api/v1/assets/machine_auth/upload";
    /// <summary>
    /// URL to get the reward.
    /// </summary>
    public static string rewardUrl = "api/v1/games/machineauth";
    /// <summary>
    /// Theme id from the actual machine.
    /// </summary>
    public static string theme_id = "67f87cecd93218ba46bfada6";
    /// <summary>
    /// URL to get theme.
    /// </summary>
    public static string getTheme = "api/v1/themes/machineauth/";
    /// <summary>
    /// URL to get the machine data.
    /// </summary>
    public static string getMachineData = "api/v1/machines/hardware/machineauth/";
    /// <summary>
    /// URL to send metrics.
    /// </summary>
    public static string sendAnalitics = "api/v1/analytics/";
    /// <summary>
    /// URL to send if user allowed use from video.
    /// </summary>
    public static string sendAllowed = "api/v1/games/";
}
