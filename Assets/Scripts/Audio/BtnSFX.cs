using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class BtnSFX : MonoBehaviour
{
    #pragma warning disable 0649
    [SerializeField] private AudioClip hover;
    [SerializeField] private AudioClip click;
    [SerializeField] private AudioClip save;
    [SerializeField] private AudioClip back;
#pragma warning restore 0649

    [HideInInspector]
    public string slotName;
    [HideInInspector]
    public Button slotButton;


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
    

    public void LoadSlotHover(string slotNum)
    {
        slotName = string.Concat("LoadSlot0", slotNum);
        slotButton = GameObject.Find(slotName).GetComponent<Button>();

        if (slotButton.isActiveAndEnabled == true)
        {
            AudioManager.publicInstance.PlaySFX(hover);
        }
    }


    public void LoadGame()
    {
        AudioManager.publicInstance.FadeOutBGM(0.2f);
    }
}
