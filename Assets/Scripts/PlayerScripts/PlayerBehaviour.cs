using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    [Tooltip("Number of pixels per second to move.")]
    public float moveSpeed;
    [HideInInspector]
    [Space]
    public float currentHealth;
    [Space]
    [Header("How far the distance is from the player in that direction where collisions are detected.")]
    public float distanceDownFromPlayerCenter;
    public float upColliderDistance;
    public float leftColliderDistance;
    public float rightColliderDistance;
    public float downColliderDistance;
    public bool showFeetColliders;
    [Space]

    public GameObject gameController;
    public GameObject spotlight;

    [Space]
    public float flashAbilityCooldown;
    public float flashlightActiveTime;
    public float rechargeScale;
    public float currentFlashlightTime;
    public bool flashlightBroke;
    public GameObject flashlight;
    public GameObject flashlightChargeUI;
    private RectTransform flashlightChargeUITransform;

    [Space]
    public string currentArea;

    private SpriteRenderer spriteRenderer;
    private WorldControl worldControl;
    [HideInInspector]
    public Animator animator;
    private string anim;

    private CapsuleCollider2D attackRange;
    private List<GameObject> playerCanAttack;
    private bool attacking;

    private float distance;

    private RaycastHit2D upCast;
    private RaycastHit2D leftCast;
    private RaycastHit2D rightCast;
    private RaycastHit2D downCast;
    private int objectMask;

    private bool blockUp;
    private bool blockLeft;
    private bool blockRight;
    private bool blockDown;
    [HideInInspector]
    public bool canMove;

    private string lastAnim;
    private bool flashlightEnabled;

    [HideInInspector]
    public int health;

    public bool canUseFlashAbility;
    private float flashAbilityCount;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        worldControl = gameController.GetComponent<WorldControl>();
        animator = GetComponent<Animator>();
        anim = "Idle";

        foreach (Transform t in transform)
        {
            if (t.name == "playerAttackRadius")
            {
                attackRange = t.gameObject.GetComponent<CapsuleCollider2D>();
            }
        }
        playerCanAttack = new List<GameObject>();

        health = 0;

        objectMask = LayerMask.GetMask("Object");

        currentFlashlightTime = 0;

        blockUp = false;
        blockLeft = false;
        blockRight = false;
        blockDown = false;

        attacking = false;
        canMove = true;

        canUseFlashAbility = false;
        flashAbilityCount = flashAbilityCooldown;

        if (flashlightActiveTime > 0)
        {
            flashlightEnabled = true;
        } else
        {
            flashlightEnabled = false;
        }

        if (flashlightChargeUI != null)
        {
            flashlightChargeUITransform = flashlightChargeUI.GetComponent<RectTransform>();
        }
    }

    void Update()
    {
        distance = moveSpeed * 0.5f * Time.deltaTime;

        if (worldControl.DialogueActive() == false && worldControl.paused == false && canMove == true)
        {
            if (Input.GetAxis("FlashAbility") > 0.1 && flashAbilityCount == 0 && canUseFlashAbility == true)
            {
                flashAbilityCount = flashAbilityCooldown;
                StartCoroutine(worldControl.LucasFlashEffect());

                if (GameObject.FindGameObjectWithTag("Boss") != null)
                {
                    GameObject.FindGameObjectWithTag("Boss").GetComponent<BossBehaviour>().FlashStunStartCoroutine();
                }
                if (GameObject.FindGameObjectWithTag("Enemy") != null)
                {
                    foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
                    {
                        enemy.GetComponent<EnemyBehaviour>().FlashStun();
                    }
                }
            }

            if (Input.GetAxis("UseFlashlight") > 0.1 && currentFlashlightTime < flashlightActiveTime && flashlightBroke == false)
            {
                flashlight.SetActive(true);
                currentFlashlightTime += Time.deltaTime;
                if (currentFlashlightTime >= flashlightActiveTime)
                {
                    flashlightBroke = true;
                }
            }
            else
            {
                flashlight.SetActive(false);
                currentFlashlightTime = Mathf.Clamp(currentFlashlightTime - (Time.deltaTime / rechargeScale), 0, Mathf.Infinity);
                if (currentFlashlightTime == 0)
                {
                    flashlightBroke = false;
                }
            }

            if (flashlightChargeUI != null)
            {
                var flashlightBarRect = flashlightChargeUITransform.transform as RectTransform;
                flashlightChargeUITransform.sizeDelta = new Vector2(119 - (119 * (currentFlashlightTime / flashlightActiveTime)), flashlightChargeUITransform.sizeDelta.y);
            }

            if (Input.GetAxis("Horizontal") > 0.1 && blockRight == false)
            {
                transform.Translate(new Vector3(distance, 0, 0));
                worldControl.UpdateCameraIfFollowing();
                spotlight.transform.eulerAngles = new Vector3(0, 0, -90);

                if (anim != "WalkRight" && anim != "WalkRightFlashlight")
                {
                    anim = "WalkRight";
                    if (flashlightEnabled)
                    {
                        anim += "Flashlight";
                    }
                    animator.SetTrigger(anim);
                }
            }
            else if (Input.GetAxis("Vertical") > 0.1 && blockUp == false)
            {
                transform.Translate(new Vector3(0, distance, 0));
                worldControl.UpdateCameraIfFollowing();
                spotlight.transform.eulerAngles = new Vector3(0, 0, 0);

                if (anim != "WalkBack" && anim != "WalkBackFlashlight")
                {
                    anim = "WalkBack";
                    if (flashlightEnabled)
                    {
                        anim += "Flashlight";
                    }
                    animator.SetTrigger(anim);
                }
            }
            else if (Input.GetAxis("Horizontal") < -0.1 && blockLeft == false)
            {
                transform.Translate(new Vector3(-distance, 0, 0));
                worldControl.UpdateCameraIfFollowing();
                spotlight.transform.eulerAngles = new Vector3(0, 0, 90);

                if (anim != "WalkLeft" && anim != "WalkLeftFlashlight")
                {
                    anim = "WalkLeft";
                    if (flashlightEnabled)
                    {
                        anim += "Flashlight";
                    }
                    animator.SetTrigger(anim);
                }
            }
            else if (Input.GetAxis("Vertical") < -0.1 && blockDown == false)
            {
                transform.Translate(new Vector3(0, -distance, 0));
                worldControl.UpdateCameraIfFollowing();
                spotlight.transform.eulerAngles = new Vector3(0, 0, 180);

                if (anim != "WalkFront" && anim != "WalkFrontFlashlight")
                {
                    anim = "WalkFront";
                    if (flashlightEnabled)
                    {
                        anim += "Flashlight";
                    }
                    animator.SetTrigger(anim);
                }
            }
            else
            {
                if (anim != "Idle" && anim != "IdleFlashlight")
                {
                    anim = "Idle";
                    if (flashlightEnabled)
                    {
                        anim += "Flashlight";
                    }
                    animator.SetTrigger(anim);
                }
            }

            flashAbilityCount -= Time.deltaTime;
            flashAbilityCount = Mathf.Clamp(flashAbilityCount, 0, flashAbilityCooldown);
        }
        else
        {
            if (anim != "Idle" && anim != "IdleFlashlight")
            {
                anim = "Idle";
                if (flashlightEnabled)
                {
                    anim += "Flashlight";
                }
                animator.SetTrigger(anim);
            }
        }

        // Cast rays in all 4 directions for wall detection
        upCast = Physics2D.Raycast(transform.position - (Vector3.up * distanceDownFromPlayerCenter), Vector2.up, upColliderDistance, objectMask);
        leftCast = Physics2D.Raycast(transform.position - (Vector3.up * distanceDownFromPlayerCenter), Vector2.left, leftColliderDistance, objectMask);
        rightCast = Physics2D.Raycast(transform.position - (Vector3.up * distanceDownFromPlayerCenter), Vector2.right, rightColliderDistance, objectMask);
        downCast = Physics2D.Raycast(transform.position - (Vector3.up * distanceDownFromPlayerCenter), Vector2.down, downColliderDistance, objectMask);

        if (showFeetColliders)
        {
            Debug.DrawRay(transform.position - (Vector3.up * distanceDownFromPlayerCenter), Vector2.up * upColliderDistance, Color.red);
            Debug.DrawRay(transform.position - (Vector3.up * distanceDownFromPlayerCenter), Vector2.left * leftColliderDistance, Color.red);
            Debug.DrawRay(transform.position - (Vector3.up * distanceDownFromPlayerCenter), Vector2.right * rightColliderDistance, Color.red);
            Debug.DrawRay(transform.position - (Vector3.up * distanceDownFromPlayerCenter), Vector2.down * downColliderDistance, Color.red);
        }

        if (upCast.collider != null && upCast.collider.tag == "Wall")
        {
            blockUp = true;
        }
        else { blockUp = false; }

        if (leftCast.collider != null && leftCast.collider.tag == "Wall")
        {
            blockLeft = true;
        }
        else { blockLeft = false; }

        if (rightCast.collider != null && rightCast.collider.tag == "Wall")
        {
            blockRight = true;
        }
        else { blockRight = false; }

        if (downCast.collider != null && downCast.collider.tag == "Wall")
        {
            blockDown = true;
        }
        else { blockDown = false; }
    }

    void LateUpdate()
    {
        // Attack enemies
        if (Input.GetAxis("Attack") > 0 && worldControl.DialogueActive() == false && worldControl.paused == false && canMove == true)
        {
            canMove = false;
            StartCoroutine(PlayAttackAnimation());

            for (int i = 0; i < playerCanAttack.Count; i++)
            {
                EnemyBehaviour enemy = playerCanAttack[i].GetComponent<EnemyBehaviour>();
                enemy.Killed();
            }
        }
    }

    public IEnumerator PlayAttackAnimation()
    {
        anim = "Attack";
        animator.SetTrigger(anim);
        for (int i = 0; i < playerCanAttack.Count; i++)
        {
            //Destroy(playerCanAttack[i]);
            EnemyBehaviour enemy = playerCanAttack[i].GetComponent<EnemyBehaviour>();
            enemy.Killed();
        }

        yield return new WaitForSeconds(1.0f);

        anim = "Idle";
        animator.SetTrigger(anim);
        canMove = true;
    }

    public IEnumerator PlayBipedalHurtAnimation()
    {
        anim = "BipedalAttack";
        animator.SetTrigger(anim);
        
        yield return new WaitForSeconds(0.667f * 2);

        anim = "Idle";
        animator.SetTrigger(anim);
    }
    public IEnumerator PlayBipedalKillAnimation()
    {
        anim = "BipedalKill";
        animator.SetTrigger(anim);

        yield return new WaitForSeconds(3);

        anim = "Idle";
        animator.SetTrigger(anim);
    }

    public IEnumerator PlayBossHurtAnimation()
    {
        anim = "BossAttack";
        animator.SetTrigger(anim);

        yield return new WaitForSeconds(0.667f * 2);

        anim = "Idle";
        animator.SetTrigger(anim);

        yield return null;
    }
    public IEnumerator PlayBossKillAnimation()
    {
        anim = "BossKill";
        animator.SetTrigger(anim);

        yield return new WaitForSeconds(0.667f * 2);

        anim = "Idle";
        animator.SetTrigger(anim);

        yield return null;
    }

    public void SetArea(string area)
    {
        currentArea = area;
    }

    public void AbleToAttack(GameObject obj)
    {
        if (!playerCanAttack.Contains(obj))
        {
            playerCanAttack.Add(obj);
            obj.GetComponent<EnemyBehaviour>().ShowAttackIndicator();
        }
    }

    public void NotAbleToAttack(GameObject obj)
    {
        if (playerCanAttack.Contains(obj))
        {
            playerCanAttack.Remove(obj);
            obj.GetComponent<EnemyBehaviour>().HideAttackIndicator();
        }
    }

    public void FlashAbility(bool value)
    {
        canUseFlashAbility = value;
    }
}
