using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Provides auxiliary button functionalities, such as resetting the scene or quitting the application.
/// </summary>
public class AuxButtons : MonoBehaviour
{
    /// <summary>
    /// Resets the current scene by loading the first scene in the build index.
    /// </summary>
    public void resetScene()
    {
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Exits the application.
    /// </summary>
    public void Quitar()
    {
        Application.Quit();
    }
}
