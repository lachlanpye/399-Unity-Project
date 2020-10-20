using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFightAudio : MonoBehaviour
{
    [SerializeField] private AudioClip[] swipeAttacks = new AudioClip[3];
    [SerializeField] private AudioClip stunned;
    [SerializeField] private AudioClip damage;
    [SerializeField] private AudioClip death;
    [SerializeField] private AudioClip pentagram;
    [SerializeField] private AudioClip laser;

    public float pitchFloor;
    public float pitchCeil;
    private float pitch;

    private AudioClip swipeAttackClip;

    public void PlaySwipe()
    {
        swipeAttackClip = swipeAttacks[Random.Range(0, swipeAttacks.Length)];
        pitch = Random.Range(pitchFloor, pitchCeil);
        AudioManager.publicInstance.PlaySFX(swipeAttackClip, pitch);
    }

    public void PlayPentagram()
    {
        AudioManager.publicInstance.PlaySFX(pentagram);
    }

    public void PlayLaser()
    {
        AudioManager.publicInstance.PlaySFX(laser);
    }

    public void PlayStun()
    {
        pitch = Random.Range(pitchFloor, pitchCeil);
        AudioManager.publicInstance.PlaySFX(stunned, pitch);
    }

    public void PlayDamaged()
    {
        pitch = Random.Range(pitchFloor, pitchCeil);
        AudioManager.publicInstance.PlaySFX(damage, pitch);
    }

    public void PlayDeath()
    {
        AudioManager.publicInstance.PlaySFX(death);
    }
}
