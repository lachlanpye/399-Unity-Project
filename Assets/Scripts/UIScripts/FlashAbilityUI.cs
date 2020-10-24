using UnityEngine;
using UnityEngine.UI;

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

    void Update()
    {
        if (playerBehaviour.canUseFlashAbility == true && playerBehaviour != null)
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
