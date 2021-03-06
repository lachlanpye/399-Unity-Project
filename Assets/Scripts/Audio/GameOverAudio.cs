﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Janine Aunzo
/// Contains audio for game over sequence.
/// Attach this script to the GameOverAudio object.
/// </summary>

public class GameOverAudio : MonoBehaviour
{
    #pragma warning disable 0649
    [SerializeField] private AudioClip gameOverBGM;
    #pragma warning restore 0649

    /// <summary>
    /// Janine Aunzo
    /// Plays game over audio sequence.
    /// Called in WorldControl script.
    /// </summary>
    public void PlayGameOver()
    {
        StartCoroutine(GameOverSequence());
    }

    /// <summary>
    /// Janine Aunzo
    /// Starts game over audio sequence.
    /// </summary>
    private IEnumerator GameOverSequence()
    {
        AudioManager.publicInstance.FadeOutBGM();
        AudioManager.publicInstance.MuteSFXAnim();
        yield return new WaitForSeconds(1.5f);
        AudioManager.publicInstance.FadeInBGM(gameOverBGM);

        yield return null;
    }
}
