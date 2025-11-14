using System;
using System.IO;
using UnityEngine;

public class ErrorLogger : MonoBehaviour
{
    private string logFilePath;
    private bool initialized = false;

    private static ErrorLogger instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeLogger();
    }

    private void InitializeLogger()
    {
        ExternalDriveSelector.EnsureValidPath();

        string folderPath = GlobalVariables.pathHDD;

        if (string.IsNullOrEmpty(folderPath))
        {
            folderPath = Path.Combine(Application.persistentDataPath, "MNUFC");
        }

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        logFilePath = Path.Combine(folderPath, "log_" + timestamp + ".txt");

        Application.logMessageReceived += HandleLog;
        AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;

        initialized = true;
        Debug.Log("ErrorLogger initialized. Log path: " + logFilePath);
    }

    public static void RestartLogger()
    {
        if (instance != null)
        {
            instance.ShutdownLogger();
            instance.InitializeLogger();
        }
    }

    private void ShutdownLogger()
    {
        if (initialized)
        {
            Application.logMessageReceived -= HandleLog;
            AppDomain.CurrentDomain.UnhandledException -= HandleUnhandledException;
            initialized = false;
        }
    }

    void OnDestroy()
    {
        ShutdownLogger();
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (!initialized || string.IsNullOrEmpty(logFilePath)) return;

        if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
        {
            string entry = "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + type + ": " + logString + "\n" + stackTrace + "\n";
            File.AppendAllText(logFilePath, entry);
        }
    }

    private void HandleUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (!initialized || string.IsNullOrEmpty(logFilePath)) return;

        string entry = "[" + DateTime.Now.ToString("HH:mm:ss") + "] Unhandled Exception: " + e.ExceptionObject.ToString() + "\n";
        File.AppendAllText(logFilePath, entry);
    }
}
