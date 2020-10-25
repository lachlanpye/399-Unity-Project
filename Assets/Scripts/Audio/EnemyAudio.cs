using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAudio : MonoBehaviour
{
    public AudioSource enemyAudioSource;
    public float pitchFloor;
    public float pitchCeiling;


    [SerializeField] private AudioClip[] sfx = new AudioClip[2];
    [SerializeField] private AudioClip[] dead = new AudioClip[2];
    [SerializeField] private AudioClip[] stun = new AudioClip[2];


    public void playSound()
    {
        enemyAudioSource.volume = AudioManager.publicInstance.GetSFXVolume();
        enemyAudioSource.clip = sfx[Random.Range(0, sfx.Length)];
        enemyAudioSource.pitch = (Random.Range(pitchFloor, pitchCeiling));
        enemyAudioSource.Play();
    }

    public void playDead()
    {
        enemyAudioSource.volume = AudioManager.publicInstance.GetSFXVolume();
        enemyAudioSource.clip = dead[Random.Range(0, dead.Length)];
        enemyAudioSource.pitch = (Random.Range(pitchFloor, pitchCeiling));
        enemyAudioSource.Play();
    }

    public void playStun()
    {
        enemyAudioSource.volume = AudioManager.publicInstance.GetSFXVolume();
        enemyAudioSource.clip = stun[Random.Range(0, stun.Length)];
        enemyAudioSource.pitch = (Random.Range(pitchFloor, pitchCeiling));
        enemyAudioSource.Play();
    }
}
