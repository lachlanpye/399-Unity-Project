using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private AudioClip playerAttack;
    [SerializeField] private AudioClip[] playerStabs = new AudioClip[3];
    [SerializeField] private AudioClip playerDamage;

    public float pitchFloor;
    public float pitchCeil;
    private float pitch;

    private AudioClip playerStabClip;
#pragma warning restore 0649

    // Update is called once per frame
    public void PlayDamage()
    {
        pitch = Random.Range(pitchFloor, pitchCeil);
        AudioManager.publicInstance.PlaySFX(playerDamage, pitch);
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
