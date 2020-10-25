using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

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
    private int bossHealth;

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
    private PlayerAudio playerAudio;

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
        bossHealth = 8;

        bossSwiping = false;
        bossPentagram = false;
        bossDarkness = false;
        bossStunned = false;

        bossAudio = GameObject.Find("BossFightAudio").GetComponent<BossFightAudio>();
        playerAudio = GameObject.Find("PlayerAudio").GetComponent<PlayerAudio>();
    }

    void Update()
    {
        if (showFeetColliders)
        {
            Debug.DrawRay(transform.position - (Vector3.up * distanceDownFromBossCenter), Vector2.up * upColliderDistance, Color.red);
            Debug.DrawRay(transform.position - (Vector3.up * distanceDownFromBossCenter), Vector2.down * downColliderDistance, Color.red);
        }

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

            transform.Translate(enemyTranslatePos * (moveSpeed / 32));

            if (bossPhaseTimer >= timeBeforePentagramAttack && bossPentagram == false)
            {
                bossMove = false;
                bossPentagram = true;

                anim = "Idle";
                animator.SetTrigger(anim);

                StartCoroutine(PentagramAttackCoroutine());
            }

            if (bossPhaseTimer >= timeBeforeDarknessAttack && bossDarkness == false)
            {
                bossMove = false;
                bossDarkness = true;

                anim = "Idle";
                animator.SetTrigger(anim);

                StartCoroutine(DarknessAttackCoroutine());
            }
        }

        if (bossPhase == 2 && worldControl.paused == false)
        {
            bossPhase++;
            cutsceneControl.StartCutscene("MidBossFight");
        }

        if (bossHealth == 0 && worldControl.paused == false)
        {
            bossMove = false;
            bossStunned = false;
            cutsceneControl.StartCutscene("EndBossFight");
        }
    }

    void LateUpdate()
    {
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

        if (Input.GetAxis("Attack") > 0 && worldControl.paused == false && bossStunned == true)
        {
            bossAudio.PlayDamaged();

            Debug.Log(bossHealth);
            bossHealth--;

            StopCoroutine(bossStunnedCoroutine);

            anim = "Idle";
            animator.SetTrigger(anim);

            bossStunned = false;
            bossMove = false;
            StartCoroutine(InterruptStunReturnToNormal());
        }
    }

    public void FlashStunStartCoroutine()
    {
        bossStunnedCoroutine = FlashStun();
        StartCoroutine(bossStunnedCoroutine);
    }

    public void BeginFirstPhase()
    {
        bossMove = true;
    }
    public void BeginSecondPhase()
    {
        worldControl.SetLightIntensity(brightLevel);

        moveSpeed = moveSpeed * 3;
        animator.speed = animator.speed * 3;

        bossPhaseTimer = 0;
        bossPentagram = false;
        bossDarkness = false;
        bossMove = true;

        player.GetComponent<PlayerBehaviour>().FlashAbility(true);
    }

    public bool BossIsStunned()
    {
        return bossStunned;
    }

    public void AttackIndicatorActive(bool value)
    {
        attackIndicator.SetActive(value);
    }

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
    private Vector2 RelativePlayerPos()
    {
        return player.transform.position - transform.position;
    }

    public void SwipeAttack(BossSwipeRadius bossSwipeRadius)
    {
        if (bossMove == true && worldControl.paused == false)
        {
            bossMove = false;

            if (bossSwiping == false)
            {
                bossSwiping = true;
                anim = "Idle";
                animator.SetTrigger(anim);

                StartCoroutine(SwipeAttackCoroutine(bossSwipeRadius));
            }
        }
    }
    private IEnumerator SwipeAttackCoroutine(BossSwipeRadius bossSwipeRadius)
    {
        yield return new WaitForSeconds(swipeAttackDelay);

        anim = "Swipe";
        animator.SetTrigger(anim);

        yield return new WaitForSeconds(0.5f);
        if (bossSwipeRadius.playerInRange == true)
        {
            StartCoroutine(worldControl.TakeBossDamage());
            yield return new WaitForSeconds(1.0f);
        }
        yield return new WaitForSeconds(0.25f);

        bossMove = true;
        bossSwiping = false;
        yield return null;
    }
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
    private IEnumerator DarknessAttackCoroutine()
    {
        bossMove = false;

        for (float i = brightLevel; i >= darkLevel; i -= (brightLevel - darkLevel) / dimmingScale)
        {
            worldControl.SetLightIntensity(i);
            yield return new WaitForSeconds((brightLevel - darkLevel) / dimmingScale);
        }

        yield return new WaitForSeconds(0.5f);

        foreach (Vector2 position in enemySpawnPositions)
        {
            GameObject enemy = Instantiate(enemyPrefab, enemyParentTransform.transform, true);
            enemy.transform.position = new Vector3(position.x, position.y, 0);
        }

        yield return new WaitForSeconds(0.5f);

        moveSpeed = moveSpeed / 3;
        animator.speed = animator.speed / 3;
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
            worldControl.SetLightIntensity(i);
            yield return new WaitForSeconds((brightLevel - darkLevel) / dimmingScale);
        }

        yield return new WaitForSeconds(0.5f);

        moveSpeed = moveSpeed * 3;
        animator.speed = animator.speed * 3;

        bossPhaseTimer = 0;
        bossPentagram = false;
        bossDarkness = false;
        bossMove = true;

        yield return null;
    }
    private IEnumerator FlashStun()
    {
        bossMove = false;
        bossStunned = true;

        bossAudio.PlayStun();
        anim = "Stagger";
        animator.SetTrigger(anim);
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        anim = "Idle";
        animator.SetTrigger(anim);

        bossMove = true;
        bossStunned = false;

        yield return null;
    }
    private IEnumerator InterruptStunReturnToNormal()
    {
        yield return new WaitForSeconds(1);
        bossMove = true;
        yield return null;
    }
}
