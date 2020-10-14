using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFightAudio : MonoBehaviour
{
    [SerializeField] private AudioClip[] swipeAttacks = new AudioClip[3];
    [SerializeField] private AudioClip pentagram;
    [SerializeField] private AudioClip laser;

    private AudioClip swipeAttackClip;

    public void playSwipe()
    {
        swipeAttackClip = swipeAttacks[Random.Range(0, swipeAttacks.Length)];
        AudioManager.publicInstance.PlaySFX(swipeAttackClip);
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
