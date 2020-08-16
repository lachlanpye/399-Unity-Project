using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotlightTrigger : MonoBehaviour
{
    public LayerMask mask;

    private float timer;

    void Start()
    {
        timer = 0;
    }

    void OnTriggerStay2D(Collider2D col)
    {
        float total = 1f;
        timer += Time.deltaTime;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, col.bounds.center - transform.position, Mathf.Infinity, LayerMask.GetMask("Enemy"));
        if (hit.collider != null)
        {
            Debug.DrawRay(transform.position, (new Vector3(hit.point.x, hit.point.y, 0)) - transform.position, Color.yellow);

            float maxDist = Vector3.Magnitude(GetComponent<PolygonCollider2D>().bounds.size);
            total = 1 - (((hit.distance / maxDist) / 2) + 0.1f);

            col.gameObject.GetComponent<EnemyBehaviour>().UpdateOpacity(total);
        }

        if (timer >= 0.5)
        {
            Debug.Log(total);
            timer = 0;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        col.gameObject.GetComponent<EnemyBehaviour>().UpdateOpacity(0.1f);
    }
}