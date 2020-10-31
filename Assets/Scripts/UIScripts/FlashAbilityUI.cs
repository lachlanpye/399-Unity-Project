using UnityEngine;
using UnityEngine.UI;

// Component used to control the look of the flash ability UI sprite.
public class FlashAbilityUI : MonoBehaviour
{
    public GameObject playerObject;
    [Space]
    public Sprite cooldownActive;
    public Sprite ready1;
    public Sprite ready2;

    private PlayerBehaviour playerBehaviour;
    private Image image;

    private float animationTimer;
    private bool bobbing;

    /// <summary>
    /// Lachlan Pye
    /// Initialize variables.
    /// </summary>
    void Start()
    {
        if (playerObject != null)
        {
            playerBehaviour = playerObject.GetComponent<PlayerBehaviour>();
        }
        image = GetComponent<Image>();

        image.enabled = false;

        animationTimer = 1.0f;
        bobbing = true;
    }

    /// <summary>
    /// Lachlan Pye
    /// Each frame, if the player can use the flash ability, enable the specific UI element.
    /// If the ability is on a cooldown, show the cooldown sprite.
    /// If the ability is ready, then alternate between the two ready sprites.
    /// </summary>
    void Update()
    {
        if (playerBehaviour != null && playerBehaviour.canUseFlashAbility == true)
        {
            image.enabled = true;

            if (playerBehaviour.LucasFlashAbilityCooldownOver() == true)
            {
                animationTimer -= Time.deltaTime;
                if (animationTimer <= 0)
                {
                    animationTimer = 1.0f;
                    bobbing = !bobbing;
                }

                if (bobbing)
                {
                    image.sprite = ready1;
                }
                else
                {
                    image.sprite = ready2;
                }
            }
            else
            {
                image.sprite = cooldownActive;
            }
        }
    }
}
