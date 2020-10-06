using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AudioManager : MonoBehaviour
{
    private static AudioManager privateInstance;
    public static AudioManager publicInstance
    {
        get
        {
            if (privateInstance == null)
            {
                privateInstance = FindObjectOfType<AudioManager>();
                if (privateInstance == null)
                {
                    //Spawn new AudioManager if there isn't one when starting a scene
                    privateInstance = new GameObject("AudioManager", typeof(AudioManager)).GetComponent<AudioManager>();
                }
            }

            return privateInstance;
        }

        private set
        {
            privateInstance = value;
        }
    }

    private AudioSource bgmSource;
    private AudioSource sfxSource;
    //Used for any looping sfx or to stitch an intro/transition to another bgm clip
    private AudioSource extraSource;
    private AudioSource dialogueSource;

    private float[] baseVolumes;

    [Range(0f, 1.0f)]
    private float globalVolume;
    [Range(0f, 0.75f)]
    private float bgmVolume;
    [Range(0f, 1.0f)]
    private float fxVolume;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        bgmSource = this.gameObject.AddComponent<AudioSource>();
        sfxSource = this.gameObject.AddComponent<AudioSource>();
        extraSource = this.gameObject.AddComponent<AudioSource>();
        dialogueSource = this.gameObject.AddComponent<AudioSource>();

        bgmSource.loop = true;
        extraSource.loop = false;

        baseVolumes = GameObject.Find("EventSystem").GetComponent<UIButtonEvents>().GetAudioSettings();

        globalVolume = baseVolumes[0];
        bgmVolume = baseVolumes[1] * globalVolume;
        fxVolume = baseVolumes[2] * globalVolume;

        bgmSource.volume = bgmVolume;
        sfxSource.volume = fxVolume;
        extraSource.volume = fxVolume;
        dialogueSource.volume = fxVolume;
    }


    public void Instantiate()
    {
        Debug.Log("AudioManager instantiated");
    }


    public void PlayBGM(AudioClip bgmClip, double startTime)
    {
        bgmSource.clip = bgmClip;
        bgmSource.volume = baseVolumes[1] * globalVolume;
        bgmSource.PlayScheduled(startTime);
    }


    public void PlayBGM(AudioClip bgmClip)
    {
        bgmSource.clip = bgmClip;
        bgmSource.volume = baseVolumes[1] * globalVolume;
        bgmSource.Play();
    }

    


    public void PlaySFXLoop(AudioClip clip, double startTime)
    {
        extraSource.loop = true;
        extraSource.clip = clip;
        extraSource.volume = baseVolumes[2] * globalVolume;
        extraSource.Play();
    }

    public void StopSFXLoop()
    {
        extraSource.Stop();
    }


    public void FadeInSFXLoop(AudioClip clip, float fadeTime = 1.5f)
    {
        extraSource.loop = true;
        StartCoroutine(FadeIn(extraSource, clip, fadeTime));
    }


    public void FadeOutSFXLoop(float fadeTime = 1.5f)
    {
        StartCoroutine(FadeOut(extraSource, extraSource.clip, fadeTime));
    }



    public void PlayBGMTransition(AudioClip clip, double startTime)
    {
        extraSource.loop = false;
        extraSource.clip = clip;
        extraSource.volume = baseVolumes[1] * globalVolume;
        extraSource.PlayScheduled(startTime);
    }

    public void FadeInBGM(AudioClip bgmClip, float fadeTime = 1.5f)
    {
        StartCoroutine(FadeIn(bgmSource, bgmClip, fadeTime));
    }

    private IEnumerator FadeIn(AudioSource source, AudioClip clip, float fadeTime)
    {
        source.Stop();
        source.clip = clip;
        source.Play();

        for (float i = 0; i < fadeTime; i += Time.deltaTime)
        {
            source.volume = (i / fadeTime) * (bgmVolume * globalVolume);
            yield return null;
        }
    }

    public void PlaySFX(AudioClip sfxClip)
    {
        sfxSource.volume = baseVolumes[2] * globalVolume;
        sfxSource.PlayOneShot(sfxClip);
    }

    public void FadeOutBGM(float fadeTime = 1.0f)
    {
        StartCoroutine(FadeOut(bgmSource, bgmSource.clip, fadeTime));
    }

    private IEnumerator FadeOut(AudioSource source, AudioClip clip, float fadeTime)
    {
        for (float i = 0; i < fadeTime; i += Time.deltaTime)
        {
            source.volume = ((bgmVolume * globalVolume) - (i / fadeTime));
            yield return null;
        }

        source.Stop();
    }

    public void PlayDialogue(AudioClip clip)
    {
        dialogueSource.volume = baseVolumes[2] * globalVolume;
        dialogueSource.clip = clip;
        dialogueSource.Play();
    }

    public void SetGlobalVolume(System.Single globalVolume)
    {
        baseVolumes[0] = globalVolume;
        this.globalVolume = globalVolume;

        SetBGMVolume(baseVolumes[1]);
        SetSFXVolume(baseVolumes[2]);
    }

    public void SetBGMVolume(System.Single bgmVolume)
    {
        baseVolumes[1] = bgmVolume;
        bgmSource.volume = bgmVolume * globalVolume;
    }

    public void SetSFXVolume(System.Single sfxVolume)
    {
        baseVolumes[2] = sfxVolume;
        sfxSource.volume = sfxVolume * globalVolume;
        extraSource.volume = sfxVolume * globalVolume;
        dialogueSource.volume = sfxVolume * globalVolume;
    }
}
