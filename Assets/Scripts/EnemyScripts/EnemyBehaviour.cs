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
    private EnemyAudio enemyAudio;

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

    bool isRepeating = false;
    bool isAttacking = false;
    bool isStunned = false;

    private float currentOpacity;
    private int flashlightLayerMask;

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

        flashlightLayerMask = LayerMask.GetMask("Flashlight");

        UpdateOpacity(0.2f);
        currentState = State.MoveIn;

        enemyAudio = GetComponent<EnemyAudio>();
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
        UpdateOpacity(0.2f);
        transform.position = spawnPosition;
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
            enemyAudio.playStun();
        }

    }
    public void FlashStun()
    {
        currentState = State.Stunned;
    }
    private IEnumerator StunForLonger()
    {
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

            if (collision.gameObject.tag == "EnemySound")
            {
                StartCoroutine(WaitForSound());
                enemyAudio.playSound();
            }
        }
    }

    private IEnumerator WaitForSound()
    {
        //yield return new WaitForSeconds(3);
        yield return new WaitUntil(() => !(enemyAudio.enemyAudioSource.isPlaying));
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (currentState == State.Stunned && collision.gameObject.tag == "Flashlight")
        {
            currentState = State.Flee;
            isStunned = false;
            return;
        }
    }

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
            worldControl.dialogueActive = true;
            StartCoroutine(worldControl.TakeBipedalDamage(gameObject));
            StartCoroutine(WaitForAttackAnim());
            
        }
    }
    IEnumerator WaitForAttackAnim()
    {
        yield return new WaitForSeconds(0.667f * 4);
        currentState = State.Flee;
        isAttacking = false;
        worldControl.dialogueActive = false;
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
        enemyAudio.playDead();
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
            UpdateOpacity(0.2f);
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

            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance + 0.01f, flashlightLayerMask);

            //trying to stop them moving into the flashlight here, maybe if they're standing still don't play any sound?
            if (!hit)
            {
                transform.Translate(translation);
                if (distance < nextWaypointDistance && currentWaypoint != path.vectorPath.Count - 1)
                {
                    currentWaypoint++;
                }
            }
            else
            {
                Debug.Log("Avoiding Flashlight");
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
