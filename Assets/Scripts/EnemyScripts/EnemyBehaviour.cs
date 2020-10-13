using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyBehaviour : MonoBehaviour
{
    public Transform target;
    public Transform enemySprite;
    SpriteRenderer spriteRenderer;

    public float speed = 5f;
    public float nextWaypointDistance = 2f;

    Path path;
    int currentWaypoint = 0;

    Seeker seeker;
    public State currentState;

    bool isRepeating = false;
    bool isAttacking = false;

    public enum State
    {
        MoveIn,
        Attack,
        Stunned,
        Flee,
        Dead
    }

    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        currentState = State.MoveIn;

        spriteRenderer = enemySprite.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //pause everything if world is paused, in dialogue, or the player is currently being attacked
        switch (currentState)
        {
            case State.MoveIn:
                {
                    MoveIntoPlayer();
                    break;
                }
            case State.Attack:
                {
                    Attack();
                    break;
                }
            case State.Stunned:
                {
                    Stunned();
                    break;
                }
            case State.Flee:
                {
                    Flee();
                    break;
                }
        }
    }
    void Flee()
    {
        Debug.Log("Flee");
        //turn off alpha so that enemy fades away over a period of time if currently visible or stays invisible if not currently visible
        //teleport to start position?
    }

    void Stunned()
    {
        Debug.Log("Stunned");
        //play stun animation
        //wait period of time
        //if still in stun state
        //then current state = State.Flee
    }
    public void FlashStun()
    {
        //inLightTime = timeBeforeStun;
        currentState = State.Stunned;

        //what's this for?
        StartCoroutine(StunForLonger());
    }
    private IEnumerator StunForLonger()
    {
        for (int i = 0; i < 60; i++)
        {
            //inLightTime = timeBeforeStun;
            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }
    void Attack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            Debug.Log("Attack");
            StartCoroutine(WaitForAttackAnim());
        }
    }

    public void Killed()
    {
        currentState = State.Dead;
        //playDeathAnimation
        Destroy(gameObject);
    }

    IEnumerator WaitForAttackAnim()
    {
        yield return new WaitForSeconds(3);
        currentState = State.Flee;
        isAttacking = false;
    }

    void MoveIntoPlayer()
    {
        if (!isRepeating)
        {
            InvokeRepeating("UpdatePath", 0f, 0.5f);

            isRepeating = true;
        }
        if (path == null)
        {
            return;
        }
        if (currentWaypoint < path.vectorPath.Count)
        {
            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - (Vector2)transform.position).normalized;
            Vector2 translation = direction * speed * Time.deltaTime;
            float distance = Vector2.Distance(transform.position, path.vectorPath[currentWaypoint]);
            transform.Translate(translation);

            if (distance < nextWaypointDistance && currentWaypoint != path.vectorPath.Count - 1)
            {
                currentWaypoint++;
            }
        }
    }

    void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(transform.position, target.position, OnPathComplete);
        }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("OnTriggerEnter");
        Debug.Log(collision.gameObject.tag);
        if (currentState == State.MoveIn)
        {
            if (collision.gameObject.tag == "Flashlight")
            {
                CancelInvoke();
                isRepeating = false;
                currentState = State.Stunned;
                return;
            }
            else if (collision.gameObject.tag == "AttackRadius")
            {
                Debug.Log("Triggered by player");
                CancelInvoke();
                isRepeating = false;
                currentState = State.Attack;
                return;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("here");
        if (currentState == State.Stunned && collision.gameObject.tag == "Flashlight")
        {
            currentState = State.Flee;
            return;
        }
    }

    public void UpdateOpacity(float value)
    {
        spriteRenderer.color = new Color(1f, 1f, 1f, value);
    }
}
