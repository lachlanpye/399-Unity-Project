using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Janine Aunzo
/// Contains all player audio clips.
/// Attach this script to the PlayerAudio object.
/// </summary>

public class PlayerAudio : MonoBehaviour
{
    #pragma warning disable 0649
    [SerializeField] private AudioClip playerAttack;
    [SerializeField] private AudioClip[] playerStabs = new AudioClip[3];
    [SerializeField] private AudioClip playerBipedalDamage;
    [SerializeField] private AudioClip playerBipedalKill;
    [SerializeField] private AudioClip playerDeath;
    [SerializeField] private AudioClip playerStagger;

    public float pitchFloor;
    public float pitchCeil;
    private float pitch;

    private AudioClip playerStabClip;
    #pragma warning restore 0649

    /// <summary>
    /// Janine Aunzo
    /// Play playerBipedalDamage sound at random pitch when attacked by a bipedal enemy.
    /// </summary>
    public void PlayBipedalDamage()
    {
        pitch = Random.Range(pitchFloor, pitchCeil);
        AudioManager.publicInstance.PlaySFX(playerBipedalDamage, pitch);
    }

    /// <summary>
    /// Janine Aunzo
    /// Play sound when killed by bipedal enemy.
    /// </summary>
    public void PlayBipedalKill()
    {
        AudioManager.publicInstance.PlaySFX(playerBipedalKill);
    }

    /// <summary>
    /// Janine Aunzo
    /// Play death sound when killed by boss.
    /// </summary>
    public void PlayDeath()
    {
        AudioManager.publicInstance.PlaySFX(playerDeath);
    }

    /// <summary>
    /// Janine Aunzo
    /// Play sound when hit by boss.
    /// </summary>
    public void PlayStagger()
    {
        pitch = Random.Range(pitchFloor, pitchCeil);
        AudioManager.publicInstance.PlaySFX(playerStagger, pitch);
    }

    /// <summary>
    /// Janine Aunzo
    /// Play stab sound when player has successfully hit an enemy.
    /// </summary>
    public void PlayStab()
    {
        playerStabClip = playerStabs[Random.Range(0, playerStabs.Length)];
        pitch = Random.Range(pitchFloor, pitchCeil);
        AudioManager.publicInstance.PlaySFX(playerStabClip, pitch);
    }

    /// <summary>
    /// Janine Aunzo
    /// Play soudn when player hits attack button but isn't hitting anything.
    /// </summary>
    public void PlayAttack()
    {
        pitch = Random.Range(pitchFloor, pitchCeil);
        AudioManager.publicInstance.PlaySFX(playerAttack, pitch);
    }
}
