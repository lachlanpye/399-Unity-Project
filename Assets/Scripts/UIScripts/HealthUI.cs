using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public Sprite fullHealthSprite;
    public Sprite halfHealthSprite;

    private Image image;

    void Start()
    {
        image = GetComponent<Image>();
        FullHealth();
    }

    public void FullHealth()
    {
        image.sprite = fullHealthSprite;
    }

    public void HalfHealth()
    {
        image.sprite = halfHealthSprite;
    }
}
