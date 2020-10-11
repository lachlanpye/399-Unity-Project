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
    public float downColliderDistance;
    public bool showFeetColliders;

    [Space]
    [Header("General attack variables")]
    public float timeBeforePentagramAttack;

    [Space]
    [Header("Swipe attack")]
    public float swipeAttackDelay;

    [Space]
    [Header("Pentagram attack")]
    public GameObject[] PentagramAttacks;
    public float delayBetweenPentagramAttacks;

    [Space]
    public bool bossMove;

    private bool bossSwiping;
    private bool bossPentagram;

    private RaycastHit2D upCast;
    private RaycastHit2D downCast;
    private int objectMask;

    private bool changeLeftRightDir;

    private Vector3 enemyTranslatePos;
    private Vector3 leftRightVector;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private string anim;

    private float bossPhaseTimer;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = "Idle";

        objectMask = LayerMask.GetMask("Object");

        bossPhaseTimer = 0;
    }

    void Update()
    {
        if (showFeetColliders)
        {
            Debug.DrawRay(transform.position - (Vector3.up * distanceDownFromBossCenter), Vector2.up * upColliderDistance, Color.red);
            Debug.DrawRay(transform.position - (Vector3.up * distanceDownFromBossCenter), Vector2.down * downColliderDistance, Color.red);
        }

        if (bossMove)
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
        }

        if (bossPhaseTimer >= timeBeforePentagramAttack && bossPentagram == false)
        {
            bossMove = false;
            bossPentagram = true;

            anim = "Idle";
            animator.SetTrigger(anim);

            StartCoroutine(PentagramAttackCoroutine());
        }
    }

    void LateUpdate()
    {
        if (bossMove)
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

    public void BeginFirstPhase()
    {
        bossMove = true;
    }

    public void SwipeAttack()
    {
        bossMove = false;

        if (bossSwiping == false)
        {
            bossSwiping = true;
            anim = "Idle";
            animator.SetTrigger(anim);

            StartCoroutine(SwipeAttackCoroutine());
        }
    }
    public IEnumerator SwipeAttackCoroutine()
    {
        yield return new WaitForSeconds(swipeAttackDelay);

        anim = "Swipe";
        animator.SetTrigger(anim);

        yield return new WaitForSeconds(0.5f);

        bossMove = true;
        bossSwiping = false;
        yield return null;
    }

    public IEnumerator PentagramAttackCoroutine()
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
        bossPentagram = false;
        bossPhaseTimer = 0;

        yield return null;
    }
}
