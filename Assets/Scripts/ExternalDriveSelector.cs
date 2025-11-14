using System.IO;
using System.Linq;
using UnityEngine;
using TMPro;

public class ExternalDriveSelector : MonoBehaviour
{
    public TMP_Dropdown dropdown;

    private string defaultPath;
    private string savedDriveKey = "SelectedDrive";

    void Awake()
    {
        defaultPath = Path.Combine(Application.persistentDataPath, "MNUFC");
        CreateDirectoryIfNotExists(defaultPath);

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
            CreateDirectoryIfNotExists(targetPath);
            GlobalVariables.pathHDD = targetPath;
        }

        PlayerPrefs.SetString(savedDriveKey, selected);
        PlayerPrefs.Save();

        Debug.Log("Assigned path: " + GlobalVariables.pathHDD);

        // Reiniciar el logger con la nueva ruta
        ErrorLogger.RestartLogger();
    }

    // Load saved path on startup
    void LoadSavedDrive()
    {
        string saved = PlayerPrefs.GetString(savedDriveKey, "default");

        if (saved != "default" && Directory.Exists(saved))
        {
            string targetPath = Path.Combine(saved, "MNUFC");
            CreateDirectoryIfNotExists(targetPath);
            GlobalVariables.pathHDD = targetPath;
        }
        else
        {
            GlobalVariables.pathHDD = defaultPath;
        }
        Debug.Log("Loaded path: " + GlobalVariables.pathHDD);
    }

    // Ensure the current path is valid before using it
    public static void EnsureValidPath()
    {
        if (string.IsNullOrEmpty(GlobalVariables.pathHDD) || !Directory.Exists(GlobalVariables.pathHDD))
        {
            string fallbackPath = Path.Combine(Application.persistentDataPath, "MNUFC");
            CreateDirectoryIfNotExists(fallbackPath);
            GlobalVariables.pathHDD = fallbackPath;
            Debug.LogWarning("Invalid path. Default path will be used: " + GlobalVariables.pathHDD);
        }
    }

    // Create directory if it does not exist
    private static void CreateDirectoryIfNotExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            Debug.Log("Directory created at: " + path);
        }
    }
}
