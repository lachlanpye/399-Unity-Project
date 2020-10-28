using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Component that controls the player gameobject's movement, animations and actions.
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
    public float invulnerabilityTime = 2f;
    public float rechargeScale;
    public float currentFlashlightTime;
    public bool flashlightBroke;
    public GameObject flashlight;
    public GameObject flashlightChargeUI;
    public GameObject lucasFlashAbilityUI;

    private RectTransform flashlightChargeUITransform;
    private Vector3 flashlightLocalPos;
    private bool flashlightEnabled;

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

    private string facingDirection;
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

    [HideInInspector]
    public int health;

    public bool canUseFlashAbility;
    private float flashAbilityCount;

    /// <summary>
    /// Lachlan Pye
    /// Intialize variables.
    /// </summary>
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

        facingDirection = "down";
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

    /// <summary>
    /// Lachlan Pye
    /// Update function provides various functions.
    /// </summary>
    void Update()
    {
        distance = moveSpeed * 0.5f * Time.deltaTime;
        flashlightLocalPos = new Vector3(0, 0, 0);

        // Lachlan Pye
        // If the player has full control, then allow them to move and do actions.
        if (worldControl.DialogueActive() == false && worldControl.paused == false && canMove == true)
        {
            // Lachlan Pye
            // If the player has the flash ability unlocked and presses the FlashAbility key, then
            // perform the white flash effect and stun all enemies in the scene.
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

            // Lachlan Pye
            // If the player is able to use their flashlight and the flashlight timer has not run out and the player presses the UseFlashlight key, then
            // enable the flashlight gameobject and increase the timer.
            if (Input.GetAxis("UseFlashlight") > 0.1 && currentFlashlightTime < flashlightActiveTime && flashlightBroke == false)
            {
                flashlight.SetActive(true);
                currentFlashlightTime += Time.deltaTime;
                if (currentFlashlightTime >= flashlightActiveTime)
                {
                    flashlightBroke = true;
                }
            }

            // Lachlan Pye
            // If the flashlight is not in use, then decrease the flashlight timer.
            else
            {
                flashlight.SetActive(false);
                currentFlashlightTime = Mathf.Clamp(currentFlashlightTime - (Time.deltaTime / rechargeScale), 0, Mathf.Infinity);
                if (currentFlashlightTime == 0)
                {
                    flashlightBroke = false;
                }
            }

            // Lachlan Pye
            // Update the flashlight charge UI based on what percentage the flashlight timer is at.
            if (flashlightChargeUI != null)
            {
                var flashlightBarRect = flashlightChargeUITransform.transform as RectTransform;
                flashlightChargeUITransform.sizeDelta = new Vector2(119 - (119 * (currentFlashlightTime / flashlightActiveTime)), flashlightChargeUITransform.sizeDelta.y);
            }

            // Lachlan Pye
            // If the player presses the WASD keys and there is not an obstacle in that direction, then move the player
            // in that direction, update the camera and play the correct animation.
            // If no keys are pressed, then play the Idle animation.
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

                facingDirection = "right";
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

                facingDirection = "up";
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

                facingDirection = "left";
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

                facingDirection = "down";
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

        // Lachlan Pye
        // Set the correct local position of the flashlight depending on the direction the player is currently facing.
        switch (facingDirection)
        {
            case "up":
                flashlightLocalPos.x = -0.16f;
                flashlightLocalPos.y = -0.096f;
                break;
            case "down":
                flashlightLocalPos.x = 0.192f;
                flashlightLocalPos.y = 0.009f;
                break;
            case "left":
                flashlightLocalPos.x = -0.152f;
                flashlightLocalPos.y = -0.062f;
                break;
            case "right":
                flashlightLocalPos.x = 0.261f;
                flashlightLocalPos.y = -0.057f;
                break;
        }

        flashlight.transform.localPosition = flashlightLocalPos;

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

        // Lachlan Pye
        // If there is anything in a certain direction, then set that direction to being blocked.
        // This stops the player from walking further in that direction.
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

    /// <summary>
    /// Lachlan Pye
    /// If the player uses the Attack key and they have control and the flashlight is enabled, then
    /// play the attack animation and kill all enemies that are close enough to be attacked right now.
    /// </summary>
    void LateUpdate()
    {
        if (Input.GetAxis("Attack") > 0 && worldControl.DialogueActive() == false && worldControl.paused == false && canMove == true && flashlightEnabled == true)
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

    /// <summary>
    /// Lachlan Pye
    /// Stop the player for a second while the attack animation is playing, and then allow control again.
    /// </summary>
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

    /// <summary>
    /// Lachlan Pye
    /// If the player is attacked by a common enemy, then play that animation.
    /// </summary>
    public IEnumerator PlayBipedalHurtAnimation()
    {
        anim = "BipedalAttack";
        animator.SetTrigger(anim);
        
        yield return new WaitForSeconds(0.667f * 2);

        anim = "Idle";
        animator.SetTrigger(anim);
    }
    /// <summary>
    /// Lachlan Pye
    /// If the player is killed by a common enemy, then play that animation.
    /// </summary>
    public IEnumerator PlayBipedalKillAnimation()
    {
        anim = "BipedalKill";
        animator.SetTrigger(anim);

        yield return new WaitForSeconds(3);

        anim = "Idle";
        animator.SetTrigger(anim);
    }

    /// <summary>
    /// Lachlan Pye
    /// If the player is attacked by the boss, then play that animation.
    /// </summary>
    public IEnumerator PlayBossHurtAnimation()
    {
        anim = "BossAttack";
        animator.SetTrigger(anim);

        yield return new WaitForSeconds(0.667f * 2);

        anim = "Idle";
        animator.SetTrigger(anim);

        yield return null;
    }
    /// <summary>
    /// Lachlan Pye
    /// If the player is killed by the boss, then play that animation.
    /// </summary>
    public IEnumerator PlayBossKillAnimation()
    {
        anim = "BossKill";
        animator.SetTrigger(anim);

        yield return null;
    }

    /// <summary>
    /// Lachlan Pye
    /// Adds an enemy object to the list of enemies that can be attacked right now, and show the attack indicator above them.
    /// </summary>
    public void AbleToAttack(GameObject obj)
    {
        if (!playerCanAttack.Contains(obj))
        {
            playerCanAttack.Add(obj);
            obj.GetComponent<EnemyBehaviour>().ShowAttackIndicator();
        }
    }

    /// <summary>
    /// Lachlan Pye
    /// Remove an enemy object from the list of enemies that can be attacked right now, and hide the attack indicator above them.
    /// </summary>
    public void NotAbleToAttack(GameObject obj)
    {
        if (playerCanAttack.Contains(obj))
        {
            playerCanAttack.Remove(obj);
            obj.GetComponent<EnemyBehaviour>().HideAttackIndicator();
        }
    }

    /// <summary>
    /// Lachlan Pye
    /// Set whether the flashlight and health UI gameobject should be enabled right now.
    /// </summary>
    /// <param name="value">Whether the flashlight and health UI gameobject should be enabled.</param>
    public void FlashlightEnabled(bool value)
    {
        flashlightEnabled = value;
        worldControl.GetHealthUI().SetActive(value);
    }
    /// <summary>
    /// Lachlan Pye
    /// Set the maximum charge of the flashlight in seconds.
    /// </summary>
    /// <param name="value">The maximum charge of the flashlight in seconds.</param>
    public void SetFlashlightActiveTime(float value)
    {
        flashlightActiveTime = value;
    }

    /// <summary>
    /// Lachlan Pye
    /// Set whether the player can use the flash ability or not.
    /// </summary>
    /// <param name="value">Whether the player can use the flash ability.</param>
    public void FlashAbility(bool value)
    {
        canUseFlashAbility = value;
    }

    /// <summary>
    /// Lachlan Pye
    /// Returns whether the flash ability cooldown is over or not.
    /// </summary>
    /// <returns>Whether the flash ability cooldown is over.</returns>
    public bool LucasFlashAbilityCooldownOver()
    {
        return flashAbilityCount == 0;
    }
}
