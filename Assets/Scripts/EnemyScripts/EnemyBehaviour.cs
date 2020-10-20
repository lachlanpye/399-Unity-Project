using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyBehaviour : MonoBehaviour
{
    public Transform target;
    public GameObject gameController;
    public GameObject playerObject;

    private SpriteRenderer spriteRenderer;
    private Seeker seeker;
    private Animator animator;
    private WorldControl worldControl;
    private GameObject attackIndicator;

    private Vector3 spawnPosition;
    private Vector3 orthogonalVector;
    private Vector3 nextPosition;
    private int intervalOfNodes;
    private float randomMoveNum;
    public float speed = 5f;
    public float nextWaypointDistance = 2f;

    private Path path;
    private int currentWaypoint = 0;

    public State currentState;
    private Vector3 startPosition;

    bool isRepeating = false;
    bool isAttacking = false;
    bool isStunned = false;

    private float currentOpacity;

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
        spawnPosition = transform.position;
        playerObject = GameObject.FindGameObjectWithTag("Player");
        target = playerObject.transform;
        gameController = GameObject.FindGameObjectWithTag("GameController");

        nextPosition = new Vector3();
        randomMoveNum = Random.value * (2 * Mathf.PI);

        seeker = GetComponent<Seeker>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        worldControl = gameController.GetComponent<WorldControl>();
        playerObject = GameObject.FindGameObjectWithTag("Player");


        attackIndicator = transform.Find("attackIndicator").gameObject;
        attackIndicator.SetActive(false);
        startPosition = transform.position;

        UpdateOpacity(0.25f);
        currentState = State.MoveIn;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //pause everything if world is paused, in dialogue, or the player is currently being attacked
        if (!worldControl.DialogueActive() && !worldControl.paused)
        {
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

    }
    //fleeing code
    void Flee()
    {
        Debug.Log("Flee");
        HideAttackIndicator();
        UpdateOpacity(0.25f);
        transform.position = startPosition;
        currentState = State.MoveIn;
    }

    //stunning code (lol)
    void Stunned()
    {
        if(!isStunned)
        {
            Debug.Log("Stunned");
            UpdateOpacity(1f);
            isStunned = true;
            animator.SetTrigger("Stunned");
            StartCoroutine(StunForLonger());
        }

    }
    public void FlashStun()
    {
        currentState = State.Stunned;
    }
    private IEnumerator StunForLonger()
    {
        //for (int i = 0; i < 60; i++)
        //{
        //    //inLightTime = timeBeforeStun;
        //    yield return new WaitForEndOfFrame();
        //}
        yield return new WaitForSeconds(5);
        isStunned = false;
        if (currentState == State.Stunned)
        {
            currentState = State.Flee;
        }
        
        //yield return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (currentState == State.MoveIn)
        {
            if (collision.gameObject.tag == "Flashlight")
            {
                CancelInvoke();
                isRepeating = false;
                currentState = State.Stunned;
            }
        }
    }

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (currentState == State.Stunned && collision.gameObject.tag == "Flashlight")
    //    {
    //        currentState = State.Flee;
    //        isStunned = false;
    //        return;
    //    }
    //}

    public void ShowAttackIndicator()
    {
        attackIndicator.SetActive(true);
    }
    public void HideAttackIndicator()
    {
        attackIndicator.SetActive(false);
    }

    //attacking code
    void Attack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            Debug.Log("Attack");
            //To Do: call damage function on player so that player takes damage and plays attack animation
            StartCoroutine(WaitForAttackAnim());
        }
    }
    IEnumerator WaitForAttackAnim()
    {
        yield return new WaitForSeconds(4);
        currentState = State.Flee;
        isAttacking = false;
    }

    public void PlayerEntersRange()
    {
        if (currentState == State.MoveIn)
        {
            CancelInvoke();
            isRepeating = false;
            currentState = State.Attack;
        }
    }

    //death code
    public void Killed()
    {
        currentState = State.Dead;
        animator.SetTrigger("Killed");
    }

    public void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    //move code
    void MoveIntoPlayer()
    {
        if (!isRepeating)
        {
            InvokeRepeating("UpdatePath", 0f, 0.5f);
            UpdateOpacity(0.25f);
            isRepeating = true;
        }
        if (path == null)
        {
            return;
        }
        if (currentWaypoint < path.vectorPath.Count)
        {
            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - (Vector2)transform.position).normalized;
            //use direction to play correct animation
            //currently plays first animation frame over and over as this is called every update but isn't a big deal as should never actually see enemy walking
            //we just want them facing the right way when flashlight is shone on them
            if(Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                if (direction.x < 0)
                {
                    animator.SetTrigger("WalkLeft");
                } else
                {
                    animator.SetTrigger("WalkRight");
                }
            } else
            {
                if (direction.y > 0)
                {
                    animator.SetTrigger("WalkBack");
                }
                else
                {
                    animator.SetTrigger("WalkFront");
                }
            }

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

    public void UpdateOpacity(float value)
    {
        spriteRenderer.color = new Color(1f, 1f, 1f, value);
        currentOpacity = value;
    }
}
