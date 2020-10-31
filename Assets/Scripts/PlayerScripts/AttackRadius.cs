using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Written by Lachlan
//Edited by Tyler
public class AttackRadius : MonoBehaviour
{
    private PlayerBehaviour playerBehaviour;
    private PlayerAudioTrigger audioTrigger;

    void Start()
    {
        playerBehaviour = GetComponentInParent<PlayerBehaviour>();
        audioTrigger = GetComponentInParent<PlayerAudioTrigger>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            EnemyBehaviour enemyBehaviour = collision.gameObject.GetComponent<EnemyBehaviour>();
            if (enemyBehaviour.currentState == EnemyBehaviour.State.Stunned)
            {
                playerBehaviour.AbleToAttack(collision.gameObject);
                audioTrigger.hitEnemy = true;
            }
        }

        if (collision.gameObject.tag == "Boss")
        {
            BossBehaviour bossBehaviour = collision.gameObject.GetComponent<BossBehaviour>();
            if (bossBehaviour.BossIsStunned() == true)
            {
                audioTrigger.hitBoss = true;
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            playerBehaviour.NotAbleToAttack(col.gameObject);
            audioTrigger.hitEnemy = false;
        }

        if (col.gameObject.tag == "Boss")
        {
            audioTrigger.hitBoss = false;
        }
    }
}
