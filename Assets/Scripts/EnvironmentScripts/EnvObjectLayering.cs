using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class EnvObjectLayering : MonoBehaviour
{
    [Tooltip("How far below the center of the sprite is the sprite's base")]
    public float spriteBasePoint;
    public bool hasShadows = true;

    private SpriteRenderer spriteRenderer;
    private ShadowCaster2D shadowCaster;
    private Transform playerT;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        shadowCaster = GetComponent<ShadowCaster2D>();
        playerT = GameObject.FindGameObjectWithTag("Player").transform;
    }

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
