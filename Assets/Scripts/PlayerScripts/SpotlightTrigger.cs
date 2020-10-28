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
            //float total = 1f;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, col.bounds.center - transform.position, Mathf.Infinity, enemyMask);
            //if (hit.collider != null)
            //{
            //    float maxDist = Vector3.Magnitude(GetComponent<PolygonCollider2D>().bounds.size);
            //    total = 1 - (hit.distance / maxDist);

                //EnemyBehaviour enemyBehaviour = col.gameObject.GetComponent<EnemyBehaviour>();
                //enemyBehaviour.StartInLightCount();
            //}
        }
    }

    //void OnTriggerExit2D(Collider2D col)
    //{
    //    if (col.gameObject.layer == 10)
    //    {
            //EnemyBehaviour enemyBehaviour = col.gameObject.GetComponent<EnemyBehaviour>();
            //enemyBehaviour.UpdateOpacity(0.5f);
            //enemyBehaviour.StopInLightCount();
    //    }
    //}
}