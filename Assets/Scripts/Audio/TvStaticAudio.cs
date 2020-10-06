using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Enviroment;

public class TvStaticAudio : MonoBehaviour
{
    #pragma warning disable 0649
    [SerializeField] private AudioClip tvStaticClip;
    #pragma warning restore 0649
    DayOrNightObjects dayOrNightObjects;

    void Awake()
    {
        dayOrNightObjects = GameObject.Find("SceneObjects").GetComponent<DayOrNightObjects>();
    }

    public void PlayStaticTv()
    {
        if (dayOrNightObjects.currentlyDay == false)
        {
            AudioManager.publicInstance.FadeInSFXLoop(tvStaticClip);
        }
    }

    public void StopStaticTv()
    {
        if (dayOrNightObjects.currentlyDay == false)
        {
            AudioManager.publicInstance.FadeOutSFXLoop();
        }
    }
}

