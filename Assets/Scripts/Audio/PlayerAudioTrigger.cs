using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioTrigger : MonoBehaviour
{
    //Trigger functions for player animation events
    private PlayerAudio playerAudio;

    public bool hitEnemy;
    public bool hitBoss;

    void Start()
    {
        playerAudio = GameObject.Find("PlayerAudio").GetComponent<PlayerAudio>();
        hitEnemy = false;
        hitBoss = false;
    }

    public void PlayAttackSound()
    {

        //If hit connects with an enemy
        if (hitEnemy == true || hitBoss == true)
        {
            playerAudio.PlayStab();
        }
        else
        {
            playerAudio.PlayAttack();
        }

    }

    public void PlayBipedalDamageSound()
    {
        playerAudio.PlayBipedalDamage();
    }

    public void PlayBipedalKillSound()
    {
        playerAudio.PlayBipedalKill();
    }

    public void PlayDeathSound()
    {
        playerAudio.PlayDeath();
    }

    public void PlayStaggerSound()
    {
        playerAudio.PlayStagger();
    }
}
