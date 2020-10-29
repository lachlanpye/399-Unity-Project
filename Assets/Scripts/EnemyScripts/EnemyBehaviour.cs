using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;


//Behaviour written and debugged by Tyler
//Audio by Janine
//edited to work with boss fight by Lachlan
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

    [SerializeField] float speed = 5f;
    [SerializeField] float nextWaypointDistance = 1f;
    [SerializeField] float visibilityDistance = 10f;
    [SerializeField] float minRespawnDistance = 2f;
    [SerializeField] float stunTime = 4f;

    private Path path;
    private Vector2 spawnPosition;
    private int currentWaypoint = 0;

    public State currentState;

    private bool isRepeating = false;
    private bool isAttacking = false;
    private bool isStunned = false;
    private bool isWaitingToRespawn = false;

    private int flashlightLayerMask;
    private string currentDirection = "f";

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

        seeker = GetComponent<Seeker>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        worldControl = gameController.GetComponent<WorldControl>();

        attackIndicator = transform.Find("attackIndicator").gameObject;
        attackIndicator.SetActive(false);

        flashlightLayerMask = LayerMask.GetMask("Flashlight");

        spriteRenderer.enabled = false;
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
        if (!isWaitingToRespawn)
        {
            Debug.Log("Flee");
            HideAttackIndicator();
            spriteRenderer.enabled = false;
            transform.position = spawnPosition;
            isWaitingToRespawn = true;
        }
        if (Vector2.Distance(transform.position, target.position) >= minRespawnDistance)
        {
            currentState = State.MoveIn;
            isWaitingToRespawn = false;
        }
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

    //stunning code (lol)
    void Stunned()
    {
        if(!isStunned)
        {
            Debug.Log("Stunned");
            spriteRenderer.enabled = true;
            UpdateOpacity(1f);
            isStunned = true;
            animator.SetTrigger("Stunned");
            StartCoroutine(StunForLonger());
            enemyAudio.playStun();
        }

    }
    private IEnumerator StunForLonger()
    {
        yield return new WaitForSeconds(stunTime);
        isStunned = false;
        if (currentState == State.Stunned)
        {
            currentState = State.Flee;
        }
    }

    //called by player when they use the stun ability in boss fight
    public void FlashStun()
    {
        if(currentState == State.MoveIn)
        {
            CancelInvoke();
            isRepeating = false;
            currentState = State.Stunned;
        }
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
        yield return new WaitUntil(() => !(enemyAudio.enemyAudioSource.isPlaying));
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
            worldControl.StartTakeBipdealDamageCoroutine(gameObject);
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
        if (currentState == State.MoveIn && !worldControl.playerIsInvulnerable)
        {
            CancelInvoke();
            isRepeating = false;
            currentState = State.Attack;
        }
    }

    //death code
    //called by player when they attack enemy
    public void Killed()
    {
        currentState = State.Dead;
        animator.SetTrigger("Killed");
        enemyAudio.playDead();
    }

    //called at end of death animation and when the boss dies
    public void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    //move code
    void MoveIntoPlayer()
    {
        //repeately looks for a path from A* package
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

            CalculateOpacity();

            Vector2 translation = direction * speed * Time.deltaTime;

            PlayWalkAnimation(translation);

            float distance = Vector2.Distance(transform.position, path.vectorPath[currentWaypoint]);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance + 0.01f, flashlightLayerMask);

            //stop them moving into the flashlight
            if (!hit)
            {
                transform.Translate(translation);
                if (distance < nextWaypointDistance && currentWaypoint != path.vectorPath.Count - 1)
                {
                    currentWaypoint++;
                }
            }
        }
    }

    void PlayWalkAnimation(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x < 0 && currentDirection != "l")
            {
                animator.SetTrigger("WalkLeft");
                currentDirection = "l";
            }
            else if (direction.x >= 0 && currentDirection != "r")
            {
                animator.SetTrigger("WalkRight");
                currentDirection = "r";
            }
        }
        else
        {
            if (direction.y > 0 && currentDirection != "b")
            {
                animator.SetTrigger("WalkBack");
                currentDirection = "b";
            }
            else if (direction.y <= 0 && currentDirection != "f")
            {
                animator.SetTrigger("WalkFront");
                currentDirection = "f";
            }
        }

    }

    private void CalculateOpacity()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, target.position);

        if (distanceToPlayer < visibilityDistance)
        {
            if (spriteRenderer.enabled == false)
            {
                spriteRenderer.enabled = true;
            }
            float alpha = (visibilityDistance - distanceToPlayer) / visibilityDistance;
            UpdateOpacity(alpha);
        }
        else
        {
            if (spriteRenderer.enabled == true)
            {
                spriteRenderer.enabled = false;
            }
        }
    }

    //find path using A* package
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
    }
}
