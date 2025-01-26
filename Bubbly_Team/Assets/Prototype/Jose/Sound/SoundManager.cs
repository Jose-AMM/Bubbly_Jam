using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{


    [SerializeField]
    private List<AudioClip> AudioClips;
    [SerializeField]
    private List<AudioClip> Sounds;
    [SerializeField]
    public AudioSource audioSource;
    public static SoundManager Instance  { get; private set; }


    void Awake(){
        
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        foreach (var Clip in AudioClips)
        {
            switch (Clip.name){
                case "Horror":
                    LoadClip(Clip, 0.6f);
                    break;
                case "Goofy":
                    LoadClip(Clip, 0.0f);
                    break;
                default:
                    break;
            }
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadClip(AudioClip Clip, float Volume){
            AudioSource Source = gameObject.AddComponent<AudioSource>();
            Source.clip = Clip;
            Source.playOnAwake = false;
            Source.volume = Volume;
            Source.loop = true;
            Source.Play();
    }

    public void PlaySound(String ClipName, float Volume){
        AudioSource Source = gameObject.AddComponent<AudioSource>();
        Source.clip = GetClipSoundByName(ClipName);
        Source.playOnAwake = false;
        Source.volume = Volume;
        Source.loop = false;
        Source.Play();
        StartCoroutine(RemoveSoundSourceWhenFinished(Source));
    }
    
    IEnumerator RemoveSoundSourceWhenFinished(AudioSource Source){
        yield return new WaitForSeconds(Source.clip.length);
        Destroy(Source);
    }

    private AudioClip GetClipSoundByName(String ClipName){
        foreach (var Clip in Sounds)
        {
            if (Clip.name == ClipName)
            {
                return Clip;
            }
        }

        Debug.Log("Clip not found: " + ClipName);
        return null;
    }

    private AudioSource GetSourceByName(String ClipName){
        foreach (AudioSource Source in gameObject.GetComponents(typeof(AudioSource))){
            if (Source.clip.name == ClipName){
                return Source;
            }
        }
        Debug.Log("Clip not found: " + ClipName);
        return null;
    }

    public void BeginClip(String ClipName, float Volume){

        AudioSource Source = GetSourceByName(ClipName);
        Source.Stop();
        Source.Play();
        Source.volume = Volume;
        return;
    }

    public void StopSounds(){
        foreach (AudioSource Source in gameObject.GetComponents(typeof(AudioSource))){
            if (Sounds.Contains(Source.clip)){
                Source.volume = 0.0f;
            }
        }
    }

    public void SetVolume(String ClipName, float Volume, float FadeTime){
        AudioSource Source = GetSourceByName(ClipName);
        if (FadeTime == 0.0f){
            Source.volume = Volume;
        } else {
            StartCoroutine(FadeVolume(Source, Source.volume, Volume, FadeTime));
        }
        return;
    }

    private IEnumerator FadeVolume(AudioSource Source, float InitialVolume, float TargetVolume, float FadeTime){
        float ElapsedTime = 0.0f;
        while (ElapsedTime < FadeTime)
        {
            ElapsedTime += Time.deltaTime;
            Source.volume = Mathf.Lerp(InitialVolume, TargetVolume, ElapsedTime / FadeTime);
            yield return null;
        }
    }
}
