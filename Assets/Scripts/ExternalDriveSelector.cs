using System.IO;
using System.Linq;
using UnityEngine;
using TMPro;

public class ExternalDriveSelector : MonoBehaviour
{
    public TMP_Dropdown dropdown;

    private string defaultPath;
    private string savedDriveKey = "SelectedDrive";

    void Start()
    {
        defaultPath = Application.persistentDataPath;
        dropdown.onValueChanged.AddListener(OnDriveSelected);
        PopulateDrives();
        LoadSavedDrive();
        SetDropdownToSavedOption();
    }

    // Populate the dropdown with available drives
    public void PopulateDrives()
    {
        dropdown.ClearOptions();

        var options = new System.Collections.Generic.List<string>();
        options.Add("default");

        var drives = DriveInfo.GetDrives()
            .Where(d => d.IsReady && d.RootDirectory.FullName != "C:\\")
            .ToList();

        foreach (var drive in drives)
        {
            options.Add(drive.RootDirectory.FullName);
        }

        dropdown.AddOptions(options);
    }

    // Set dropdown to the saved option
    void SetDropdownToSavedOption()
    {
        string saved = PlayerPrefs.GetString(savedDriveKey, "default");

        for (int i = 0; i < dropdown.options.Count; i++)
        {
            if (dropdown.options[i].text == saved)
            {
                dropdown.value = i;
                dropdown.RefreshShownValue();
                break;
            }
        }
    }

    // Handle user selection from dropdown
    void OnDriveSelected(int index)
    {
        string selected = dropdown.options[index].text;

        if (selected == "default")
        {
            GlobalVariables.pathHDD = defaultPath;
        }
        else
        {
            string targetPath = Path.Combine(selected, "MNUFC");

            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
                Debug.Log("MNUFC folder created at: " + targetPath);
            }

            GlobalVariables.pathHDD = targetPath;
        }

        PlayerPrefs.SetString(savedDriveKey, selected);
        PlayerPrefs.Save();

        //Debug.Log("Assigned path: " + GlobalVariables.pathHDD);
    }

    // Load saved path on startup
    void LoadSavedDrive()
    {
        string saved = PlayerPrefs.GetString(savedDriveKey, "default");

        if (saved != "default" && Directory.Exists(saved))
        {
            string targetPath = Path.Combine(saved, "MNUFC");

            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
                Debug.Log("MNUFC folder created at: " + targetPath);
            }

            GlobalVariables.pathHDD = targetPath;
        }
        else
        {
            GlobalVariables.pathHDD = defaultPath;
        }

        //Debug.Log("Loaded path on startup: " + GlobalVariables.pathHDD);
    }

    // Ensure the current path is valid before using it
    public static void EnsureValidPath()
    {
        if (string.IsNullOrEmpty(GlobalVariables.pathHDD) || !Directory.Exists(GlobalVariables.pathHDD))
        {
            GlobalVariables.pathHDD = Application.persistentDataPath;
            Debug.LogWarning("Invalid path. Default path will be used: " + GlobalVariables.pathHDD);
        }
    }
}
