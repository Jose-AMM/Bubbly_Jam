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
                case "COMODIN":
                    LoadClip(Clip, 1.0f);
                    break;
                case "MUS-AGUA":
                    LoadClip(Clip, 1.0f);
                    break;
                case "MUS-AGUA-GLITCH":
                    LoadClip(Clip, 0.0f);
                    break;
                case "MUS-TIENDA":
                    LoadClip(Clip, 0.0f);
                    break;
                case "MUS-TIENDA-GLITCH":
                    LoadClip(Clip, 0.0f);
                    break;
                case "ALARM":
                    LoadClip(Clip, 0.0f);
                    break;
                case "CHORD-TENSION":
                    LoadClip(Clip, 0.0f);
                    break;
                case "BUBBLE-LOOP":
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

    public void EnterShop()
    {
        float GlitchedMusicRatio = GameManager.Instance.GlitchedMusicRatio;
        StopSounds();
        SetVolume("COMODIN", 0.0f, 0.0f);
        SetVolume("MUS-AGUA", 0.0f, 0.5f);
        SetVolume("MUS-AGUA-GLITCH", 0.0f, 0.5f);
        BeginClip("MUS-TIENDA-GLITCH", 0.0f);
        SetVolume("MUS-TIENDA-GLITCH", GlitchedMusicRatio, 5.0f);
        BeginClip("MUS-TIENDA", 0.0f);
        SetVolume("MUS-TIENDA", 1.0f-GlitchedMusicRatio, 5.0f);
        PlaySound("PUERTA", 1.0f);
    }

    public void ExitShop()
    {
        float GlitchedMusicRatio = GameManager.Instance.GlitchedMusicRatio;
        StopSounds();
        SetVolume("COMODIN", 1.0f, 0.0f);
        SetVolume("MUS-TIENDA", 0.0f, 1.0f);
        SetVolume("MUS-TIENDA-GLITCH", 0.0f, 1.0f);
        SetVolume("MUS-AGUA-GLITCH", GlitchedMusicRatio, 1.0f);
        SetVolume("MUS-AGUA", (1.0f-GlitchedMusicRatio), 1.0f);
    }
}
