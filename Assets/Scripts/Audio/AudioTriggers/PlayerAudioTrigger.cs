using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Janine Aunzo
/// Trigger functions for player animation events. It calls methods in Player Audio script
/// which contains the player sound files.
/// Attach this script to all game objects that use player animations.
/// </summary>

public class PlayerAudioTrigger : MonoBehaviour
{
    private PlayerAudio playerAudio;
    public bool hitEnemy;
    public bool hitBoss;

    /// <summary>
    /// Janine Aunzo
    /// Initializing variables.
    /// </summary>
    void Start()
    {
        playerAudio = GameObject.Find("PlayerAudio").GetComponent<PlayerAudio>();
        hitEnemy = false;
        hitBoss = false;
    }

    /// <summary>
    /// Janine Aunzo
    /// Plays certain attack sound depending on whether the player has successfully hit an enemy
    /// or boss.
    /// </summary>
    public void PlayAttackSound()
    {
        if (hitEnemy == true || hitBoss == true)
        {
            playerAudio.PlayStab();
        }
        else
        {
            playerAudio.PlayAttack();
        }
    }

    /// <summary>
    /// Janine Aunzo
    /// This function is called by animation event at the last frame of the player attack 
    /// animation.
    /// This is to ensure that the stab audio is not played after getting one successful
    /// hit at a boss after it is stunned.
    /// </summary>
    public void DisableHitBoss()
    {
        hitBoss = false;
    }

    /// <summary>
    /// Janine Aunzo
    /// This is called by animation event 3 times during the bipedal damage animation.
    /// </summary>
    public void PlayBipedalDamageSound()
    {
        playerAudio.PlayBipedalDamage();
    }

    /// <summary>
    /// Janine Aunzo
    /// This is called by animation event once at end of player bidepal death animation.
    /// </summary>
    public void PlayBipedalKillSound()
    {
        playerAudio.PlayBipedalKill();
    }

    /// <summary>
    /// Janine Aunzo
    /// This is called by animation event during the player death animation.
    /// </summary>
    public void PlayDeathSound()
    {
        playerAudio.PlayDeath();
    }

    /// <summary>
    /// Janine Aunzo
    /// This is called by animation event in player stagger animation.
    /// </summary>
    public void PlayStaggerSound()
    {
        playerAudio.PlayStagger();
    }
}
