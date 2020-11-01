using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

// Component that changes the layering of an object depending on their position relative to the player.
public class EnvObjectLayering : MonoBehaviour
{
    [Tooltip("How far below the center of the sprite is the sprite's base")]
    public float spriteBasePoint;
    public bool hasShadows = true;

    private SpriteRenderer spriteRenderer;
    private ShadowCaster2D shadowCaster;
    private Transform playerT;

    /// <summary>
    /// Lachlan Pye
    /// Initialize variables.
    /// </summary>
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        shadowCaster = GetComponent<ShadowCaster2D>();
        playerT = GameObject.FindGameObjectWithTag("Player").transform;
    }

    /// <summary>
    /// Lachlan Pye
    /// Move the game object to a different sorting layer depending on their y position relative to the player.
    /// This will cause the player to appear behind or in front of the object depending on their y position.
    /// </summary>
    void Update()
    {
        Debug.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - spriteBasePoint, transform.position.z), Color.green);

        

        if (playerT.position.y < transform.position.y - spriteBasePoint)
        {
            spriteRenderer.sortingLayerName = "BehindObject";
            if (hasShadows)
            {
                shadowCaster.selfShadows = false;
            }
        }
        else
        {
            spriteRenderer.sortingLayerName = "FrontObject";
            if (hasShadows)
            {
                shadowCaster.selfShadows = true;
            }
        }
    }
}
