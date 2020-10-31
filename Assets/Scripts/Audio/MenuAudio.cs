using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Janine Aunzo
/// Contains main menu audio
/// Attach this script to the MainMenuAudio object
/// </summary>

public class MenuAudio : MonoBehaviour
{
    #pragma warning disable 0649
    [SerializeField] private AudioClip mainBGM;
    [SerializeField] private AudioClip newGameChord;
    [SerializeField] private AudioClip introBGM1;
    #pragma warning restore 0649

    /// <summary>
    /// Janine Aunzo 
    /// Play main menu music at scene load
    /// </summary>
    private void Awake()
    {
        AudioManager.publicInstance.PlayBGM(mainBGM);
    }

    /// <summary>
    /// Janine Aunzo
    /// Fade out main menu music
    /// </summary>
    public void FadeOutTheme()
    {
        AudioManager.publicInstance.FadeOutBGM();
    }


    public void NewGameTransition()
    {
        StartCoroutine(Transition());
    }

    /// <summary>
    /// Janine Aunzo
    /// Stitches two audio clips seamlessly when starting the new game cutscene
    /// </summary>
    /// <returns></returns>
    IEnumerator Transition()
    {
        yield return new WaitForSeconds(1f);

        double startTime = AudioSettings.dspTime + 0.2;
        AudioManager.publicInstance.PlayBGMTransition(newGameChord, startTime);

        double duration = newGameChord.length;
        AudioManager.publicInstance.PlayBGM(introBGM1, startTime + duration);
    }
}
