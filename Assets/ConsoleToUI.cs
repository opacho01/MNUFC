using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Captures Unity console logs and displays them in a TextMeshPro UI element.
/// Allows filtering, scrolling, and color-coded log messages.
/// </summary>
public class ConsoleToTMP : MonoBehaviour
{
    /// <summary>
    /// TextMeshPro component for displaying console messages.
    /// </summary>
    public TMP_Text consoleText;

    /// <summary>
    /// ScrollRect component for navigating through console logs.
    /// </summary>
    public ScrollRect scrollRect;

    /// <summary>
    /// Input field for searching within the console logs.
    /// </summary>
    public TMP_InputField searchInputField;

    /// <summary>
    /// List that stores console log messages.
    /// </summary>
    private List<string> logMessages = new List<string>();

    /// <summary>
    /// Maximum number of messages displayed in the console.
    /// </summary>
    public int maxMessages = 50;

    /// <summary>
    /// Color for standard log messages.
    /// </summary>
    public Color logColor = Color.white;

    /// <summary>
    /// Color for warning messages.
    /// </summary>
    public Color warningColor = Color.yellow;

    /// <summary>
    /// Color for error messages.
    /// </summary>
    public Color errorColor = Color.red;

    /// <summary>
    /// Color for exception messages.
    /// </summary>
    public Color exceptionColor = Color.magenta;

    /// <summary>
    /// Subscribes to the Unity console log event on enable.
    /// </summary>
    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    /// <summary>
    /// Unsubscribes from the Unity console log event on disable.
    /// </summary>
    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    /// <summary>
    /// Handles incoming console logs, formats them, and updates the UI.
    /// </summary>
    /// <param name="logString">The log message.</param>
    /// <param name="stackTrace">The stack trace associated with the log.</param>
    /// <param name="type">The type of log (Log, Warning, Error, Exception).</param>
    void HandleLog(string logString, string stackTrace, LogType type)
    {
        string formattedMessage = FormatLogMessage(logString, type);
        logMessages.Add(formattedMessage);

        if (logMessages.Count > maxMessages)
        {
            logMessages.RemoveAt(0);
        }

        UpdateConsoleText();
    }

    /// <summary>
    /// Formats a log message with color coding based on type.
    /// </summary>
    /// <param name="message">The message to format.</param>
    /// <param name="type">The type of log.</param>
    /// <returns>Formatted HTML color-coded message.</returns>
    string FormatLogMessage(string message, LogType type)
    {
        string colorHex;
        switch (type)
        {
            case LogType.Warning:
                colorHex = ColorUtility.ToHtmlStringRGBA(warningColor);
                break;
            case LogType.Error:
                colorHex = ColorUtility.ToHtmlStringRGBA(errorColor);
                break;
            case LogType.Exception:
                colorHex = ColorUtility.ToHtmlStringRGBA(exceptionColor);
                break;
            default:
                colorHex = ColorUtility.ToHtmlStringRGBA(logColor);
                break;
        }

        return $"<color=#{colorHex}>{message}</color>";
    }

    /// <summary>
    /// Updates the UI with the latest console logs.
    /// Filters logs based on search input and scrolls to the latest message.
    /// </summary>
    void UpdateConsoleText()
    {
        string fullText = string.Join("\n", logMessages);

        if (!string.IsNullOrEmpty(searchInputField.text))
        {
            fullText = FilterMessages(fullText, searchInputField.text);
        }

        consoleText.text = fullText;
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0;
    }

    /// <summary>
    /// Filters log messages based on a search term.
    /// </summary>
    /// <param name="fullText">The complete log text.</param>
    /// <param name="searchTerm">The term used to filter messages.</param>
    /// <returns>Filtered log messages.</returns>
    string FilterMessages(string fullText, string searchTerm)
    {
        StringBuilder filteredText = new StringBuilder();
        string[] lines = fullText.Split('\n');

        foreach (string line in lines)
        {
            if (line.Contains(searchTerm))
            {
                filteredText.AppendLine(line);
            }
        }

        return filteredText.ToString();
    }

    /// <summary>
    /// Refreshes the console display when the search input changes.
    /// </summary>
    public void OnSearchInputChanged()
    {
        UpdateConsoleText();
    }

    /// <summary>
    /// Closes the application.
    /// </summary>
    public void Quitar()
    {
        Debug.Log("Closing App");
        Application.Quit();
    }

    /// <summary>
    /// Clears the console logs and UI text.
    /// </summary>
    public void LimpiarConsola()
    {
        consoleText.text = "";
        logMessages.Clear();
        consoleText.text = "";
    }
}