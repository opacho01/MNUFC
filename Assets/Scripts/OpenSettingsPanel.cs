using UnityEngine;

/// <summary>
/// Manages the activation of the settings panel based on a predefined input code sequence.
/// </summary>
public class OpenSettingsPanel : MonoBehaviour
{
    /// <summary>
    /// The settings panel GameObject to be activated when the correct code is entered.
    /// </summary>
    public GameObject panelSettings;

    /// <summary>
    /// Initiates the code checking process to determine whether the settings panel should be opened.
    /// </summary>
    /// <param name="pos">The input position (digit) for the code sequence.</param>
    public void OpenSettingsCode(string pos)
    {
        CheckCode(pos);
    }

    /// <summary>
    /// Stores the currently entered code sequence.
    /// </summary>
    private string ClickedCode = "";

    /// <summary>
    /// Tracks the current digit position in the sequence.
    /// </summary>
    private byte actualDigitCode = 0;

    /// <summary>
    /// Validates and builds the input sequence, activating the settings panel if the correct code is entered.
    /// </summary>
    /// <param name="pos">The input digit for the sequence.</param>
    private void CheckCode(string pos)
    {
        switch (actualDigitCode)
        {
            case 0:
                if (pos == "3")
                {
                    ClickedCode = pos;
                    actualDigitCode++;
                }
                else
                {
                    ClickedCode = "";
                    actualDigitCode = 0;
                }
                break;
            case 1:
                if (pos == "2")
                {
                    ClickedCode += pos;
                    actualDigitCode++;
                }
                else
                {
                    ClickedCode = "";
                    actualDigitCode = 0;
                }
                break;
            case 2:
                if (pos == "1")
                {
                    ClickedCode += pos;
                    actualDigitCode++;
                }
                else
                {
                    ClickedCode = "";
                    actualDigitCode = 0;
                }
                break;
            case 3:
                if (pos == "0")
                {
                    ClickedCode += pos;
                    actualDigitCode++;
                }
                else
                {
                    ClickedCode = "";
                    actualDigitCode = 0;
                }
                break;
        }

        // If the correct sequence "3210" is entered, activate the settings panel
        if (ClickedCode == "3210")
        {
            panelSettings.gameObject.SetActive(true);
            ClickedCode = "";
            actualDigitCode = 0;
        }
    }
}
