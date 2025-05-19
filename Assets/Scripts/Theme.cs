using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a theme containing asset information for different steps in the game workflow.
/// </summary>
[System.Serializable]
public class Theme
{
    /// <summary>
    /// The name of the theme.
    /// </summary>
    public string name;

    /// <summary>
    /// A brief description of the theme.
    /// </summary>
    public string description;

    /// <summary>
    /// Global settings for the theme, including colors and assets.
    /// </summary>
    public Step0GlobalSettings step_0_global_settings;

    /// <summary>
    /// Settings for the attract video step.
    /// </summary>
    public Step1AttractVideo step_1_attract_video;

    /// <summary>
    /// Settings for the player selection screen step.
    /// </summary>
    public Step2SelectScreen step_2_select_screen;

    /// <summary>
    /// Settings for the player video preview step.
    /// </summary>
    public Step3PlayerVideoPreview step_3_player_video_preview;

    /// <summary>
    /// Settings for the countdown step before recording.
    /// </summary>
    public Step4Countdown step_4_countdown;

    /// <summary>
    /// Settings for the video recording step.
    /// </summary>
    public Step5CameraRecord step_5_camera_record;

    /// <summary>
    /// Settings for video upload step.
    /// </summary>
    public Step6VideoUpload step_6_video_upload;

    /// <summary>
    /// Settings for the celebration screen step.
    /// </summary>
    public Step7CelebrationScreen step_7_celebration_screen;

    /// <summary>
    /// Settings for the prize screen step.
    /// </summary>
    public Step8PrizeScreen step_8_prize_screen;

    /// <summary>
    /// Settings for the QR code screen step.
    /// </summary>
    public Step9QrScreen step_9_qr_screen;

    /// <summary>
    /// Unique identifier for the theme.
    /// </summary>
    public string _id;

    /// <summary>
    /// Timestamp indicating when the theme was created.
    /// </summary>
    public string created_at;

    /// <summary>
    /// Timestamp indicating the last update to the theme.
    /// </summary>
    public string updated_at;

    /// <summary>
    /// Identifier of the user who created the theme.
    /// </summary>
    public string created_by;

    /// <summary>
    /// Identifier of the user who last updated the theme.
    /// </summary>
    public string updated_by;
}

/// <summary>
/// Defines global settings such as template colors and UI assets.
/// </summary>
[System.Serializable]
public class Step0GlobalSettings
{
    /// <summary>
    /// Primary template color setting.
    /// </summary>
    public string template_color_1;

    /// <summary>
    /// Secondary template color setting.
    /// </summary>
    public string template_color_2;

    /// <summary>
    /// Tertiary template color setting.
    /// </summary>
    public string template_color_3;

    /// <summary>
    /// Quaternary template color setting.
    /// </summary>
    public string template_color_4;

    /// <summary>
    /// The asset used for the home button in the UI.
    /// </summary>
    public string home_button_asset;

    /// <summary>
    /// The banner asset displayed in the footer section.
    /// </summary>
    public string footer_banner_asset;

    /// <summary>
    /// The logo asset displayed in the footer section.
    /// </summary>
    public string footer_logo_asset;
}

/// <summary>
/// Defines settings for the attract video screen.
/// </summary>
[System.Serializable]
public class Step1AttractVideo
{
    /// <summary>
    /// The background video asset used for this step.
    /// </summary>
    public string background_video_asset;

    /// <summary>
    /// The logo asset displayed on the screen.
    /// </summary>
    public string logo_asset;

    /// <summary>
    /// The touch button asset used for user interaction.
    /// </summary>
    public string touch_button_asset;

    /// <summary>
    /// The text displayed during step 1.
    /// </summary>
    public string step1_text;

    /// <summary>
    /// A clean version of the footer logo used in the UI.
    /// </summary>
    public string footer_logo_clean;

    /// <summary>
    /// The audio clip played during step 1.
    /// </summary>
    public string screenAudioP1;

}

/// <summary>
/// Defines settings for the player selection screen.
/// </summary>
[System.Serializable]
public class Step2SelectScreen
{
    /// <summary>
    /// The background video asset used for the selection screen.
    /// </summary>
    public string background_video_asset;

    /// <summary>
    /// An array of player video previews available for selection.
    /// </summary>
    public PlayerVideoPreview[] player_video_previews;

    /// <summary>
    /// The text displayed in the top name banner.
    /// </summary>
    public string top_name_banner_text;

    /// <summary>
    /// An array of container video assets associated with the selection.
    /// </summary>
    public string[] container_video_asset;

    /// <summary>
    /// The text displayed in the bottom name banner.
    /// </summary>
    public string bottom_name_banner_text;

    /// <summary>
    /// The guide banner asset providing instructions on the selection screen.
    /// </summary>
    public string guide_banner_asset;

    /// <summary>
    /// The audio clip played during the selection screen.
    /// </summary>
    public string screenAudioP2;
}

/// <summary>
/// Represents video preview assets for the selection screen.
/// </summary>
[System.Serializable]
public class PlayerVideoPreview
{
    /// <summary>
    /// The asset representing the video preview.
    /// </summary>
    public string video_preview_asset;

    /// <summary>
    /// The main banner text displayed above the video preview.
    /// </summary>
    public string main_banner_text;

    /// <summary>
    /// The secondary banner text displayed below the video preview.
    /// </summary>
    public string secondary_banner_text;

}

/// <summary>
/// Defines settings for the player video preview step.
/// </summary>
[System.Serializable]
public class Step3PlayerVideoPreview
{
    /// <summary>
    /// The background video asset used for this step.
    /// </summary>
    public string background_video_asset;

    /// <summary>
    /// An array of player videos available for selection.
    /// </summary>
    public PlayerVideo[] player_videos;

    /// <summary>
    /// The asset representing the video preview.
    /// </summary>
    public string video_preview_asset;

    /// <summary>
    /// The main banner text displayed above the video preview.
    /// </summary>
    public string main_banner_text;

    /// <summary>
    /// The secondary banner text displayed below the video preview.
    /// </summary>
    public string secondary_banner_text;

    /// <summary>
    /// An array of audio file names associated with the video previews.
    /// </summary>
    public string[] audio_names;

    /// <summary>
    /// The audio clip played during this step.
    /// </summary>
    public string screenAudioP3;

}

/// <summary>
/// Represents player videos and their associated assets.
/// </summary>
[System.Serializable]
public class PlayerVideo
{
    /// <summary>
    /// The video asset associated with this step.
    /// </summary>
    public string video_asset;

    /// <summary>
    /// The main banner text displayed above the video.
    /// </summary>
    public string main_banner_text;

    /// <summary>
    /// The secondary banner text displayed below the video.
    /// </summary>
    public string secondary_banner_text;

    /// <summary>
    /// The audio asset linked to the video playback.
    /// </summary>
    public string audio_asset;
}

/// <summary>
/// Defines settings for the countdown step before recording.
/// </summary>
[System.Serializable]
public class Step4Countdown
{
    /// <summary>
    /// The background video asset used for the countdown screen.
    /// </summary>
    public string background_video_asset;

    /// <summary>
    /// The banner text displayed during the first stage of the countdown.
    /// </summary>
    public string countdown_banner_text_1;

    /// <summary>
    /// The banner text displayed during the second stage of the countdown.
    /// </summary>
    public string countdown_banner_text_2;

    /// <summary>
    /// The banner text displayed during the final stage of the countdown.
    /// </summary>
    public string countdown_banner_text_3;

    /// <summary>
    /// The audio asset played for the "Ready" cue in the countdown sequence.
    /// </summary>
    public string readyAudio;

    /// <summary>
    /// The audio asset played for the "Set" cue in the countdown sequence.
    /// </summary>
    public string setAudio;

    /// <summary>
    /// The audio asset played for the "Flex" cue in the countdown sequence.
    /// </summary>
    public string flexAudio;

}

/// <summary>
/// Defines settings for the camera recording step.
/// </summary>
[System.Serializable]
public class Step5CameraRecord
{
    /// <summary>
    /// The duration final from video.
    /// </summary>
    public float video_duration_seconds;
}

/// <summary>
/// Defines settings for the video upload step.
/// </summary>
[System.Serializable]
public class Step6VideoUpload
{
    /// <summary>
    /// The background video asset used for the loading screen.
    /// </summary>
    public string background_video_asset;

    /// <summary>
    /// The banner text displayed during the loading process.
    /// </summary>
    public string loading_banner_text;

    /// <summary>
    /// An array of assets used for loading animations.
    /// </summary>
    public string[] loading_animation_asset;

    /// <summary>
    /// The audio clip played during the loading screen.
    /// </summary>
    public string screenAudioP6;

}

/// <summary>
/// Defines settings for the celebration screen step.
/// </summary>
[System.Serializable]
public class Step7CelebrationScreen
{
    /// <summary>
    /// The background video asset used for the celebration screen.
    /// </summary>
    public string background_video_asset;

    /// <summary>
    /// The message text displayed during the celebration.
    /// </summary>
    public string celebration_message_text;

    /// <summary>
    /// An array of assets used for celebration animations.
    /// </summary>
    public string[] celebration_animation_asset;

    /// <summary>
    /// The audio clip played during the celebration screen.
    /// </summary>
    public string screenAudioP7;

}

/// <summary>
/// Defines settings for the prize screen step.
/// </summary>
[System.Serializable]
public class Step8PrizeScreen
{
    /// <summary>
    /// The background video asset used for the prize screen.
    /// </summary>
    public string background_video_asset;

    /// <summary>
    /// The message text displayed for the winner announcement.
    /// </summary>
    public string winner_message_text;

    /// <summary>
    /// The banner text highlighting the prize details.
    /// </summary>
    public string prize_banner_text;

    /// <summary>
    /// The disclaimer text displayed in the prize section.
    /// </summary>
    public string disclaimer_banner_text;

    /// <summary>
    /// An array of assets used for reward animations.
    /// </summary>
    public string[] reward_animation_asset;

    /// <summary>
    /// An additional array of reward animation assets for variations.
    /// </summary>
    public string[] reward_animation_asset1;

    /// <summary>
    /// A secondary array of reward animation assets for alternative effects.
    /// </summary>
    public string[] reward_animation_asset2;

    /// <summary>
    /// The audio clip played during the prize screen.
    /// </summary>
    public string screenAudioP8;

}

/// <summary>
/// Defines settings for the QR code screen step.
/// </summary>
[System.Serializable]
public class Step9QrScreen
{
    /// <summary>
    /// The background video asset used for the QR display screen.
    /// </summary>
    public string background_video_asset;

    /// <summary>
    /// The banner text displaying a thank-you message.
    /// </summary>
    public string thank_you_banner_text;

    /// <summary>
    /// The banner text providing instructions on the QR display screen.
    /// </summary>
    public string instructions_banner_text;

    /// <summary>
    /// The placeholder for the generated QR code.
    /// </summary>
    public string qr_code_placeholder;

    /// <summary>
    /// The asset used for the check banner display.
    /// </summary>
    public string check_banner_asset;

    /// <summary>
    /// The audio clip played during the QR display screen.
    /// </summary>
    public string screenAudioP9;

}