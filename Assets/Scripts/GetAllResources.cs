using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;
using Debug = UnityEngine.Debug;

/// <summary>
/// Class responsible for obtaining all resources stored in themes from the server
/// </summary>
public class GetAllResources : MonoBehaviour
{
    //Global Assets
    /// <summary>
    /// The home button texture used in the UI.
    /// </summary>
    public Texture2D houseButton;

    /// <summary>
    /// The footer banner texture displayed at the bottom of the screen.
    /// </summary>
    public Texture2D footerBanner;

    /// <summary>
    /// The footer logo texture displayed alongside branding elements.
    /// </summary>
    public Texture2D footerLogo;

    //Attract Video
    /// <summary>
    /// The background video file for the attract screen.
    /// </summary>
    public string appBackground;

    /// <summary>
    /// The touch button texture used in the attract screen.
    /// </summary>
    public Texture2D touchButton;

    /// <summary>
    /// The logo texture displayed on the attract screen.
    /// </summary>
    public Texture2D appLogo;

    /// <summary>
    /// The audio clip played on the attract screen.
    /// </summary>
    public AudioClip screenAudioP1;

    //Select Video
    /// <summary>
    /// The background video file for the player selection screen.
    /// </summary>
    public string backgroundP2;

    /// <summary>
    /// An array of video preview files for player selection.
    /// </summary>
    public string[] videoPlayersPre;

    /// <summary>
    /// The guide banner texture displayed in the selection screen.
    /// </summary>
    public Texture2D GuideBanner;

    /// <summary>
    /// The audio clip played during the player selection screen.
    /// </summary>
    public AudioClip screenAudioP2;

    //Video Preview
    /// <summary>
    /// The background video file for the video preview screen.
    /// </summary>
    public string backgroundP3;

    /// <summary>
    /// An array of video files used for player previews.
    /// </summary>
    public string[] videoPlayers;

    /// <summary>
    /// The audio clip played during the video preview screen.
    /// </summary>
    public AudioClip screenAudioP3;

    /// <summary>
    /// An array of additional audio clips used in the preview.
    /// </summary>
    public AudioClip[] audioPlayers = new AudioClip[2];

    //Countdown
    /// <summary>
    /// The background video file for the countdown screen.
    /// </summary>
    public string backgroundP4;

    /// <summary>
    /// The audio clip played for the "Ready" cue during countdown.
    /// </summary>
    public AudioClip readyAudio;

    /// <summary>
    /// The audio clip played for the "Set" cue during countdown.
    /// </summary>
    public AudioClip setAudio;

    /// <summary>
    /// The audio clip played for the "Flex" cue during countdown.
    /// </summary>
    public AudioClip flexAudio;

    //Video upload
    /// <summary>
    /// The background video file for the video upload screen.
    /// </summary>
    public string backgroundP6;

    /// <summary>
    /// An array of loading screen textures used during video upload.
    /// </summary>
    public Texture2D[] loadingImg;

    /// <summary>
    /// The audio clip played during the video upload screen.
    /// </summary>
    public AudioClip screenAudioP6;

    //Celebration
    /// <summary>
    /// The background video file for the celebration screen.
    /// </summary>
    public string backgroundP7;

    /// <summary>
    /// An array of animated textures for the celebration screen.
    /// </summary>
    public Texture2D[] teamPuppet3;

    /// <summary>
    /// The audio clip played during the celebration screen.
    /// </summary>
    public AudioClip screenAudioP7;

    //Prize
    /// <summary>
    /// The background video file for the prize screen.
    /// </summary>
    public string backgroundP8;
    /// <summary>
    /// An array of animated textures for puppet1 animations.
    /// </summary>
    public Texture2D[] teamPuppet1;
    /// <summary>
    /// An array of animated textures for puppet2 animations.
    /// </summary>
    public Texture2D[] teamPuppet2;
    /// <summary>
    /// An array of animated textures for puppetAll animations.
    /// </summary>
    public Texture2D[] teamPuppetAll;
    /// <summary>
    /// The audio clip played during the prize screen.
    /// </summary>
    public AudioClip screenAudioP8;

    //QR Display
    /// <summary>
    /// The background video file for the QR display screen.
    /// </summary>
    public string backgroundP9;

    /// <summary>
    /// The audio clip played during the QR display screen.
    /// </summary>
    public AudioClip screenAudioP9;

    /// <summary>
    /// The number of player celebrations available.
    /// </summary>
    public byte numCelebrations = 4;

    /// <summary>
    /// Total number of resources to be downloaded.
    /// </summary>
    private int totalResources = 22;

    /// <summary>
    /// Counter tracking the number of downloaded resources.
    /// </summary>
    private int downloadedResources = 0;

    /// <summary>
    /// Panel used for displaying download progress.
    /// </summary>
    public GameObject panelDownloading;

    /// <summary>
    /// Timestamp marking the start of resource downloads.
    /// </summary>
    private float initTime;

    /// <summary>
    /// Timestamp marking the end of resource downloads.
    /// </summary>
    private float endTime;

    /// <summary>
    /// UI slider representing the download progress.
    /// </summary>
    public Slider downloadBar;

    /// <summary>
    /// Text field displaying the download progress status.
    /// </summary>
    public TMP_Text downoloadingText;

    public TMP_Text succesfullText;
    public TMP_Text ErrorText;
    

    bool Dummys = true;

    public PrizeManager prizeManager;

    /// <summary>
    /// The first function to be executed calls GetMachineData(); when the application starts.
    /// </summary>
    void Start()
    {
        Application.targetFrameRate = 30;
        initError.gameObject.SetActive(false);
        //Debug.Log("GetAllResources " + GlobalVariables.offline);
        prizeManager.InitializeComplete();
        // Verificar si hay datos guardados localmente
        if (GlobalVariables.offline)
        {
            //offline = true;
            //Debug.Log("Modo offline activado - usando datos guardados localmente");
            if (PlayerPrefs.HasKey("Reset"))
            {
                if (PlayerPrefs.GetInt("Reset") == 1)
                {
                    GetMachineData();
                } else
                {
                    LoadOfflineMachineData();
                }
            }
            else
            {
                LoadOfflineMachineData();
            }

        }
        else
        {
            //offline = false;
            //Debug.Log("Modo online - obteniendo datos del servidor");
            GetMachineData();
        }
        
    }

    /// <summary>
    /// Check if there is machine data saved locally for offline mode.
    /// </summary>
    /// <returns>True if offline data exists</returns>
    private bool CheckForOfflineData()
    {
        string machineDataPath = Path.Combine(Application.persistentDataPath, "machineData.json");
        return File.Exists(machineDataPath);
    }

    /// <summary>
    /// Load machine data from local storage for offline mode.
    /// </summary>
    private void LoadOfflineMachineData()
    {
        //////////////Aqui carga offline mode 
        DateTime Now = DateTime.Now;
        DateTime Saved;

        if (!PlayerPrefs.HasKey(GlobalVariables.ONLINE_DATE))
        {
            GlobalVariables.isValidTime = false;
            Saved = DateTime.Parse("04/04/1990 01:01:01 p. m.");
        } else
        {
            Saved = DateTime.Parse(PlayerPrefs.GetString(GlobalVariables.ONLINE_DATE, DateTime.Now.ToString()));
        }
        double DaysSince = (Now - Saved).TotalDays;
        if (Now < Saved)
        {
            GlobalVariables.isValidTime = false;
        } else if (DaysSince > GlobalVariables.DaysLastonline)
        {
            GlobalVariables.isValidTime = false;
        }
        else
        {
            GlobalVariables.isValidTime = true;
            if(!PlayerPrefs.HasKey(GlobalVariables.OFFLINE_DATE))
            {
                PlayerPrefs.SetString(GlobalVariables.OFFLINE_DATE, Now.ToString());
            }
            else if(DateTime.Parse(PlayerPrefs.GetString(GlobalVariables.OFFLINE_DATE, Now.ToString())) > Now)
            {
                GlobalVariables.isValidTime = false;
            }
        }
        if (GlobalVariables.isValidTime)
        {
            string machineDataPath = Path.Combine(Application.persistentDataPath, "machineData.json");

            try
            {
                if (File.Exists(machineDataPath))
                {
                    string savedData = File.ReadAllText(machineDataPath);
                    GlobalVariables.machineData = JsonUtility.FromJson<MachineData>(savedData);
                    themeData = GlobalVariables.machineData.theme;
                    URLdirectory.theme_id = themeData._id;

                    //Debug.Log("Datos de máquina cargados desde almacenamiento local");
                    DownloadAssets(); // Proceder a cargar los recursos offline
                }
                else
                {
                    //Debug.LogError("No se encontraron datos offline guardados");
                    initError.gameObject.SetActive(true);
                    initError.text = "No hay datos offline disponibles. Conéctese a internet para descargar los datos.";
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error cargando datos offline: " + e.Message);
                initError.gameObject.SetActive(true);
                initError.text = "Error cargando datos offline: " + e.Message;
            }
        }
        else
        {
            initError.text = "Please update your data using the online version before continuing to use the offline version.";
            initError.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Save machine data to local storage for offline use.
    /// </summary>
    /// <param name="machineDataJson">JSON string of machine data</param>
    private void SaveMachineDataForOffline(string machineDataJson)
    {
        try
        {
            string machineDataPath = Path.Combine(Application.persistentDataPath, "machineData.json");
            File.WriteAllText(machineDataPath, machineDataJson);
            Debug.Log("Datos de máquina guardados para uso offline");
        }
        catch (Exception e)
        {
            Debug.LogError("Error guardando datos offline: " + e.Message);
        }
    }

    /// <summary>
    /// Download all theme assets from the server and save in the machine.
    /// </summary>
    private void DownloadAssets()
    {
        if(!PlayerPrefs.HasKey("Reset"))
        {
            PlayerPrefs.SetInt("Reset", 0);
        }
        if(PlayerPrefs.GetInt("Reset") == 1)
        {
            //Debug.Log("Bajando todo el contenido");
            Dummys = false;
            PlayerPrefs.SetInt("Reset", 0);
        } else
        {
            //Debug.Log("Usando contenido precargado");
        }
        string extension = "";
        HttpManager.UpdateDisplay(succesfullText, ErrorText);
        //Initialize all arrays whit the final length.
        videoPlayersPre = new string[numCelebrations];
        videoPlayers = new string[numCelebrations];
        loadingImg = new Texture2D[themeData.step_6_video_upload.loading_animation_asset.Length];
        teamPuppet1 = new Texture2D[themeData.step_8_prize_screen.reward_animation_asset1.Length];
        teamPuppet2 = new Texture2D[themeData.step_8_prize_screen.reward_animation_asset2.Length];
        teamPuppetAll = new Texture2D[themeData.step_8_prize_screen.reward_animation_asset.Length];
        teamPuppet3 = new Texture2D[themeData.step_7_celebration_screen.celebration_animation_asset.Length];
        audioPlayers = new AudioClip[numCelebrations];
        initTime = Time.time;
        panelDownloading.SetActive(true);
        videoPlayersPre = new string[numCelebrations];
        videoPlayers = new string[numCelebrations];

        //Add totalResources whith the lengt of th arrays to block the downloading panel until everything is finished downloading.
        totalResources += videoPlayersPre.Length;
        totalResources += videoPlayers.Length;
        totalResources += loadingImg.Length;
        totalResources += teamPuppet1.Length;
        totalResources += teamPuppet2.Length;
        totalResources += teamPuppetAll.Length;
        totalResources += teamPuppet3.Length;
        totalResources += audioPlayers.Length;

        downloadBar.maxValue = totalResources;

        ///////////////Global Assets

        string houseButtonUrl = themeData.step_0_global_settings.home_button_asset;
        if (!Dummys)
        {
            HttpManager.GetTextureWithRetries(houseButtonUrl, "houseButton", (texture) => { SaveTexture(ref houseButton, texture, "houseButton"); });
        } else
        {
            houseButton = SaveImageWhithoutDownload("houseButton");
        }
        succesfullText.text = $"Downloading texture houseButton";

        string footerBannerUrl = themeData.step_0_global_settings.footer_banner_asset;
        if (!Dummys)
        {
            HttpManager.GetTextureWithRetries(footerBannerUrl, "footerBanner", (texture) => { SaveTexture(ref footerBanner, texture, "footerBanner"); });
        }
        else
        {
            footerBanner = SaveImageWhithoutDownload("footerBanner");
        }
        succesfullText.text = $"Downloading texture footerBanner";
        ///////////////Attract Video

        string appBackgroundUrl = themeData.step_1_attract_video.background_video_asset;
        if (!Dummys)
        {
            HttpManager.GetVideoWithRetries(appBackgroundUrl, "appBackground", (path) => { SaveVideoPath(ref appBackground, path, "appBackground"); });
        } else
        {
            appBackground = SaveVideoWhithoutDownload("appBackground");
        }
        succesfullText.text = $"Downloading video appBackground";

        string touchButtonUrl = themeData.step_1_attract_video.touch_button_asset;
        if (!Dummys)
        {
            HttpManager.GetTextureWithRetries(touchButtonUrl, "touchButton", (texture) => { SaveTexture(ref touchButton, texture, "touchButton"); });
        }
        else
        {
            touchButton = SaveImageWhithoutDownload("touchButton");
        }
        succesfullText.text = $"Downloading texture touchButton";

        string appLogoUrl = themeData.step_1_attract_video.logo_asset;
        if (!Dummys)
        {
            HttpManager.GetTextureWithRetries(appLogoUrl, "appLogo", (texture) => { SaveTexture(ref appLogo, texture, "appLogo"); });
        }
        else
        {
            appLogo = SaveImageWhithoutDownload("appLogo");
        }
        succesfullText.text = $"Downloading texture appLogo";
        string screenAudioP1Url = themeData.step_1_attract_video.screenAudioP1;
        extension = screenAudioP1Url.EndsWith(".wav") ? ".wav" : ".mp3";
        if (!Dummys)
        {
            HttpManager.GetAudioWithRetries(screenAudioP1Url, "screenAudioP1" + extension, (audio) => { SaveAudioPath(ref screenAudioP1, audio, "screenAudioP1"); });
        }
        else
        {
            LoadAudioFromFile("screenAudioP1", (loadedClip) =>
            {
                screenAudioP1 = loadedClip; 
            });
        }
        succesfullText.text = $"Downloading audio screenAudioP1";
        ///////////////Select Video

        string backgroundP2Url = themeData.step_2_select_screen.background_video_asset;
        if (!Dummys)
        {
            HttpManager.GetVideoWithRetries(backgroundP2Url, "backgroundP2", (path) => { SaveVideoPath(ref backgroundP2, path, "backgroundP2"); });
        }
        else
        {
            backgroundP2 = SaveVideoWhithoutDownload("backgroundP2");
        }
        succesfullText.text = $"Downloading video backgroundP2";
        for (byte i = 0; i < numCelebrations; i++)
        {
            byte index = i;
            videoPlayersPre[index] = themeData.step_2_select_screen.player_video_previews[i].video_preview_asset;
            string videoPreFileName = "videoPlayersPre" + index;
            if (!Dummys)
            {
                HttpManager.GetVideoWithRetries(videoPlayersPre[index], videoPreFileName, (path) => { SaveVideoPath(ref videoPlayersPre[index], path, "videoPlayersPre-" + index); });
            }
            else
            {
                videoPlayersPre[index] = SaveVideoWhithoutDownload("videoPlayersPre" + index);
            }
            succesfullText.text = $"Downloading video " + "videoPlayersPre" + index;
        }
        
        for (byte i = 0; i < numCelebrations; i++)
        {
            byte index = i;
            if (!Dummys)
            {
                string audioPlayersURL = themeData.step_3_player_video_preview.player_videos[i].audio_asset;
                extension = audioPlayersURL.EndsWith(".wav") ? ".wav" : ".mp3";
                HttpManager.GetAudioWithRetries(audioPlayersURL, "audioPlayers-" + index + extension, (audio) =>
                {
                    SaveAudioPath(ref audioPlayers[index], audio, "audioPlayers-" + index);
                });
            }
            else
            {
                LoadAudioFromFile("audioPlayers-" + index, (loadedClip) =>
                {
                    audioPlayers[index] = loadedClip;
                });
            }
            succesfullText.text = $"Downloading audio " + "audioPlayers-" + index;
        }

        string GuideBannerUrl = themeData.step_2_select_screen.guide_banner_asset;
        if (!Dummys)
        {
            HttpManager.GetTextureWithRetries(GuideBannerUrl, "GuideBanner", (texture) => { SaveTexture(ref GuideBanner, texture, "GuideBanner"); });
        }
        else
        {
            GuideBanner = SaveImageWhithoutDownload("GuideBanner");
        }
        succesfullText.text = $"Downloading texture GuideBanner";
        if (!Dummys)
        {
            string screenAudioP2Url = themeData.step_2_select_screen.screenAudioP2;
        extension = screenAudioP2Url.EndsWith(".wav") ? ".wav" : ".mp3";
        HttpManager.GetAudioWithRetries(screenAudioP2Url, "screenAudioP2" + extension, (audio) => { SaveAudioPath(ref screenAudioP2, audio, "screenAudioP2"); });
        }
        else
        {
            LoadAudioFromFile("screenAudioP2", (loadedClip) =>
            {
                screenAudioP2 = loadedClip;
            });
        }
        succesfullText.text = $"Downloading audio screenAudioP2";

        ///////////////Video Preview

        string backgroundP3Url = themeData.step_3_player_video_preview.background_video_asset;
        if (!Dummys)
        {
            HttpManager.GetVideoWithRetries(backgroundP3Url, "backgroundP3", (path) => { SaveVideoPath(ref backgroundP3, path, "backgroundP3"); });
        }
        else
        {
            backgroundP3 = SaveVideoWhithoutDownload("backgroundP3");
        }
        succesfullText.text = $"Downloading video backgroundP3";
        for (byte i = 0; i < numCelebrations; i++)
        {
            byte index = i;
            videoPlayers[index] = themeData.step_3_player_video_preview.player_videos[i].video_asset;
            string videoFileName = "videoPlayers" + index;
            if (!Dummys)
            {
                HttpManager.GetVideoWithRetries(videoPlayers[index], videoFileName, (path) => { SaveVideoPath(ref videoPlayers[index], path, "videoPlayers-" + index); });
            }
            else
            {
                videoPlayers[index] = SaveVideoWhithoutDownload("videoPlayers" + index);
            }
            succesfullText.text = $"Downloading video " + "videoPlayers" + index;
        }
        if (!Dummys)
        {
            string screenAudioP3Url = themeData.step_3_player_video_preview.screenAudioP3;
        extension = screenAudioP3Url.EndsWith(".wav") ? ".wav" : ".mp3";
        HttpManager.GetAudioWithRetries(screenAudioP3Url, "screenAudioP3" + extension, (audio) => { SaveAudioPath(ref screenAudioP3, audio, "screenAudioP3"); });
        }
        else
        {
            LoadAudioFromFile("screenAudioP3", (loadedClip) =>
            {
                screenAudioP3 = loadedClip;
            });
        }
        succesfullText.text = $"Downloading audio screenAudioP3";
        ///////////////Countdown

        string backgroundP4Url = themeData.step_4_countdown.background_video_asset;
        if (!Dummys)
        {
            HttpManager.GetVideoWithRetries(backgroundP4Url, "backgroundP4", (path) => { SaveVideoPath(ref backgroundP4, path, "backgroundP4"); });
        }
        else
        {
            backgroundP4 = SaveVideoWhithoutDownload("backgroundP4");
        }
        succesfullText.text = $"Downloading video backgroundP4";
        if (!Dummys)
        {
            string readyAudioUrl = themeData.step_4_countdown.readyAudio;
        extension = readyAudioUrl.EndsWith(".wav") ? ".wav" : ".mp3";
        HttpManager.GetAudioWithRetries(readyAudioUrl, "readyAudio" + extension, (audio) => { SaveAudioPath(ref readyAudio, audio, "readyAudio"); });
        }
        else
        {
            LoadAudioFromFile("readyAudio", (loadedClip) =>
            {
                readyAudio = loadedClip;
            });
        }
        succesfullText.text = $"Downloading audio readyAudio";
        if (!Dummys)
        {
            string setAudioUrl = themeData.step_4_countdown.setAudio;
        extension = setAudioUrl.EndsWith(".wav") ? ".wav" : ".mp3";
        HttpManager.GetAudioWithRetries(setAudioUrl, "setAudio" + extension, (audio) => { SaveAudioPath(ref setAudio, audio, "setAudio"); });
        }
        else
        {
            LoadAudioFromFile("setAudio", (loadedClip) =>
            {
                setAudio = loadedClip;
            });
        }
        succesfullText.text = $"Downloading audio setAudio";
        if (!Dummys)
        {
            string flexAudioUrl = themeData.step_4_countdown.flexAudio;
        extension = flexAudioUrl.EndsWith(".wav") ? ".wav" : ".mp3";
        HttpManager.GetAudioWithRetries(flexAudioUrl, "flexAudio" + extension, (audio) => { SaveAudioPath(ref flexAudio, audio, "flexAudio"); });
        }
        else
        {
            LoadAudioFromFile("flexAudio", (loadedClip) =>
            {
                flexAudio = loadedClip;
            });
        }
        succesfullText.text = $"Downloading audio flexAudio";
        ///////////////Video upload

        string backgroundP6Url = themeData.step_6_video_upload.background_video_asset;
        if (!Dummys)
        {
            HttpManager.GetVideoWithRetries(backgroundP6Url, "backgroundP6", (path) => { SaveVideoPath(ref backgroundP6, path, "backgroundP6"); });
        }
        else
        {
            backgroundP6 = SaveVideoWhithoutDownload("backgroundP6");
        }
        succesfullText.text = $"Downloading video backgroundP6";

        for (byte i = 0; i < themeData.step_6_video_upload.loading_animation_asset.Length; i++)
        {
            int index = i;
            string loadingImgUrl = themeData.step_6_video_upload.loading_animation_asset[i];
            if (!Dummys)
            {
                HttpManager.GetTextureWithRetries(loadingImgUrl, "loadingImg-" + index, (texture) =>
            {
                SaveTexture(ref loadingImg[index], texture, "loadingImg-" + index);
            });
            }
            else
            {
                loadingImg[index] = SaveImageWhithoutDownload("loadingImg-" + index);
            }
            succesfullText.text = $"Downloading texture " + "loadingImg-" + index;
        }
        if (!Dummys)
        {
            string screenAudioP6Url = themeData.step_6_video_upload.screenAudioP6;
        extension = screenAudioP6Url.EndsWith(".wav") ? ".wav" : ".mp3";
        HttpManager.GetAudioWithRetries(screenAudioP6Url, "screenAudioP6" + extension, (audio) => { SaveAudioPath(ref screenAudioP6, audio, "screenAudioP6"); });
        }
        else
        {
            LoadAudioFromFile("screenAudioP6", (loadedClip) =>
            {
                screenAudioP6 = loadedClip;
            });
        }
        succesfullText.text = $"Downloading audio screenAudioP6";
        ///////////////Celebration
        string backgroundP7Url = themeData.step_7_celebration_screen.background_video_asset;
        if (!Dummys)
        {
            HttpManager.GetVideoWithRetries(backgroundP7Url, "backgroundP7", (path) => { SaveVideoPath(ref backgroundP7, path, "backgroundP7"); });
        }
        else
        {
            backgroundP7 = SaveVideoWhithoutDownload("backgroundP7");
        }
        succesfullText.text = $"Downloading video backgroundP7";
        for (byte i = 0; i < themeData.step_7_celebration_screen.celebration_animation_asset.Length; i++)
        {
            int index = i;
            string teamPuppet3Url = themeData.step_7_celebration_screen.celebration_animation_asset[index];
            if (!Dummys)
            {
                HttpManager.GetTextureWithRetries(teamPuppet3Url, "teamPuppet3-" + index, (texture) =>
            {
                SaveTexture(ref teamPuppet3[index], texture, "teamPuppet3-" + index);
            });
            }
            else
            {
                teamPuppet3[index] = SaveImageWhithoutDownload("teamPuppet3-" + index);
            }
            succesfullText.text = $"Downloading texture " + "teamPuppet3-" + index;
        }
        if (!Dummys)
        {
            string screenAudioP7Url = themeData.step_7_celebration_screen.screenAudioP7;
        extension = screenAudioP7Url.EndsWith(".wav") ? ".wav" : ".mp3";
        HttpManager.GetAudioWithRetries(screenAudioP7Url, "screenAudioP7" + extension, (audio) => { SaveAudioPath(ref screenAudioP7, audio, "screenAudioP7"); });
        }
        else
        {
            LoadAudioFromFile("screenAudioP7", (loadedClip) =>
            {
                screenAudioP7 = loadedClip;
            });
        }
        succesfullText.text = $"Downloading audio screenAudioP7";
        ///////////////Prize

        string backgroundP8Url = themeData.step_8_prize_screen.background_video_asset;
        if (!Dummys)
        {
            HttpManager.GetVideoWithRetries(backgroundP8Url, "backgroundP8", (path) => { SaveVideoPath(ref backgroundP8, path, "backgroundP8"); });
        }
        else
        {
            backgroundP8 = SaveVideoWhithoutDownload("backgroundP8");
        }
        succesfullText.text = $"Downloading video backgroundP8";
        for (byte i = 0; i < themeData.step_8_prize_screen.reward_animation_asset1.Length; i++)
        {
                int index = i;
                string teamPuppet1Url = themeData.step_8_prize_screen.reward_animation_asset1[i];
            if (!Dummys)
            {
                HttpManager.GetTextureWithRetries(teamPuppet1Url, "teamPuppet1-" + index, (texture) =>
                {
                    SaveTexture(ref teamPuppet1[index], texture, "teamPuppet1-" + index);
                });
            }
            else
            {
                teamPuppet1[index] = SaveImageWhithoutDownload("teamPuppet1-" + index);
            }
            succesfullText.text = $"Downloading texture " + "teamPuppet1-" + index;
        }
       
        for (byte i = 0; i < themeData.step_8_prize_screen.reward_animation_asset2.Length; i++)
        {
                int index = i;
                string teamPuppet2Url = themeData.step_8_prize_screen.reward_animation_asset2[i];
            if (!Dummys)
            {
                HttpManager.GetTextureWithRetries(teamPuppet2Url, "teamPuppet2-" + index, (texture) =>
                {
                    SaveTexture(ref teamPuppet2[index], texture, "teamPuppet2-" + index);
                });
            }
            else
            {
                teamPuppet2[index] = SaveImageWhithoutDownload("teamPuppet2-" + index);
            }
            succesfullText.text = $"Downloading texture " + "teamPuppet2-" + index;
        }
        
        for (byte i = 0; i < themeData.step_8_prize_screen.reward_animation_asset.Length; i++)
        {
                int index = i;
                string teamPuppetAllUrl = themeData.step_8_prize_screen.reward_animation_asset[i];
            if (!Dummys)
            {
                HttpManager.GetTextureWithRetries(teamPuppetAllUrl, "teamPuppetAll-" + index, (texture) =>
                {
                    SaveTexture(ref teamPuppetAll[index], texture, "teamPuppetAll-" + index);
                });
            }
            else
            {
                teamPuppetAll[index] = SaveImageWhithoutDownload("teamPuppetAll-" + index);
            }
            succesfullText.text = $"Downloading texture " + "teamPuppetAll-" + index;
        }
        if (!Dummys)
        {
            string screenAudioP8Url = themeData.step_8_prize_screen.screenAudioP8;
        extension = screenAudioP8Url.EndsWith(".wav") ? ".wav" : ".mp3";
        HttpManager.GetAudioWithRetries(screenAudioP8Url, "screenAudioP8" + extension, (audio) => { SaveAudioPath(ref screenAudioP8, audio, "screenAudioP8"); });
        }
        else
        {
            LoadAudioFromFile("screenAudioP8", (loadedClip) =>
            {
                screenAudioP8 = loadedClip;
            });
        }
        succesfullText.text = $"Downloading audio screenAudioP8";
        ///////////////QR Display
        string backgroundP9Url = themeData.step_9_qr_screen.background_video_asset;
        if (!Dummys)
        {
            HttpManager.GetVideoWithRetries(backgroundP9Url, "backgroundP9", (path) => { SaveVideoPath(ref backgroundP9, path, "backgroundP9"); });
        }
        else
        {
            backgroundP9 = SaveVideoWhithoutDownload("backgroundP9");
        }
        succesfullText.text = $"Downloading video backgroundP9";
        if (!Dummys)
        {
            string screenAudioP9Url = themeData.step_9_qr_screen.screenAudioP9;
        extension = screenAudioP9Url.EndsWith(".wav") ? ".wav" : ".mp3";
        HttpManager.GetAudioWithRetries(screenAudioP9Url, "screenAudioP9" + extension, (audio) => { SaveAudioPath(ref screenAudioP9, audio, "screenAudioP9"); });
        }
        else
        {
            LoadAudioFromFile("screenAudioP9", (loadedClip) =>
            {
                screenAudioP9 = loadedClip;
            });
        }
        succesfullText.text = $"Downloading audio screenAudioP9";
        PlayerPrefs.SetInt("Reset", 0);
        Dummys = true;
    }

    /// <summary>
    /// Save to machine the texture whit the given name.
    /// </summary>
    /// <param name="textureVar">Variable where the image will be saved</param>
    /// <param name="downloadedTexture">Downloaded texture</param>
    /// <param name="fileName">name to save texture</param>
    private void SaveTexture(ref Texture2D textureVar, Texture2D downloadedTexture, string fileName)
    {
        if (downloadedTexture == null)
        {
            UnityEngine.Debug.LogError($"Could not download texture: {fileName} - Continuing without this resource");
            // Set a default/fallback texture if needed
            // textureVar = defaultTexture; // Uncomment if you have a default texture
        }
        else
        {
            textureVar = downloadedTexture; // Assigns the downloaded texture to the variable
            byte[] bytes = downloadedTexture.EncodeToPNG();
            string savePath = Path.Combine(Application.persistentDataPath, fileName + ".png");
            File.WriteAllBytes(savePath, bytes);
        }
        
        // Always increment counter regardless of success/failure to maintain progress
        downloadedResources++;//Increase the downloaded resources counter
        downoloadingText.text = downloadedResources + " / " + (totalResources + 1);
        CheckAllResourcesDownloaded();
    }




    /// <summary>
    /// Save to machine the video whit the given name.
    /// </summary>
    /// <param name="targetPath">variable where the path will be saved</param>
    /// <param name="path">path form the video</param>
    /// <param name="fileName">name to save the video</param>
    private void SaveVideoPath(ref string targetPath, string path, string fileName)
    {
        if (!string.IsNullOrEmpty(path))
        {
            targetPath = path; 
        } 
        else 
        { 
            UnityEngine.Debug.LogError($"Failed to download video: {fileName} - Continuing without this resource"); 
            // Leave targetPath as is (empty/null) - the application should handle missing videos gracefully
        } 
        
        // Always increment counter regardless of success/failure to maintain progress
        downloadedResources++;//Increase the downloaded resources counter
        downoloadingText.text = downloadedResources + " / " + (totalResources + 1); 
        CheckAllResourcesDownloaded();
    }

    private string SaveVideoWhithoutDownload(string fileName)
    {
        
        downloadedResources++;//Increase the downloaded resources counter
        downoloadingText.text = downloadedResources + " / " + (totalResources + 1);
        CheckAllResourcesDownloaded();
        return Path.Combine(Application.persistentDataPath, fileName + ".mp4");
    }

    private Texture2D SaveImageWhithoutDownload(string fileName)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName + ".png");

        if (File.Exists(path))
        {
            byte[] fileData = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2, 2); // tamaño inicial arbitrario

            if (texture.LoadImage(fileData))
            {
                downloadedResources++;//Increase the downloaded resources counter
                downoloadingText.text = downloadedResources + " / " + (totalResources + 1);
                CheckAllResourcesDownloaded();
                return texture;
            }
            else
            {
                UnityEngine.Debug.LogWarning("La imagen no pudo ser cargada desde: " + path);
            }
        }
        else
        {
            UnityEngine.Debug.LogWarning("El archivo no existe en: " + path);
        }

        return null;
    }

    /// <summary>
    /// Save to machine the audio whit the given name.
    /// </summary>
    /// <param name="clipVar">Variable where the audio will be saved</param>
    /// <param name="downloadedClip">Downloaded audio</param>
    /// <param name="fileName">name to save the audio</param>
    void SaveAudioPath(ref AudioClip clipVar, AudioClip downloadedClip, string fileName)
    {
        if (downloadedClip == null)
        {
            UnityEngine.Debug.LogError($"Failed to download audio: {fileName} - Continuing without this resource");
            // Leave clipVar as is (null) - the application should handle missing audio gracefully
        }
        else
        {
            clipVar = downloadedClip;
        }

        // Always increment counter regardless of success/failure to maintain progress
        downloadedResources++;//Increase the downloaded resources counter
        downoloadingText.text = downloadedResources + " / " + (totalResources + 1);
        CheckAllResourcesDownloaded();
    }


    public void LoadAudioFromFile(string fileName, System.Action<AudioClip> onAudioLoaded)
    {
        // Construye la ruta completa del archivo
        string filePath = Path.Combine(Application.persistentDataPath, fileName);

        // Intenta cargar como MP3 primero, luego WAV
        string finalPath = "";
        if (File.Exists(filePath + ".mp3"))
        {
            finalPath = filePath + ".mp3";
        }
        else if (File.Exists(filePath + ".wav"))
        {
            finalPath = filePath + ".wav";
        }
        else if (File.Exists(filePath + ".mp3.mp3"))
        {
            finalPath = filePath + ".mp3.mp3";
        }
        else if (File.Exists(filePath + ".wav.wav"))
        {
            finalPath = filePath + ".wav.wav";
        }
        else
        {
            Debug.LogError($"Audio file not found at: {filePath}.mp3 or {filePath}.wav");
            onAudioLoaded?.Invoke(null); // Llama al callback con null si no se encuentra
            return; // Sale de la función
        }

        // Para cargar audio local con UnityWebRequest, necesitamos usar un prefijo "file://"
        string uri = "file://" + finalPath;

        // Inicia la coroutine para la carga asíncrona del audio
        StartCoroutine(LoadAudioCoroutine(uri, fileName, onAudioLoaded)); // Pasa el callback directamente
    }
    private IEnumerator LoadAudioCoroutine(string uri, string fileName, System.Action<AudioClip> callback)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(uri, AudioType.UNKNOWN))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error loading audio '{fileName}' from URI '{uri}': {www.error}");
                callback?.Invoke(null); // Invoca el callback con null en caso de error
            }
            else
            {
                AudioClip myClip = DownloadHandlerAudioClip.GetContent(www);
                if (myClip != null)
                {
                    myClip.name = fileName; // Asigna el nombre al AudioClip para depuración
                    //Debug.Log($"Audio '{fileName}' loaded successfully.");
                    succesfullText.text = $"Audio '{fileName}' loaded successfully.";
                    downloadedResources++;//Increase the downloaded resources counter
                    downoloadingText.text = downloadedResources + " / " + (totalResources + 1);
                    CheckAllResourcesDownloaded();
                }
                else
                {
                    Debug.LogError($"Failed to get AudioClip content from '{fileName}' URI '{uri}'.");
                }
                callback?.Invoke(myClip); // Invoca el callback con el AudioClip cargado
            }
        }
    }


    private VideoPlayer.EventHandler onPrepared;

    /// <summary>
    /// Prepare and play video, save the last frame in a texture and set it as the starting frame for the video.
    /// </summary>
    /// <param name="path">direction of video in the machine</param>
    /// <param name="videoPlayer">videoPlayer to play video</param>
    /// <param name="panel">panel containing the video</param>
    public void PlayVideo(string path, VideoPlayer videoPlayer, GameObject panel)
    {
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = "file://" + path;
        videoPlayer.waitForFirstFrame = true;
        videoPlayer.Prepare();

        videoPlayer.prepareCompleted -= onPrepared;//Ensure there are no duplicate events

        // Define the event with the correct signature
        onPrepared = (VideoPlayer vp) => {
            vp.Play();
            if (!panel.GetComponent<PanelContent>().videoPrepared)
            {
                //Copies the last frame of the video and sets it as the starting frame.
                RenderTexture.active = videoPlayer.targetTexture;
                Texture2D lastFrameTexture = new Texture2D(videoPlayer.targetTexture.width, videoPlayer.targetTexture.height, TextureFormat.RGB24, false);
                lastFrameTexture.ReadPixels(new Rect(0, 0, lastFrameTexture.width, lastFrameTexture.height), 0, 0);
                lastFrameTexture.Apply();
                RenderTexture.active = null;
                Graphics.Blit(lastFrameTexture, videoPlayer.targetTexture);
                panel.GetComponent<PanelContent>().videoPrepared = true;
                videoPlayer.prepareCompleted -= onPrepared; // Remove the event after it is executed
            }
        };
        /*videoPlayer.errorReceived += (VideoPlayer vp, string message) => {
            if (videoPlayer.url.Length > 10)
            {
                UnityEngine.Debug.LogError("VideoPlayer error: " + message);
            }
        };*/
    }

    /// <summary>
    /// Prepare and play secondary video in a panel, save the last frame in a texture and set it as the starting frame for the video.
    /// </summary>
    /// <param name="path">direction of video in the machine</param>
    /// <param name="videoPlayer">videoPlayer to play video</param>
    public void PlayVideo(string path, VideoPlayer videoPlayer)
    {
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = "file://" + path;
        videoPlayer.Prepare();

        videoPlayer.prepareCompleted += (VideoPlayer vp) => {
           
                // Capturar el último frame antes de desactivar el panel
                RenderTexture.active = videoPlayer.targetTexture;
                Texture2D lastFrameTexture = new Texture2D(videoPlayer.targetTexture.width, videoPlayer.targetTexture.height, TextureFormat.RGB24, false);
                lastFrameTexture.ReadPixels(new Rect(0, 0, lastFrameTexture.width, lastFrameTexture.height), 0, 0);
                lastFrameTexture.Apply();
                RenderTexture.active = null;

                // Asignar el último frame al RenderTexture del VideoPlayer
                Graphics.Blit(lastFrameTexture, videoPlayer.targetTexture);

                vp.Play();
        };
    }

    public GameObject buttonForce;
    /// <summary>
    /// Check if downloading and saved all resources.
    /// </summary>
    void CheckAllResourcesDownloaded()
    {
        downloadBar.value = downloadedResources;
        if (downloadedResources > totalResources)
        {
            //UnityEngine.Debug.Log("All resources have been downloaded and saved.");
            OnAllResourcesDownloaded();
        }else if(downloadedResources > totalResources - 20)
        {
            //buttonForce.SetActive(true);
        }
    }

    /// <summary>
    /// The primary color used in the theme.
    /// </summary>
    public Color color1;

    /// <summary>
    /// The secondary color used in the theme.
    /// </summary>
    public Color color2;

    /// <summary>
    /// The tertiary color used in the theme.
    /// </summary>
    public Color color3;

    /// <summary>
    /// The quaternary color used in the theme.
    /// </summary>
    public Color color4;

    /// <summary>
    /// When all resources are downloading and saved asign to global variables the color of theme, and call the DressAll function.
    /// </summary>
    void OnAllResourcesDownloaded()
    {
        string newColor;
        try { newColor = themeData.step_0_global_settings.template_color_1; } catch (Exception e) { newColor = "#ff0000"; }
        ColorUtility.TryParseHtmlString(newColor, out GlobalVariables.colorPrimary);
        color1 = GlobalVariables.colorPrimary;

        string newColor1;
        try { newColor1 = themeData.step_0_global_settings.template_color_2; } catch (Exception e) { newColor1 = "#ff0000"; }
        ColorUtility.TryParseHtmlString(newColor1, out GlobalVariables.colorSecondary);
        color2 = GlobalVariables.colorSecondary;

        string newColor2;
        try { newColor2 = themeData.step_0_global_settings.template_color_3; } catch (Exception e) { newColor2 = "#ff0000"; }
        ColorUtility.TryParseHtmlString(newColor2, out GlobalVariables.colorBackground);
        color3 = GlobalVariables.colorBackground;

        string newColor3;
        try { newColor3 = themeData.step_0_global_settings.template_color_4; } catch (Exception e) { newColor3 = "#ff0000"; }
        ColorUtility.TryParseHtmlString(newColor3, out GlobalVariables.colorText);
        color4 = GlobalVariables.colorText;

        endTime = Time.time;
        panelDownloading.GetComponent<PanelDownload>().sendTime("DownloadTime", (endTime - initTime));
        GetComponent<DressApp>().DressAll();
    }

    /// <summary>
    /// Get all machine data form the server.
    /// </summary>
    private void GetMachineData()
    {
        HttpManager.AddRequestHeader("X-Machine-Key", GlobalVariables.machinesSecretKey);
        HttpManager.Get(URLdirectory.getMachineData + GlobalVariables.machineId, MachineDataComplete);
        if (PlayerPrefs.HasKey(GlobalVariables.ONLINE_DATE))
        {
            string fechaStr = PlayerPrefs.GetString(GlobalVariables.ONLINE_DATE, DateTime.Now.ToString());
            GlobalVariables.lastOnlineDate = DateTime.Parse(fechaStr);
            if(GlobalVariables.lastOnlineDate.Date < DateTime.Now.Date)
            {
                PlayerPrefs.SetString(GlobalVariables.ONLINE_DATE, DateTime.Now.Date.ToString());
            }
        }
        else {
            PlayerPrefs.SetString(GlobalVariables.ONLINE_DATE, DateTime.Now.Date.ToString());
            GlobalVariables.lastOnlineDate = DateTime.Now.Date;
        }
    }

    /// <summary>
    /// Get complete theme from the server whit the theme_id. 
    /// </summary>
    private void GetTheme()
    {   
        HttpManager.AddRequestHeader("X-Machine-Key", GlobalVariables.machinesSecretKey);
        HttpManager.Get(URLdirectory.getTheme + URLdirectory.theme_id, ThemeComplete);
    }
    /// <summary>
    /// Object themeData contains all links to resources in the server.
    /// </summary>
    public Theme themeData;

    /// <summary>
    /// Callback from GetTheme assign the number of number celebrations in the game and call the DownloadAssets function.
    /// </summary>
    /// <param name="response"></param>
    private void ThemeComplete(string response)
    {
        //UnityEngine.Debug.Log("--------------------------------\n" + response + "\n----------------------------------");
        themeData = JsonUtility.FromJson<Theme>(response);
        numCelebrations = 0;
        try
        {
            //count the number of celebrations in theme
            for(byte i = 0; i < (byte)themeData.step_2_select_screen.player_video_previews.Length; i++)
            {
                if((byte)themeData.step_2_select_screen.player_video_previews[i].video_preview_asset.Length > 2)
                {
                    numCelebrations++;
                }
            }

        } catch (Exception e)
        {
            numCelebrations = 2;
        }
        try
        {
            if (themeData.step_7_celebration_screen.celebration_animation_asset?.Length == 0)
            {
                themeData.step_7_celebration_screen.celebration_animation_asset = new string[1];
                themeData.step_7_celebration_screen.celebration_animation_asset[0] = "https://1000marcas.net/wp-content/uploads/2020/01/Juventus-logo.png";
            }
            if (themeData.step_6_video_upload.loading_animation_asset?.Length == 0)
            {
                themeData.step_6_video_upload.loading_animation_asset = new string[1];
                themeData.step_6_video_upload.loading_animation_asset[0] = "https://1000marcas.net/wp-content/uploads/2020/01/Juventus-logo.png";
            }
            if (themeData.step_8_prize_screen.reward_animation_asset1?.Length == 0)
            {
                themeData.step_8_prize_screen.reward_animation_asset1 = new string[1];
                themeData.step_8_prize_screen.reward_animation_asset1[0] = "https://1000marcas.net/wp-content/uploads/2020/01/Juventus-logo.png";
            }
            if (themeData.step_8_prize_screen.reward_animation_asset2?.Length == 0)
            {
                themeData.step_8_prize_screen.reward_animation_asset2 = new string[1];
                themeData.step_8_prize_screen.reward_animation_asset2[0] = "https://1000marcas.net/wp-content/uploads/2020/01/Juventus-logo.png";
            }
            if (themeData.step_8_prize_screen.reward_animation_asset?.Length == 0)
            {
                themeData.step_8_prize_screen.reward_animation_asset = new string[1];
                themeData.step_8_prize_screen.reward_animation_asset[0] = "https://1000marcas.net/wp-content/uploads/2020/01/Juventus-logo.png";
            }
        }
        catch (Exception e)
        {
            themeData = new Theme();
            themeData.step_7_celebration_screen = new Step7CelebrationScreen();
            themeData.step_6_video_upload = new Step6VideoUpload();
            themeData.step_8_prize_screen = new Step8PrizeScreen();
            themeData.step_7_celebration_screen.celebration_animation_asset = new string[1];
            themeData.step_7_celebration_screen.celebration_animation_asset[0] = "https://1000marcas.net/wp-content/uploads/2020/01/Juventus-logo.png";

            themeData.step_6_video_upload.loading_animation_asset = new string[1];
            themeData.step_6_video_upload.loading_animation_asset[0] = "https://1000marcas.net/wp-content/uploads/2020/01/Juventus-logo.png";

            themeData.step_8_prize_screen.reward_animation_asset1 = new string[1];
            themeData.step_8_prize_screen.reward_animation_asset1[0] = "https://1000marcas.net/wp-content/uploads/2020/01/Juventus-logo.png";

            themeData.step_8_prize_screen.reward_animation_asset2 = new string[1];
            themeData.step_8_prize_screen.reward_animation_asset2[0] = "https://1000marcas.net/wp-content/uploads/2020/01/Juventus-logo.png";

            themeData.step_8_prize_screen.reward_animation_asset = new string[1];
            themeData.step_8_prize_screen.reward_animation_asset[0] = "https://1000marcas.net/wp-content/uploads/2020/01/Juventus-logo.png";
        }
        DownloadAssets();
    }

    /// <summary>
    /// TextMeshPro component used to display initialization error messages.
    /// </summary>
    public TMP_Text initError;

    /// <summary>
    /// Callback from GetMachineData save in to global variable the machine data, if everything is okay, call GetTheme function.
    /// </summary>
    /// <param name="response"></param>
    private void MachineDataComplete(string response)
    {
        UnityEngine.Debug.Log("--------------------------------\n" + response + "\n----------------------------------");

        // Guardar los datos para uso offline
        if (!GlobalVariables.offline)
        {
            SaveMachineDataForOffline(response);
        }

        GlobalVariables.machineData = JsonUtility.FromJson<MachineData>(response);
        if (GlobalVariables.machineData.event_id == null)
        {
            initError.gameObject.SetActive(true);
            initError.text = "The event_id variable is not assigned, please check the event in the CMS.";

            // Intentar cargar datos offline si hay error
            if (CheckForOfflineData())
            {
                Debug.Log("Intentando cargar datos offline debido a error...");
                GlobalVariables.offline = true;
                LoadOfflineMachineData();
                return;
            }
        }

        themeData = GlobalVariables.machineData.theme;
        URLdirectory.theme_id = themeData._id;

        GetTheme();
    }

    public void SetOfflineMode(bool isOffline)
    {
        GlobalVariables.offline = isOffline;
        PlayerPrefs.SetInt("OfflineMode", isOffline ? 1 : 0);

        if (isOffline)
        {
            Debug.Log("Modo offline activado manualmente");
            if (CheckForOfflineData())
            {
                LoadOfflineMachineData();
            }
        }
        else
        {
            Debug.Log("Modo online activado - recargando datos del servidor");
            GetMachineData();
        }
    }

    public void ForceInit()
    {
        downloadedResources += 500;
        CheckAllResourcesDownloaded();
    }
}



public class AudioResult
{
    public AudioClip clip;
}