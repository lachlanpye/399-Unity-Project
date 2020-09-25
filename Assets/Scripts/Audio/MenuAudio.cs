using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAudio : MonoBehaviour
{
    #pragma warning disable 0649
    [SerializeField] private AudioClip main_bgm;

    private void Awake()
    {
        AudioManager.publicInstance.PlayBGM(main_bgm);
    }


    public void FadeOutTheme()
    {
        AudioManager.publicInstance.FadeOutBGM();
    }

    #pragma warning restore 0649
}
