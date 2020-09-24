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
    private AudioSource sfxLoopSource;

    [Range(0f, 0.75f)]
    private float bgmVolume;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        bgmSource = this.gameObject.AddComponent<AudioSource>();
        sfxSource = this.gameObject.AddComponent<AudioSource>();
        sfxLoopSource = this.gameObject.AddComponent<AudioSource>();

        bgmSource.loop = true;
        sfxLoopSource.loop = true;

        bgmVolume = 0.75f;
        bgmSource.volume = bgmVolume;
    }

    public void PlayBGM(AudioClip bgmClip)
    {
        bgmSource.clip = bgmClip;
        bgmSource.volume = bgmVolume;
        bgmSource.Play();
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
            source.volume = i / fadeTime;
            yield return null;
        }
    }


    public void PlaySFX(AudioClip sfxClip)
    {
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
            source.volume = (1 - (i / fadeTime));
            yield return null;
        }

        source.Stop();
    }

    public void SetBGMVolume(float bgmVloume)
    {
        bgmSource.volume = bgmVloume;
    }

    public void SetSFXVolume(float sfxVolume)
    {
        sfxSource.volume = sfxVolume;
        sfxLoopSource.volume = sfxVolume;
    }
}
