using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Janine Aunzo
/// Contains audio for Town scene.
/// Attach this script to the TownAudio object.
/// </summary>

public class TownAudio : MonoBehaviour
{
    #pragma warning disable 0649
    [SerializeField] private AudioClip townBGM;
    #pragma warning restore 0649


    /// <summary>
    /// Janine Aunzo
    /// Play music on scene load.
    /// </summary>
    void Awake()
    {
        AudioManager.publicInstance.PlayBGM(townBGM);
    }

    /// <summary>
    /// Janine Aunzo
    /// Fades out town music when entering forest.
    /// </summary>
    public void FadeOutBGM()
    {
        AudioManager.publicInstance.FadeOutBGM(0.5f);
    }
}
