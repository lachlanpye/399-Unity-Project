using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    [Tooltip("Number of pixels per second to move.")]
    public float moveSpeed;
    [Space]
    [Header("How far the distance is from the player in that direction where collisions are detected.")]
    public float upColliderDistance;
    public float leftColliderDistance;
    public float rightColliderDistance;
    public float downColliderDistance;
    [Space]

    public GameObject gameController;
    public GameObject spotlight;

    [Space]
    public string currentArea;

    private SpriteRenderer spriteRenderer;
    private WorldControl worldControl;
    private Animator animator;

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

        foreach (Transform t in transform)
        {
            if (t.name == "playerAttackRadius")
            {
                attackRange = t.gameObject.GetComponent<CapsuleCollider2D>();
                Debug.Log(attackRange);
            }
        }
        playerCanAttack = new List<GameObject>();

        objectMask = LayerMask.GetMask("Object");

        blockUp = false;
        blockLeft = false;
        blockRight = false;
        blockDown = false;
    }

    void Update()
    {
        distance = moveSpeed * 0.5f * Time.deltaTime;

        if (worldControl.DialogueActive() == false)
        {
            if (Input.GetAxis("Horizontal") > 0.5 && blockRight == false)
            {
                transform.Translate(new Vector3(distance, 0, 0));
                spotlight.transform.eulerAngles = new Vector3(0, 0, -90);

                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Military_Char_walking_right") == false)
                {
                    animator.SetTrigger("WalkRight");
                }
            }
            else if (Input.GetAxis("Horizontal") < -0.5 && blockLeft == false)
            {
                transform.Translate(new Vector3(-distance, 0, 0));
                spotlight.transform.eulerAngles = new Vector3(0, 0, 90);

                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Military_Char_walking_left") == false)
                {
                    animator.SetTrigger("WalkLeft");
                }
            }
            else if (Input.GetAxis("Vertical") > 0.5 && blockUp == false)
            {
                transform.Translate(new Vector3(0, distance, 0));
                spotlight.transform.eulerAngles = new Vector3(0, 0, 0);

                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Military_Char_walking_back") == false)
                {
                    animator.SetTrigger("WalkBack");
                }
            }
            else if (Input.GetAxis("Vertical") < -0.5 && blockDown == false)
            {
                transform.Translate(new Vector3(0, -distance, 0));
                spotlight.transform.eulerAngles = new Vector3(0, 0, 180);

                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Military_Char_walking_front") == false)
                {
                    animator.SetTrigger("WalkFront");
                }
            }
            else
            {
                animator.SetTrigger("Idle");
            }
        }

        // Cast rays in all 4 directions for wall detection
        upCast = Physics2D.Raycast(transform.position - (Vector3.up * 0.5f), Vector2.up, upColliderDistance, objectMask);
        leftCast = Physics2D.Raycast(transform.position - (Vector3.up * 0.5f), Vector2.left, leftColliderDistance, objectMask);
        rightCast = Physics2D.Raycast(transform.position - (Vector3.up * 0.5f), Vector2.right, rightColliderDistance, objectMask);
        downCast = Physics2D.Raycast(transform.position - (Vector3.up * 0.5f), Vector2.down, downColliderDistance, objectMask);

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
        if (Input.GetAxis("Attack") > 0)
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

    private void RoundPositionX()
    {
        /*Vector3 pos = transform.position;
        pos.x = (float) (Mathf.Round(pos.x));
        transform.position = pos;*/
    }
    private void RoundPositionY()
    {
        /*Vector3 pos = transform.position;
        pos.y = (float)(Mathf.Round(pos.y));
        transform.position = pos;*/
    }
}
