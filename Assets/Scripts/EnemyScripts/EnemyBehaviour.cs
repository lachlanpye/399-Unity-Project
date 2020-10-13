using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public float moveSpeed;
    public float nodeFrequency;
    public float sineWaveFrequency;
    public float sineWaveAmplitude;

    [Space]
    [Header("Attacking")]
    public float attackTime;
    [Tooltip("Multiplies the player detection range by this amount before attempting to attack the player.")]
    public float increasedAttackRadius;

    [Space]
    [Header("Being attacked")]
    [Tooltip("Time the enemy can be in light before being stunned.")]
    public float timeBeforeStun;
    [Tooltip("How long the enemy is stunned for after leaving the light.")]
    public float stunnedTime;

    [Space]
    [Header("Game objects and sprites")]
    public GameObject playerObject;
    public GameObject gameController;
    [Tooltip("0 = normal sprite, 1 = stunned sprite")]
    public Sprite[] enemySprites;

    [Space]
    //Avoid string comparisons for this kind of stuff cause it's really easy to screw up. 
    //I think it can be replaced with an index from the scene's position in the build settings?
    [Tooltip("This must have the same name as one of the scenes in 'GameController|WorldControl'.")]
    public string enemyArea;

    private SpriteRenderer spriteRenderer;
    private WorldControl worldControl;
    private PlayerBehaviour playerBehaviour;
    private Animator animator;

    private GameObject attackIndicator;

    private Vector3 orthogonalVector;
    private Vector3 nextPosition;
    private int intervalOfNodes;
    private float randomMoveNum;

    private bool playerNear;
    private float currentTime;
    private int playerMask;

    private bool inLight;
    private float inLightTime;
    [HideInInspector]
    public bool stunned;

    // Start is called before the first frame update
    void Start()
    {
        nextPosition = new Vector3();
        randomMoveNum = Random.value * (2 * Mathf.PI);

        spriteRenderer = GetComponent<SpriteRenderer>();
        playerBehaviour = playerObject.GetComponent<PlayerBehaviour>();
        worldControl = gameController.GetComponent<WorldControl>();
        animator = GetComponent<Animator>();

        attackIndicator = transform.Find("attackIndicator").gameObject;
        attackIndicator.SetActive(false);

        playerNear = false;
        currentTime = 0;
        playerMask = LayerMask.GetMask("Player");

        UpdateOpacity(0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        //animation doesn't currently stop on game pause
        if (worldControl.paused == false)
        {
            if (playerNear == false && stunned == false)
            {
                if (playerBehaviour.currentArea == enemyArea && !worldControl.DialogueActive())
                {
                    currentTime = 0;

                    Vector3 distance = playerObject.GetComponent<Transform>().position - transform.position;
                    intervalOfNodes = Mathf.RoundToInt(Vector3.Magnitude(distance) / (nodeFrequency / 16));

                    // Orthogonal direction vector between enemy and player
                    orthogonalVector = Vector3.Normalize(Vector3.Cross(distance, new Vector3(0, 0, -90)));

                    nextPosition = transform.position
                                    + (distance / intervalOfNodes)
                                    + (orthogonalVector * (sineWaveAmplitude / 32) * Mathf.Sin((Time.time + randomMoveNum) * (sineWaveFrequency / 32)));

                    Vector2 dir = (nextPosition - transform.position).normalized;
                    transform.Translate(dir * moveSpeed * 0.5f * Time.deltaTime, Space.World);
                    int octan = Mathf.RoundToInt(4 * Mathf.Atan2(dir.y, dir.x) / (2 * Mathf.PI) + 4) % 4;

                    if (octan == 0 && animator.GetCurrentAnimatorStateInfo(0).IsName("bipedalWalkR") == false)
                    {
                        animator.SetTrigger("WalkRight");
                    }
                    else if (octan == 1 && animator.GetCurrentAnimatorStateInfo(0).IsName("bipedalWalkB") == false)
                    {
                        animator.SetTrigger("WalkBack");
                    }
                    else if (octan == 2 && animator.GetCurrentAnimatorStateInfo(0).IsName("bipedalWalkL") == false)
                    {
                        animator.SetTrigger("WalkLeft");
                    }
                    else if (octan == 3 && animator.GetCurrentAnimatorStateInfo(0).IsName("bipedalWalkF") == false)
                    {
                        animator.SetTrigger("WalkFront");
                    }
                }
            }
            if (playerNear == true && stunned == false)
            {
                currentTime += Time.deltaTime;
                if (currentTime >= attackTime)
                {
                    CapsuleCollider2D attackCollider = gameObject.GetComponentInChildren<CapsuleCollider2D>();
                    Collider2D[] objectsInsideRadius = Physics2D.OverlapCapsuleAll(new Vector2(transform.position.x, transform.position.y),
                                                                                    attackCollider.size * increasedAttackRadius, CapsuleDirection2D.Vertical,
                                                                                    0f, playerMask);
                    foreach (Collider2D obj in objectsInsideRadius)
                    {
                        if (obj.tag == "Player")
                        {
                            worldControl.TakeDamage();
                        }
                    }

                    currentTime = 0;
                    playerNear = false;
                }
            }

            if (inLight == true)
            {
                inLightTime += (Time.deltaTime * spriteRenderer.color.a);
            }
            else
            {
                inLightTime -= (Time.deltaTime / 2);
            }
            inLightTime = Mathf.Clamp(inLightTime, 0, timeBeforeStun);

            if (inLightTime == timeBeforeStun)
            {
                stunned = true;
                animator.SetTrigger("Stunned");
                spriteRenderer.sprite = enemySprites[1];
            }

            if (inLightTime == 0)
            {
                stunned = false;
                spriteRenderer.sprite = enemySprites[0];
            }
        }
    }

    public void PlayerEntersRange()
    {
        playerNear = true;
    }

    public void StartInLightCount()
    {
        inLight = true;
    }

    public void StopInLightCount()
    {
        inLight = false;
    }

    public void ShowAttackIndicator()
    {
        attackIndicator.SetActive(true);
    }

    public void HideAttackIndicator()
    {
        attackIndicator.SetActive(false);
    }

    public void UpdateOpacity(float value)
    {
        spriteRenderer.color = new Color(1f, 1f, 1f, value);
    }
}
