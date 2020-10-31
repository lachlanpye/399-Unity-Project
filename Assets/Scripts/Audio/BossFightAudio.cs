using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Janine Aunzo
/// Contains all audio clips for boss fight in the Church scene
/// Attach this script to the BossFightAudio object
/// </summary>
public class BossFightAudio : MonoBehaviour
{
    #pragma warning disable 0649
    [SerializeField] private AudioClip[] swipeAttacks = new AudioClip[3];
    [SerializeField] private AudioClip stunned;
    [SerializeField] private AudioClip damage;
    [SerializeField] private AudioClip pentagram;
    [SerializeField] private AudioClip laser;

    public float pitchFloor;
    public float pitchCeil;
    private float pitch;

    private AudioClip swipeAttackClip;
    #pragma warning restore 0649

    /// <summary>
    /// Janine Aunzo
    /// Plays boss attack swipe at random pitch
    /// </summary>
    public void PlaySwipe()
    {
        swipeAttackClip = swipeAttacks[Random.Range(0, swipeAttacks.Length)];
        pitch = Random.Range(pitchFloor, pitchCeil);
        AudioManager.publicInstance.PlaySFX(swipeAttackClip, pitch);
    }

    /// <summary>
    /// Janine Aunzo
    /// Plays pentagram sound
    /// </summary>
    public void PlayPentagram()
    {
        AudioManager.publicInstance.PlaySFX(pentagram);
    }

    /// <summary>
    /// Janine Aunzo
    /// Plays laser sound
    /// </summary>
    public void PlayLaser()
    {
        AudioManager.publicInstance.PlaySFX(laser);
    }

    /// <summary>
    /// Janine Aunzo
    /// Plays at random pitch when boss is stunned
    /// </summary>
    public void PlayStun()
    {
        pitch = Random.Range(pitchFloor, pitchCeil);
        AudioManager.publicInstance.PlaySFX(stunned, pitch);
    }

    /// <summary>
    /// Janine Aunzo
    /// Plays sound at random pitch when boss is damaged.
    /// </summary>
    public void PlayDamaged()
    {
        pitch = Random.Range(pitchFloor, pitchCeil);
        AudioManager.publicInstance.PlaySFX(damage, pitch);
    }
}
