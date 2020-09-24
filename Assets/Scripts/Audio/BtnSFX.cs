using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnSFX : MonoBehaviour
{
    #pragma warning disable 0649
    [SerializeField] private AudioClip hover;
    [SerializeField] private AudioClip click;
    [SerializeField] private AudioClip save;
    [SerializeField] private AudioClip back;


    public void PlayHover()
    {
        AudioManager.publicInstance.PlaySFX(hover);
    }

    public void PlayClick()
    {
        AudioManager.publicInstance.PlaySFX(click);
    }

    public void PlaySave()
    {
        AudioManager.publicInstance.PlaySFX(save);
    }

    public void PlayBack()
    {
        AudioManager.publicInstance.PlaySFX(back);
    }
    #pragma warning restore 0649
}
