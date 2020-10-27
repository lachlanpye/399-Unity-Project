using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestAudio : MonoBehaviour
{
    #pragma warning disable 0649
    [SerializeField] private AudioClip forestBGM;
    #pragma warning restore 0649


    private void Awake()
    {
        AudioManager.publicInstance.FadeInBGM(forestBGM);
    }

    public void FadeOutBGM()
    {
        AudioManager.publicInstance.FadeOutBGM(3f);
    }
}
