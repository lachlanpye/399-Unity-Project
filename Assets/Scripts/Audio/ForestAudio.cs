using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestAudio : MonoBehaviour
{
    #pragma warning disable 0649
    [SerializeField] private AudioClip townBGM;
    #pragma warning restore 0649


    private void Awake()
    {
        AudioManager.publicInstance.FadeInBGM(townBGM);
    }

}
