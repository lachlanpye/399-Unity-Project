using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    [Tooltip("Number of pixels per second to move.")]
    public float moveSpeed;
    public float maxHealth;
    [HideInInspector]
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
    public float flashlightActiveTime;
    public float rechargeScale;
    public float currentFlashlightTime;
    public bool flashlightBroke;
    public GameObject flashlight;

    [Space]
    public string currentArea;

    private SpriteRenderer spriteRenderer;
    private WorldControl worldControl;
    private Animator animator;
    private string anim;

    private CapsuleCollider2D attackRange;
    private List<GameObject> playerCanAttack; 

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

        objectMask = LayerMask.GetMask("Object");

        currentFlashlightTime = 0;

        blockUp = false;
        blockLeft = false;
        blockRight = false;
        blockDown = false;
    }

    void Update()
    {
        distance = moveSpeed * 0.5f * Time.deltaTime;

        if (worldControl.DialogueActive() == false && worldControl.paused == false)
        {
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

            if (Input.GetAxis("Horizontal") > 0.1 && blockRight == false)
            {
                transform.Translate(new Vector3(distance, 0, 0));
                spotlight.transform.eulerAngles = new Vector3(0, 0, -90);

                if (anim != "WalkRight")
                {
                    animator.SetTrigger("WalkRight");
                    anim = "WalkRight";
                }
            }
            else if (Input.GetAxis("Horizontal") < -0.1 && blockLeft == false)
            {
                transform.Translate(new Vector3(-distance, 0, 0));
                spotlight.transform.eulerAngles = new Vector3(0, 0, 90);

                if (anim != "WalkLeft")
                {
                    animator.SetTrigger("WalkLeft");
                    anim = "WalkLeft";
                }
            }
            else if (Input.GetAxis("Vertical") > 0.1 && blockUp == false)
            {
                transform.Translate(new Vector3(0, distance, 0));
                spotlight.transform.eulerAngles = new Vector3(0, 0, 0);

                if (anim != "WalkBack")
                {
                    animator.SetTrigger("WalkBack");
                    anim = "WalkBack";
                }
            }
            else if (Input.GetAxis("Vertical") < -0.1 && blockDown == false)
            {
                transform.Translate(new Vector3(0, -distance, 0));
                spotlight.transform.eulerAngles = new Vector3(0, 0, 180);

                if (anim != "WalkFront")
                {
                    animator.SetTrigger("WalkFront");
                    anim = "WalkFront";
                }
            }
            else
            {
                if (anim != "Idle")
                {
                    animator.SetTrigger("Idle");
                    anim = "Idle";
                }
            }
        }
        else
        {
            if (anim != "Idle")
            {
                animator.SetTrigger("Idle");
                anim = "Idle";
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
        if (Input.GetAxis("Attack") > 0 && worldControl.paused == false)
        {
            for (int i = 0; i < playerCanAttack.Count; i++)
            {
                Destroy(playerCanAttack[i]);
            }
        }
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
}
