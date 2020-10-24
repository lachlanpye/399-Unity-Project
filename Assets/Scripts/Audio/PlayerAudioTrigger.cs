using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioTrigger : MonoBehaviour
{
    //Trigger functions for player animation events
    private PlayerAudio playerAudio;
    private PlayerBehaviour player;

    void Start()
    {
        playerAudio = GameObject.Find("PlayerAudio").GetComponent<PlayerAudio>();
        player = GameObject.Find("Player").GetComponent<PlayerBehaviour>();
    }

    public void playAttackSound()
    {
        //If hit connects with an enemy
        if (player.hitSuccess == true)
        {
            playerAudio.PlayStab();
        } else
        {
            playerAudio.PlayAttack();
        }
    }
}
