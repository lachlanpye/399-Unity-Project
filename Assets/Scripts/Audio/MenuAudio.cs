using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAudio : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private AudioClip mainBGM;
    [SerializeField] private AudioClip newGameChord;
    [SerializeField] private AudioClip introBGM1;

    private void Awake()
    {
        AudioManager.publicInstance.PlayBGM(mainBGM);
    }


    public void FadeOutTheme()
    {
        AudioManager.publicInstance.FadeOutBGM(1f);
    }
#pragma warning restore 0649

    public void NewGameTransition()
    {
        StartCoroutine(Transition());
    }

    IEnumerator Transition()
    {
        yield return new WaitForSeconds(1f);

        double startTime = AudioSettings.dspTime + 0.2;
        AudioManager.publicInstance.PlayBGMTransition(newGameChord, startTime);

        double duration = newGameChord.length;
        AudioManager.publicInstance.PlayBGM(introBGM1, startTime + duration);
    }
}
