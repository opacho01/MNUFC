using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Handles fading out of a SpriteRenderer over time, with an optional restart feature.
/// </summary>
public class FadeOutByTime : MonoBehaviour
{
    /// <summary>
    /// The SpriteRenderer component that will fade out.
    /// </summary>
    public SpriteRenderer spriteRenderer;

    /// <summary>
    /// The ParticleSystem associated with the effect (optional).
    /// </summary>
    public ParticleSystem particleSystem;

    /// <summary>
    /// Duration of the fade effect in seconds.
    /// </summary>
    public float fadeDuration = 7f;

    /// <summary>
    /// Determines whether the fade effect should restart after completion.
    /// </summary>
    public bool restart = false;

    /// <summary>
    /// Initializes the fade effect when the object is enabled.
    /// </summary>
    private void OnEnable()
    {
        init();
    }

    /// <summary>
    /// Prepares the component for fading, ensuring the SpriteRenderer is set and starting the fade coroutine.
    /// </summary>
    private void init()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.color = Color.white;

        // Uncomment if particle effects should play when fading begins.
        /*if (particleSystem != null)
        {
            particleSystem.Play();
        }*/

        StartCoroutine(FadeOutCoroutine());
    }

    /// <summary>
    /// Coroutine that gradually decreases the sprite's alpha value until fully transparent.
    /// If the restart flag is enabled, the fade effect will restart after a delay.
    /// </summary>
    /// <returns>An IEnumerator for coroutine execution.</returns>
    private IEnumerator FadeOutCoroutine()
    {
        float elapsedTime = 0f;
        Color originalColor = spriteRenderer.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null; // Waits for a frame before continuing.
        }

        // Ensures the sprite is fully transparent at the end of the fade.
        spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        // Optionally stops particle effects after the fade is complete.
        /*if (particleSystem != null)
        {
            particleSystem.Stop();
        }*/

        // Restarts the fade effect after 8 seconds if restart is enabled.
        if (restart)
        {
            yield return new WaitForSeconds(8);
            init();
        }
    }
}
