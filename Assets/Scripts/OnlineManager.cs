using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OnlineManager : MonoBehaviour
{
    public Slider[] onlineSlider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (PlayerPrefs.HasKey("Offline"))
        {
            if (PlayerPrefs.GetInt("Offline") == 1)
            {
                GlobalVariables.offline = true;
                for (byte i = 0; i < onlineSlider.Length; i++)
                {
                    onlineSlider[i].value = 0;
                }
            } else
            {
                GlobalVariables.offline = false;
                for (byte i = 0; i < onlineSlider.Length; i++)
                {
                    onlineSlider[i].value = 1;
                }
            }
        } else
        {
            PlayerPrefs.SetInt("Offline", 1);
        }
        //Debug.Log("OnlineManager " + GlobalVariables.offline);
    }

    public void OnlineOfflineSelector()
    {
        GlobalVariables.offline = !GlobalVariables.offline;
        if (GlobalVariables.offline)
        {
            PlayerPrefs.SetInt("Offline", 1);
        } else
        {
            PlayerPrefs.SetInt("Offline", 0);
        }
        SceneManager.LoadScene(0);
    }
}
