using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public float moveSpeed;
    [Tooltip("The amount of waypoint nodes created for each segment of the path.")]
    public float nodeFrequency;
    [Space]
    public float sineWaveFrequency;
    public float sineWaveAmplitude;
    [Space]
    public float attackTime;
    [Tooltip("Multiplies the player detection range by this amount before attempting to attack the player.")]
    public float increasedAttackRadius;
    [Space]
    public GameObject playerObject;
    public GameObject gameController;
    [Space]
    [Tooltip("This must have the same name as one of the scenes in 'GameController|WorldControl'.")]
    public string enemyArea;

    private SpriteRenderer spriteRenderer;
    private WorldControl worldControl;
    private PlayerBehaviour playerBehaviour;

    private Vector3 orthogonalVector;
    private Vector3 nextPosition;
    private int intervalOfNodes;

    private bool playerNear;
    private float currentTime;
    private int playerMask;

    // Start is called before the first frame update
    void Start()
    {
        nextPosition = new Vector3();

        spriteRenderer = GetComponent<SpriteRenderer>();
        playerBehaviour = playerObject.GetComponent<PlayerBehaviour>();
        worldControl = gameController.GetComponent<WorldControl>();

        playerNear = false;
        currentTime = 0;
        playerMask = LayerMask.GetMask("Player");

        UpdateOpacity(0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerNear == false)
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
                                + (orthogonalVector * (sineWaveAmplitude / 32) * Mathf.Sin(Time.time * (sineWaveFrequency / 32)));

                transform.Translate((nextPosition - transform.position).normalized * moveSpeed * 0.5f * Time.deltaTime, Space.World);
            }
        }
        else
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
    }

    public void PlayerEntersRange()
    {
        playerNear = true;
    }

    public void UpdateOpacity(float value)
    {
        spriteRenderer.color = new Color(1f, 1f, 1f, value);
    }
}
