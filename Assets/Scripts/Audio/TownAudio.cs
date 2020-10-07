using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownAudio : MonoBehaviour
{
    #pragma warning disable 0649
    [SerializeField] private AudioClip townBGM;
    #pragma warning restore 0649

    void Awake()
    {
        AudioManager.publicInstance.PlayBGM(townBGM);
    }
}
