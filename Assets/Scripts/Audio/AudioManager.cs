using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Janine Aunzo
/// This script generates an AudioManager object that handles all 2D audio.
/// The AudioManager object does not get destroyed at scene change.
/// 
/// 
/// *** Do not manually add an AudioManager object into the scene, an object will be automatically generated if there isn't
/// one. Having more than 1 AudioManager object may cause some unexpected audio problems.
/// </summary>
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

    //baseVolumes[0] = global volume
    //baseVolumes[1] = music volume
    //baseVolumes[2] = sfx volume
    private float[] baseVolumes;

    [Range(0f, 1.0f)]
    private float globalVolume;
    [Range(0f, 0.75f)]
    private float bgmVolume;
    [Range(0f, 1.0f)]
    private float fxVolume;

    /// <summary>
    /// Janine Aunzo
    /// Sets all the audio sources and their settings.
    /// </summary>
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

    /// <summary>
    /// Janine Aunzo
    /// Resets volume and pitch of extraSource audio.
    /// This also instantiates an AudioManager object if there already isn't one in the scene.
    /// </summary>
    public void Instantiate()
    {
        UnmuteSFXAnim();
        extraSource.pitch = 1f;
    }

    /// <summary>
    /// Overloaded PlayBGM method which schedules play at the given start time.
    /// </summary>
    /// <param name="bgmClip">Music clip to be played.</param>
    /// <param name="startTime">Time in seconds on when audio should start playing.</param>
    public void PlayBGM(AudioClip bgmClip, double startTime)
    {
        bgmSource.clip = bgmClip;
        bgmSource.volume = baseVolumes[1] * globalVolume;
        bgmSource.PlayScheduled(startTime);
    }

    /// <summary>
    /// Janine Aunzo
    /// Plays music.
    /// </summary>
    /// <param name="bgmClip">Audio clip to be played.</param>
    public void PlayBGM(AudioClip bgmClip)
    {
        bgmSource.clip = bgmClip;
        bgmSource.volume = baseVolumes[1] * globalVolume;
        bgmSource.Play();
    }

    /// <summary>
    /// Janine Aunzo
    /// Plays the first out of 2 clips that will be seamlessly stiched together.
    /// </summary>
    /// <param name="clip">Audio clip to be played.</param>
    /// <param name="startTime">Time in seconds on when audio should start playing.</param>
    public void PlayBGMTransition(AudioClip clip, double startTime)
    {
        extraSource.loop = false;
        extraSource.clip = clip;
        extraSource.volume = baseVolumes[1] * globalVolume;
        extraSource.PlayScheduled(startTime);
    }

    /// <summary>
    /// Janine Aunzo
    /// Fades in music. Default fade time is 1 second.
    /// </summary>
    /// <param name="bgmClip">Music audio clip that will be faded in.</param>
    /// <param name="fadeTime">Time it takes to reach full volume.</param>
    public void FadeInBGM(AudioClip bgmClip, float fadeTime = 1.5f)
    {
        StartCoroutine(FadeIn(bgmSource, bgmClip, fadeTime));
    }

    /// <summary>
    /// Janine Aunzo
    /// Stops music.
    /// </summary>
    public void StopBGM()
    {
        bgmSource.Stop();
    }

    /// <summary>
    /// Janine Aunzo
    /// Fades out music.
    /// </summary>
    /// <param name="fadeTime">Time it takes for volume to reach 0.</param>
    public void FadeOutBGM(float fadeTime = 1f)
    {
        StartCoroutine(FadeOut(bgmSource, fadeTime));
    }

    /// <summary>
    /// Janine Aunzo
    /// Plays a looping sound effect.
    /// </summary>
    /// <param name="clip">Sound effect that will be played on loop.</param>
    public void PlaySFXLoop(AudioClip clip)
    {
        extraSource.loop = true;
        extraSource.clip = clip;
        extraSource.volume = baseVolumes[2] * globalVolume;
        extraSource.Play();
    }

    /// <summary>
    /// Janine Aunzo
    /// Stops a looping sound effect.
    /// </summary>
    public void StopSFXLoop()
    {
        extraSource.Stop();
    }


    /// <summary>
    /// Janine Aunzo
    /// Mutes the sound triggered by animation events in a looping animation.
    /// </summary>
    public void MuteSFXAnim()
    {
        extraSource.volume = 0;
    }

    /// <summary>
    /// Janine Aunzo
    /// Unmutes the sound triggered by animation events.
    /// </summary>
    public void UnmuteSFXAnim()
    {
        extraSource.volume = fxVolume;
    }

    /// <summary>
    /// Janine Aunzo
    /// Fades in a looping sound effect. Default fade time is 1 second.
    /// </summary>
    /// <param name="clip">Audio clip that will be faded in.</param>
    /// <param name="fadeTime">Time it takes to reach full volume.</param>
    public void FadeInSFXLoop(AudioClip clip, float fadeTime = 1.5f)
    {
        extraSource.loop = true;
        StartCoroutine(FadeIn(extraSource, clip, fadeTime));
    }

    /// <summary>
    /// Janine Aunzo
    /// Fades out a looping sound effect. Default fade time is 1 second.
    /// </summary>
    /// <param name="fadeTime">Time it takes for volume to reach 0.</param>
    public void FadeOutSFXLoop(float fadeTime = 1f)
    {
        StartCoroutine(FadeOut(extraSource, fadeTime));
    }

    /// <summary>
    /// Janine Aunzo
    /// Overloaded PlaySFX where pitch is modified.
    /// </summary>
    /// <param name="sfxClip">Sound effect audio clip to be played.</param>
    /// <param name="pitch">The pitch the audio will be played at.</param>
    public void PlaySFX(AudioClip sfxClip, float pitch)
    {
        extraSource.pitch = pitch;
        extraSource.volume = baseVolumes[2] * globalVolume;
        extraSource.PlayOneShot(sfxClip);
    }

    /// <summary>
    /// Janine Aunzo
    /// Play sound effect.
    /// </summary>
    /// <param name="sfxClip">Sound effect audio clip to be played.</param>
    public void PlaySFX(AudioClip sfxClip)
    {
        sfxSource.volume = baseVolumes[2] * globalVolume;
        sfxSource.PlayOneShot(sfxClip);
    }

    /// <summary>
    /// Janine Aunzo
    /// Play dialogue blips.
    /// </summary>
    /// <param name="clip">Audio clip of dialogue speaker.</param>
    public void PlayDialogue(AudioClip clip)
    {
        dialogueSource.volume = baseVolumes[2] * globalVolume;
        dialogueSource.clip = clip;
        dialogueSource.Play();
    }

    /// <summary>
    /// Janine Aunzo
    /// Get sound effects volume.
    /// </summary>
    /// <returns>Sound effects volume.</returns>
    public float GetSFXVolume()
    {
        return baseVolumes[2] * globalVolume;
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

    /// <summary>
    /// Janine Aunzo
    /// Fades in audio by gradually increasing volume within the given fade time.
    /// </summary>
    /// <param name="source">Audio source where audio will be faded in.</param>
    /// <param name="clip">Audio clip that will be faded in.</param>
    /// <param name="fadeTime">Time it takes to reach full volume.</param>
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

    /// <summary>
    /// Janine Aunzo
    /// Fades out audio by gradually decreasing volume within the given fade time.
    /// </summary>
    /// <param name="source">Audio source of the audio being faded out.</param>
    /// <param name="fadeTime">Time it takes for volume to reach 0.</param>
    private IEnumerator FadeOut(AudioSource source, float fadeTime)
    {
        for (float i = 0; i < fadeTime; i += Time.deltaTime)
        {
            source.volume = ((bgmVolume * globalVolume) - (i / fadeTime));
            yield return null;
        }

        source.Stop();
    }
}
