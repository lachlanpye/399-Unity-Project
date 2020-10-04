using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRadius : MonoBehaviour
{
    private PlayerBehaviour playerBehaviour;

    void Start()
    {
        playerBehaviour = GetComponentInParent<PlayerBehaviour>();
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            EnemyBehaviour enemyBehaviour = col.gameObject.GetComponent<EnemyBehaviour>();
            if (enemyBehaviour.stunned == true)
            {
                playerBehaviour.AbleToAttack(col.gameObject);
            }
            else
            {
                playerBehaviour.NotAbleToAttack(col.gameObject);
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            playerBehaviour.NotAbleToAttack(col.gameObject);
        }
    }
}
