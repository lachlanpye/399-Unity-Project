using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Janine
//Trigger functions for player animation events. It calls methods in Player Audio script
//which contains the player sound files.
//Attach this script to any game objects that use player animations

public class PlayerAudioTrigger : MonoBehaviour
{
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
        //Plays an attack sound depending on whether the player has stabbed an enemy
        if (hitEnemy == true || hitBoss == true)
        {
            playerAudio.PlayStab();
        }
        else
        {
            playerAudio.PlayAttack();
        }
    }

    public void DisableHitBoss()
    {
        //This function is called at the last frame of the player attack animation
        //This is to ensure that the stab audio is not played after getting one successful
        //hit at a boss after it is stunned
        hitBoss = false;
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
