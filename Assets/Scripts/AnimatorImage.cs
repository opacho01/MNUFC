using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

/// <summary>
/// Handles the animation of images by converting textures into sprites and cycling through frames.
/// </summary>
public class AnimatorImage : MonoBehaviour
{
    /// <summary>
    /// The array of Texture2D images retrieved from the server.
    /// </summary>
    public Texture2D[] objects;

    /// <summary>
    /// The converted sprites used for animation.
    /// </summary>
    public Sprite[] textures;

    /// <summary>
    /// The Image component used to display the animation.
    /// </summary>
    public Image goMaterial;

    /// <summary>
    /// Tracks the current animation frame index.
    /// </summary>
    public int frameCounter = 0;

    /// <summary>
    /// Determines which animation sequence to play.
    /// </summary>
    public int nombre;

    /// <summary>
    /// Provides access to animation resources.
    /// </summary>
    public GetAllResources allResources;

    /// <summary>
    /// Initializes the animation by selecting the appropriate textures based on the assigned name.
    /// </summary>
    public void initAnim()
    {
        switch (nombre)
        {
            case 0:
                this.objects = allResources.loadingImg;
                break;
            case 1:
                this.objects = allResources.teamPuppet3;
                break;

            case 2:
                this.objects = allResources.teamPuppet1;
                break;

            case 3:
                this.objects = allResources.teamPuppet2;
                break;
        }
        
        this.textures = new Sprite[objects.Length];
        goMaterial = GetComponent<Image>();
        for (int i = 0; i < objects.Length; i++)
        {
            this.textures[i] = TextureToSpriteConverter(objects[i]);//Convert Texture2D in to sprites.
        }
    }

    /// <summary>
    /// Starts the puppet animation with a predefined delay.
    /// </summary>
    public void animPuppet()
    {
        Anima(0.05f);
    }

    /// <summary>
    /// Converts a Texture2D into a Sprite.
    /// </summary>
    /// <param name="texture">The Texture2D to convert.</param>
    /// <returns>The generated sprite.</returns>
    Sprite TextureToSpriteConverter(Texture2D texture)
    {
        //Create sprite whit atributes of Texture2D
        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f)
        );

        return sprite;
    }

    /// <summary>
    /// Starts the animation loop with a given delay.
    /// </summary>
    /// <param name="retraso">The delay between frames.</param>
    void Anima(float retraso)
    {
        try
        {
            StartCoroutine("PlayLoop", retraso);
            goMaterial.sprite = textures[frameCounter];
        } catch (Exception e)
        {
            Debug.LogWarning(e);
        }
    }

    /// <summary>
    /// Continuously cycles through the texture frames to create an animation effect.
    /// </summary>
    /// <param name="delay">The delay between each frame change.</param>
    IEnumerator PlayLoop(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            if (textures.Length > 1)
            {
                frameCounter = (++frameCounter) % textures.Length;//cycle the index.
                goMaterial.sprite = textures[frameCounter];//assign the sprites
            }
        }
    }

}