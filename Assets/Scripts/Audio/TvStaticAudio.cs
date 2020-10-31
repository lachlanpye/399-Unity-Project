using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Enviroment;

/// <summary>
/// Janine Aunzo
/// Contains the TV static sound effect that plays at night in the living room area in houseInterior
/// scene.
/// Attach this script to the TvStaticAudio object.
/// </summary>

public class TVStaticAudio : MonoBehaviour
{
    #pragma warning disable 0649
    [SerializeField] private AudioClip tvStaticClip;
    #pragma warning restore 0649
    DayOrNightObjects dayOrNightObjects;


    /// <summary>
    /// Janine Aunzo
    /// Initialise variable.
    /// </summary>
    void Awake()
    {
        dayOrNightObjects = GameObject.Find("SceneObjects").GetComponent<DayOrNightObjects>();
    }


    /// <summary>
    /// Janine Aunzo
    /// Plays sound effect only if at night time.
    /// Triggered when player walk over zones that eneter the living room area
    /// at night.
    /// </summary>
    public void PlayStaticTv()
    {
        if (dayOrNightObjects.currentlyDay == false)
        {
            AudioManager.publicInstance.FadeInSFXLoop(tvStaticClip);
        }
    }

    /// <summary>
    /// Janine Aunzo
    /// Stops sound effect when player exits the room.
    /// Triggered when player walks to zones that leave the living room area
    /// at night.
    /// </summary>
    public void StopStaticTv()
    {
        if (dayOrNightObjects.currentlyDay == false)
        {
            AudioManager.publicInstance.FadeOutSFXLoop();
        }
    }
}

