using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

/// <summary>
/// Janine Aunzo
/// Contains all the audio clips for UI
/// Attach this script to the UIAudio object which is part of the UI object
/// </summary>

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

    /// <summary>
    /// Janine Aunzo
    /// Plays sound when hovering cursor over a button
    /// </summary>
    public void PlayHover()
    {
        AudioManager.publicInstance.PlaySFX(hover);
    }

    /// <summary>
    /// Janine Aunzo
    /// Plays sound when clicking a button
    /// </summary>
    public void PlayClick()
    {
        AudioManager.publicInstance.PlaySFX(click);
    }


    /// <summary>
    /// Janine Aunzo
    /// Plays save sound when clicking on save button
    /// </summary>
    public void PlaySave()
    {
        AudioManager.publicInstance.PlaySFX(save);
    }

    /// <summary>
    /// Janine Aunzo
    /// Plays back sound when clicking on back button
    /// </summary>
    public void PlayBack()
    {
        AudioManager.publicInstance.PlaySFX(back);
    }
    
    /// <summary>
    /// Janine Aunzo
    /// Plays the button hover sound only when the load slot button is active
    /// </summary>
    /// <param name="slotNum"></param>
    public void LoadSlotHover(string slotNum)
    {
        slotName = string.Concat("LoadSlot0", slotNum);
        slotButton = GameObject.Find(slotName).GetComponent<Button>();

        if (slotButton.isActiveAndEnabled == true)
        {
            AudioManager.publicInstance.PlaySFX(hover);
        }
    }

    /// <summary>
    /// Janine Aunzo
    /// Fades out music when loading a game
    /// </summary>
    public void LoadGame()
    {
        AudioManager.publicInstance.FadeOutBGM(0.2f);
    }
}
