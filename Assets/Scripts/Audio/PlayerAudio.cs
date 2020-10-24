using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void PlayBipedalDamage()
    {
        pitch = Random.Range(pitchFloor, pitchCeil);
        AudioManager.publicInstance.PlaySFX(playerBipedalDamage, pitch);
    }

    public void PlayBipedalKill()
    {
        AudioManager.publicInstance.PlaySFX(playerBipedalKill);
    }

    public void PlayDeath()
    {
        AudioManager.publicInstance.PlaySFX(playerDeath);
    }

    public void PlayStagger()
    {
        pitch = Random.Range(pitchFloor, pitchCeil);
        AudioManager.publicInstance.PlaySFX(playerStagger, pitch);
    }

    public void PlayAttack()
    {
        playerStabClip = playerStabs[Random.Range(0, playerStabs.Length)];
        pitch = Random.Range(pitchFloor, pitchCeil);
        AudioManager.publicInstance.PlaySFX(playerStabClip, pitch);
    }

    public void PlayStab()
    {
        pitch = Random.Range(pitchFloor, pitchCeil);
        AudioManager.publicInstance.PlaySFX(playerAttack, pitch);
    }
}
