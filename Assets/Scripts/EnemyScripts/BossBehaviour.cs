using System.Collections;
using UnityEngine;

// Component that controls how the boss operates and attacks the player.
// Debugged by Tyler
public class BossBehaviour : MonoBehaviour
{
    public GameObject player;
    public GameObject gameController;
    public GameObject cutsceneController;
    [Space]
    public GameObject attackIndicator; 

    [Space]
    public float moveSpeed;
    [Space]
    public float distanceDownFromBossCenter;
    public float upColliderDistance;
    public float downColliderDistance;
    public bool showFeetColliders;

    [Space]
    [Header("General attack variables")]
    public float timeBeforePentagramAttack;
    public float timeBeforeDarknessAttack;

    [Space]
    [Header("Swipe attack")]
    public float swipeAttackDelay;

    [Space]
    [Header("Pentagram attack")]
    public GameObject[] PentagramAttacks;
    public float delayBetweenPentagramAttacks;

    [Space]
    [Header("Darkness attack")]
    public Vector2[] enemySpawnPositions;
    public GameObject enemyParentTransform;
    public GameObject enemyPrefab;
    public float dimmingScale;
    public float brightLevel;
    public float darkLevel;

    [Space]
    public bool bossMove;
    private int bossPhase;
    public int bossHealth = 8;
    public float stunTime = 3f;
    public float damageDistance = 1f;

    private bool bossSwiping;
    private bool bossPentagram;
    private bool bossDarkness;
    public bool bossStunned;

    private RaycastHit2D upCast;
    private RaycastHit2D downCast;
    private int objectMask;

    private bool changeLeftRightDir;

    private Vector3 enemyTranslatePos;
    private Vector3 leftRightVector;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private WorldControl worldControl;
    private CutsceneControl cutsceneControl;
    private string anim;

    private float bossPhaseTimer;

    private IEnumerator bossStunnedCoroutine;

    private BossFightAudio bossAudio;

    /// <summary>
    /// Lachlan Pye
    /// Initialize variables.
    /// </summary>
    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        worldControl = gameController.GetComponent<WorldControl>();
        cutsceneControl = cutsceneController.GetComponent<CutsceneControl>();

        attackIndicator.SetActive(false);

        anim = "Idle";

        objectMask = LayerMask.GetMask("Object");

        bossPhaseTimer = 0;
        bossPhase = 0;

        bossSwiping = false;
        bossPentagram = false;
        bossDarkness = false;
        bossStunned = false;

        bossAudio = GameObject.Find("BossFightAudio").GetComponent<BossFightAudio>();
    }

    /// <summary>
    /// Lachlan Pye
    /// Provides various update functions.
    /// </summary>
    void Update()
    {
        if (showFeetColliders)
        {
            Debug.DrawRay(transform.position - (Vector3.up * distanceDownFromBossCenter), Vector2.up * upColliderDistance, Color.red);
            Debug.DrawRay(transform.position - (Vector3.up * distanceDownFromBossCenter), Vector2.down * downColliderDistance, Color.red);
        }

        // Lachlan Pye
        // If the boss is not currently stunned, move them in the direction of the player
        // and play the correct animation if the player is above or below them on the y axis.
        // If the boss timer is high enough, then trigger either the pentagram attack or the darkness attack.
        if (bossMove && bossPhase != 2 && worldControl.paused == false)
        {
            bossPhaseTimer += Time.deltaTime;
            enemyTranslatePos = (player.transform.position - transform.position).normalized;

            upCast = Physics2D.Raycast(transform.position - (Vector3.up * distanceDownFromBossCenter), Vector2.up, upColliderDistance, objectMask);
            downCast = Physics2D.Raycast(transform.position - (Vector3.up * distanceDownFromBossCenter), Vector2.down, downColliderDistance, objectMask);

            if (upCast.collider != null && RelativePlayerPos().y > 0)
            {
                YDirectionBlocked();
            }
            else if (downCast.collider != null && RelativePlayerPos().y < 0)
            {
                YDirectionBlocked();
            }
            else { changeLeftRightDir = true; }
            if (!bossStunned)
            {
                if (enemyTranslatePos.y <= 0 && animator.GetCurrentAnimatorStateInfo(0).IsName("bossWalkFront") == false)
                {
                    anim = "WalkFront";
                    animator.SetTrigger(anim);
                }
                else if (enemyTranslatePos.y > 0 && animator.GetCurrentAnimatorStateInfo(0).IsName("bossWalkBack") == false)
                {
                    anim = "WalkBack";
                    animator.SetTrigger(anim);
                }

                transform.Translate(enemyTranslatePos * moveSpeed * Time.deltaTime);
            }

            if (bossPhaseTimer >= timeBeforePentagramAttack && bossPentagram == false)
            {
                bossMove = false;
                bossPentagram = true;
                if (!bossStunned) {
                    anim = "Idle";
                    animator.SetTrigger(anim);
                }

                StartCoroutine(PentagramAttackCoroutine());
            }

            if (bossPhaseTimer >= timeBeforeDarknessAttack && bossDarkness == false)
            {
                bossMove = false;
                bossDarkness = true;
                if (!bossStunned)
                {
                    anim = "Idle";
                    animator.SetTrigger(anim);
                }

                StartCoroutine(DarknessAttackCoroutine());
            }
        }

        // Lachlan Pye
        // If the boss has done enough attacks, start the mid boss fight cutscene.
        if (bossPhase == 2 && worldControl.paused == false)
        {
            bossPhase++;
            cutsceneControl.StartCutscene("MidBossFight");
        }

        // Lachlan Pye
        // If the boss has lost all of their health, start the end boss fight cutscene.
        if (bossHealth == 0 && worldControl.paused == false)
        {
            bossMove = false;
            bossStunned = false;

            foreach (Transform t in enemyParentTransform.transform)
            {
                t.gameObject.GetComponent<EnemyBehaviour>().DestroyEnemy();
            }

            cutsceneControl.StartCutscene("EndBossFight");
        }
    }

    void LateUpdate()
    {
        // Lachlan Pye
        // Flip the boss' sprite depending on the player's x position relative to the boss.
        if (bossMove && worldControl.paused == false)
        {
            if (anim == "WalkFront")
            {
                if (enemyTranslatePos.x <= 0)
                {
                    spriteRenderer.flipX = true;
                }
                else if (enemyTranslatePos.x > 0)
                {
                    spriteRenderer.flipX = false;
                }
            }
            else if (anim == "WalkBack")
            {
                if (enemyTranslatePos.x > 0)
                {
                    spriteRenderer.flipX = true;
                }
                else if (enemyTranslatePos.x <= 0)
                {
                    spriteRenderer.flipX = false;
                }
            }
        }

        // Lachlan Pye
        // If the player uses the Attack key and the boss is stunned, and the player is close enough, 
        // decrease the boss' health.
        if (Input.GetAxis("Attack") > 0 && worldControl.paused == false && bossStunned == true)
        {
            float distToPlayer = Vector2.Distance(transform.position, player.transform.position);
            if (distToPlayer <= damageDistance)
            {
                bossAudio.PlayDamaged();

                bossHealth--;

                attackIndicator.SetActive(false);
                StopCoroutine(bossStunnedCoroutine);
                bossStunned = false;
                bossMove = false;
                StartCoroutine(InterruptStunReturnToNormal());
            }
        }
    }

    /// <summary>
    /// Lachlan Pye
    /// Start the first phase of the boss fight.
    /// </summary>
    public void BeginFirstPhase()
    {
        bossMove = true;
    }

    /// <summary>
    /// Lachlan Pye
    /// Start the second phase of the boss fight.
    /// </summary>
    public void BeginSecondPhase()
    {
        worldControl.SetLightIntensity(brightLevel);

        moveSpeed = moveSpeed * 2;
        //animator.speed = animator.speed * 3;

        bossPhaseTimer = 0;
        bossPentagram = false;
        bossDarkness = false;
        bossMove = true;

        player.GetComponent<PlayerBehaviour>().FlashAbility(true);
    }

    /// <summary>
    /// Lachlan Pye
    /// Returns whether the boss is currently stunned or not.
    /// </summary>
    /// <returns></returns>
    public bool BossIsStunned()
    {
        return bossStunned;
    }

    /// <summary>
    /// Lachlan Pye
    /// Set the attack indicator icon to be active or inactive.
    /// </summary>
    /// <param name="value">Whether the attack indicator icon should be active or inactive.</param>
    public void AttackIndicatorActive(bool value)
    {
        attackIndicator.SetActive(value);
    }

    /// <summary>
    /// Lachlan Pye
    /// If the boss is blocked in the y direction, then move them to the left or right depending on the player's x position.
    /// </summary>
    private void YDirectionBlocked()
    {
        if (changeLeftRightDir != false)
        {
            if (RelativePlayerPos().x < 0)
            {
                leftRightVector = Vector3.left;
            }
            else
            {
                leftRightVector = Vector3.right;
            }
        }

        enemyTranslatePos = leftRightVector;
        changeLeftRightDir = false;
    }

    /// <summary>
    /// Lachlan Pye
    /// Gets the relative position of the player to the boss.
    /// </summary>
    /// <returns></returns>
    private Vector2 RelativePlayerPos()
    {
        return player.transform.position - transform.position;
    }

    /// <summary>
    /// Lachlan Pye
    /// Stops the boss and starts the SwipeAttack coroutine.
    /// </summary>
    /// <param name="bossSwipeRadius">The component that is calling the function.</param>
    public void SwipeAttack(BossSwipeRadius bossSwipeRadius)
    {
        if (bossMove == true && worldControl.paused == false)
        {
            bossMove = false;

            if (bossSwiping == false && !bossStunned)
            {
                bossSwiping = true;
                anim = "Idle";
                animator.SetTrigger(anim);

                StartCoroutine(SwipeAttackCoroutine(bossSwipeRadius));
            }
        }
    }

    /// <summary>
    /// Lachlan Pye
    /// Waits for some time before playing the swipe animation and damaging the player if they are near.
    /// </summary>
    /// <param name="bossSwipeRadius">The component that called the function that is now calling the coroutine.</param>
    private IEnumerator SwipeAttackCoroutine(BossSwipeRadius bossSwipeRadius)
    {
        yield return new WaitForSeconds(swipeAttackDelay);

        if (!bossStunned)
        {
            anim = "Swipe";
            animator.SetTrigger(anim);

            yield return new WaitForSeconds(0.5f);
            if (bossSwipeRadius.playerInRange == true)
            {
                worldControl.StartBossDamageCoroutine();
                yield return new WaitForSeconds(1.0f);
                anim = "Idle";
            }
            //yield return new WaitForSeconds(0.25f);
            bossMove = true;
        }
        
        bossSwiping = false;
        yield return null;
    }
    
    /// <summary>
    /// Lachlan Pye
    /// Activate each pentagram in a sequence.
    /// </summary>
    private IEnumerator PentagramAttackCoroutine()
    {
        foreach (GameObject pentagramParent in PentagramAttacks)
        {
            foreach (Transform pentagram in pentagramParent.transform)
            {
                pentagram.gameObject.SetActive(true);
                pentagram.gameObject.GetComponent<PentagramAttackLogic>().BeginPentagramSequence();
            }
            yield return new WaitForSeconds(delayBetweenPentagramAttacks);
        }

        yield return new WaitForSeconds(1);

        bossMove = true;

        yield return null;
    }

    /// <summary>
    /// Lachlan Pye
    /// Darkens the area, slows down the boss and summons a number of enemies for the player to fight.
    /// Once all enemies are defeated, increase the light level and return the boss to normal speed.
    /// </summary>
    private IEnumerator DarknessAttackCoroutine()
    {
        bossMove = false;

        for (float i = brightLevel; i >= darkLevel; i -= (brightLevel - darkLevel) / dimmingScale)
        {
            yield return new WaitForSeconds((brightLevel - darkLevel) / dimmingScale);
            worldControl.SetLightIntensity(i);
        }

        yield return new WaitForSeconds(0.5f);

        foreach (Vector2 position in enemySpawnPositions)
        {
            GameObject enemy = Instantiate(enemyPrefab, enemyParentTransform.transform, true);
            enemy.transform.position = new Vector3(position.x, position.y, 0);
        }

        yield return new WaitForSeconds(0.5f);

        moveSpeed = moveSpeed / 2;
        //animator.speed = animator.speed / 3;
        bossMove = true;

        while (enemyParentTransform.transform.childCount != 0)
        {
            yield return null;
        }

        bossMove = false;

        bossPhase++;
        yield return new WaitForSeconds(0.5f);

        for (float i = darkLevel; i <= brightLevel; i += (brightLevel - darkLevel) / dimmingScale)
        {
            yield return new WaitForSeconds((brightLevel - darkLevel) / dimmingScale);
            worldControl.SetLightIntensity(i);
        }

        yield return new WaitForSeconds(0.5f);

        moveSpeed = moveSpeed * 2;
        //animator.speed = animator.speed * 3;

        bossPhaseTimer = 0;
        bossPentagram = false;
        bossDarkness = false;
        bossMove = true;

        yield return null;
    }

    public void FlashStunStartCoroutine()
    {
        bossStunnedCoroutine = FlashStun();
        StartCoroutine(bossStunnedCoroutine);
    }

    private IEnumerator FlashStun()
    {
        bossMove = false;
        bossStunned = true;

        bossAudio.PlayStun();
        anim = "Stagger";
        animator.SetTrigger(anim);
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(stunTime);
        //animator.GetCurrentAnimatorStateInfo(0).length
        anim = "Idle";
        animator.SetTrigger(anim);

        bossMove = true;
        bossStunned = false;

        yield return null;
    }

    private IEnumerator InterruptStunReturnToNormal()
    {
        anim = "Hurt";
        animator.SetTrigger(anim);
        yield return new WaitForSeconds(1);
        bossMove = true;
        
        anim = "Idle";

        bossStunned = false;
        yield return null;
    }
}
