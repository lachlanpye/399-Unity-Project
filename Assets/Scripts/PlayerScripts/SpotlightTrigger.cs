using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotlightTrigger : MonoBehaviour
{
    private int enemyMask;

    void Start()
    {
        enemyMask = LayerMask.GetMask("Enemy");
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.layer == 10)
        {
            float total = 1f;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, col.bounds.center - transform.position, Mathf.Infinity, enemyMask);
            if (hit.collider != null)
            {
                Debug.DrawRay(transform.position, (new Vector3(hit.point.x, hit.point.y, 0)) - transform.position, Color.yellow);

                float maxDist = Vector3.Magnitude(GetComponent<PolygonCollider2D>().bounds.size);
                total = 1 - (hit.distance / maxDist);

                EnemyBehaviour enemyBehaviour = col.gameObject.GetComponent<EnemyBehaviour>();
                enemyBehaviour.UpdateOpacity(total);
                enemyBehaviour.StartInLightCount();
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.layer == 10)
        {
            EnemyBehaviour enemyBehaviour = col.gameObject.GetComponent<EnemyBehaviour>();
            enemyBehaviour.UpdateOpacity(0.1f);
            enemyBehaviour.StopInLightCount();
        }
    }
}