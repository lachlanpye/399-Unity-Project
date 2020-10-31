using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Janine Aunzo
/// Contains audio clips for enemies.
/// Attach this script to the Enemy object which will also have an Audio Source component.
/// </summary>

public class EnemyAudio : MonoBehaviour
{
    public AudioSource enemyAudioSource;
    public float pitchFloor;
    public float pitchCeiling;

    [SerializeField] private AudioClip[] sfx = new AudioClip[2];
    [SerializeField] private AudioClip[] dead = new AudioClip[2];
    [SerializeField] private AudioClip[] stun = new AudioClip[2];

    /// <summary>
    /// Janine Aunzo
    /// Plays enemy sound effect. This is called when an enemy collides with EnemySoundRadius object.
    /// within the Player object.
    /// </summary>
    public void PlaySound()
    {
        enemyAudioSource.volume = AudioManager.publicInstance.GetSFXVolume();
        enemyAudioSource.clip = sfx[Random.Range(0, sfx.Length)];
        enemyAudioSource.pitch = (Random.Range(pitchFloor, pitchCeiling));
        enemyAudioSource.Play();
    }

    /// <summary>
    /// Janine Aunzo
    /// Plays enemy death when an enemy's state is dead.
    /// </summary>
    public void PlayDead()
    {
        enemyAudioSource.volume = AudioManager.publicInstance.GetSFXVolume();
        enemyAudioSource.clip = dead[Random.Range(0, dead.Length)];
        enemyAudioSource.pitch = (Random.Range(pitchFloor, pitchCeiling));
        enemyAudioSource.Play();
    }

    /// <summary>
    /// Janine Aunzo
    /// Plays stun sound when enemy's state is stunned.
    /// </summary>
    public void PlayStun()
    {
        enemyAudioSource.volume = AudioManager.publicInstance.GetSFXVolume();
        enemyAudioSource.clip = stun[Random.Range(0, stun.Length)];
        enemyAudioSource.pitch = (Random.Range(pitchFloor, pitchCeiling));
        enemyAudioSource.Play();
    }
}
