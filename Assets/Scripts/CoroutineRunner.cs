using System.Collections;
using UnityEngine;

/// <summary>
/// Provides a singleton-based coroutine runner that persists across scenes.
/// This ensures that coroutines can be started without requiring a specific MonoBehaviour instance.
/// </summary>
public class CoroutineRunner : MonoBehaviour
{
    /// <summary>
    /// The singleton instance of the CoroutineRunner.
    /// </summary>
    private static CoroutineRunner _instance;

    /// <summary>
    /// Initializes the singleton instance and ensures it persists across scenes.
    /// </summary>
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    /// <summary>
    /// Starts a coroutine using the singleton instance.
    /// </summary>
    /// <param name="coroutine">The IEnumerator coroutine to execute.</param>
    /// <returns>The Coroutine instance for tracking execution.</returns>
    public static Coroutine Start(IEnumerator coroutine)
    {
        EnsureInstance();
        return _instance.StartCoroutine(coroutine);
    }

    /// <summary>
    /// Ensures the singleton instance exists, creating it if necessary.
    /// </summary>
    private static void EnsureInstance()
    {
        if (_instance == null)
        {
            GameObject obj = new GameObject("CoroutineRunner");
            _instance = obj.AddComponent<CoroutineRunner>();
        }
    }
}