using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFightAudio : MonoBehaviour
{
    [SerializeField] private AudioClip[] swipeAttacks = new AudioClip[3];
    [SerializeField] private AudioClip pentagram;
    [SerializeField] private AudioClip laser;

    public float pitchFloor;
    public float pitchCeil;
    private float pitch;

    private AudioClip swipeAttackClip;

    public void playSwipe()
    {
        swipeAttackClip = swipeAttacks[Random.Range(0, swipeAttacks.Length)];
        pitch = Random.Range(pitchFloor, pitchCeil);
        AudioManager.publicInstance.PlaySFX(swipeAttackClip, pitch);
    }

    public void playPentagram()
    {
        AudioManager.publicInstance.PlaySFX(pentagram);
    }

    public void playLaser()
    {
        AudioManager.publicInstance.PlaySFX(laser);
    }
}
