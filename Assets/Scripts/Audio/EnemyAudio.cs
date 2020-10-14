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

    void Awake()
    {
        //enemyAudioSource.pitch = (Random.Range(pitchFloor, pitchCeiling));
    }

    public void playSound()
    {
        enemyAudioSource.clip = sfx[Random.Range(0, sfx.Length)];
        enemyAudioSource.Play();
    }

    public void playDead()
    {
        enemyAudioSource.clip = dead[Random.Range(0, dead.Length)]; ;
        enemyAudioSource.Play();
    }

    public void playStun()
    {
        enemyAudioSource.clip = stun[Random.Range(0, stun.Length)];
        enemyAudioSource.Play();
    }
}
