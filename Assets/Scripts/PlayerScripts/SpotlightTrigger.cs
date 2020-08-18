using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotlightTrigger : MonoBehaviour
{
    void OnTriggerStay2D(Collider2D col)
    {
        float total = 1f;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, col.bounds.center - transform.position, Mathf.Infinity, LayerMask.GetMask("Enemy"));
        if (hit.collider != null)
        {
            Debug.DrawRay(transform.position, (new Vector3(hit.point.x, hit.point.y, 0)) - transform.position, Color.yellow);

            float maxDist = Vector3.Magnitude(GetComponent<PolygonCollider2D>().bounds.size);
            total = 1 - (hit.distance / maxDist);

            col.gameObject.GetComponent<EnemyBehaviour>().UpdateOpacity(total);
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        col.gameObject.GetComponent<EnemyBehaviour>().UpdateOpacity(0.1f);
    }
}