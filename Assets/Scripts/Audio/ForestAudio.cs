using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Janine Aunzo
/// Contains all the audio clips for the Forest scene.
/// Attach this script to the ForestAduio object.
/// </summary>

public class ForestAudio : MonoBehaviour
{
    #pragma warning disable 0649
    [SerializeField] private AudioClip forestBGM;
    #pragma warning restore 0649

    /// <summary>
    /// Janine Aunzo
    /// Play music at start of scene load.
    /// </summary>
    private void Start()
    {
        StartCoroutine(Transition()); 
    }

    /// <summary>
    /// Janine Aunzo
    /// Wait for the music in previous scene to fully fade out then
    /// fade in the forest ambience.
    /// </summary>
    IEnumerator Transition()
    {
        yield return new WaitForSeconds(0.6f);
        AudioManager.publicInstance.FadeInBGM(forestBGM);
    }

    /// <summary>
    /// Janine Aunzo
    /// Fade out forest ambience when entering church.
    /// This method is triggered by interact event on the ChapelInteract object.
    /// </summary>
    public void FadeOutBGM()
    {
        AudioManager.publicInstance.FadeOutBGM(3f);
    }
}
