using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehaviour : MonoBehaviour
{
    public GameObject player;

    [Space]
    public float moveSpeed;
    [Space]
    public float distanceDownFromBossCenter;
    public float upColliderDistance;
    public float leftColliderDistance;
    public float rightColliderDistance;
    public float downColliderDistance;
    public bool showFeetColliders;

    private RaycastHit2D upCast;
    private RaycastHit2D leftCast;
    private RaycastHit2D rightCast;
    private RaycastHit2D downCast;
    public int objectMask;

    private bool blockUp;
    private bool blockLeft;
    private bool blockRight;
    private bool blockDown;

    void Start()
    {

    }

    void Update()
    {
        transform.Translate((player.transform.position - transform.position).normalized * (moveSpeed / 32));

        upCast = Physics2D.Raycast(transform.position - (Vector3.up * distanceDownFromBossCenter), Vector2.up, upColliderDistance, objectMask);
        leftCast = Physics2D.Raycast(transform.position - (Vector3.up * distanceDownFromBossCenter), Vector2.left, leftColliderDistance, objectMask);
        rightCast = Physics2D.Raycast(transform.position - (Vector3.up * distanceDownFromBossCenter), Vector2.right, rightColliderDistance, objectMask);
        downCast = Physics2D.Raycast(transform.position - (Vector3.up * distanceDownFromBossCenter), Vector2.down, downColliderDistance, objectMask);

        if (showFeetColliders)
        {
            Debug.DrawRay(transform.position - (Vector3.up * distanceDownFromBossCenter), Vector2.up * upColliderDistance, Color.red);
            Debug.DrawRay(transform.position - (Vector3.up * distanceDownFromBossCenter), Vector2.left * leftColliderDistance, Color.red);
            Debug.DrawRay(transform.position - (Vector3.up * distanceDownFromBossCenter), Vector2.right * rightColliderDistance, Color.red);
            Debug.DrawRay(transform.position - (Vector3.up * distanceDownFromBossCenter), Vector2.down * downColliderDistance, Color.red);
        }

        if (upCast.collider != null)
        {
            blockUp = true;
            Debug.Log("blocked");
        }
        else { blockUp = false; }

        if (leftCast.collider != null)
        {
            blockLeft = true;
            Debug.Log("blocked");
        }
        else { blockLeft = false; }

        if (rightCast.collider != null)
        {
            blockRight = true;
            Debug.Log("blocked");
        }
        else { blockRight = false; }

        if (downCast.collider != null)
        {
            blockDown = true;
            Debug.Log("blocked");
        }
        else { blockDown = false; }


    }

    public void BeginFirstPhase()
    {

    }
}
