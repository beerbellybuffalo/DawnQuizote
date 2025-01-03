using UnityEngine;
using System;
using System.Linq;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    
    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        PlayMusic("Main Menu bgm");
    }

    public void PlayMusic(string musicName)
    {
        Sound s = Array.Find(musicSounds, x => x.name == musicName);
        if (s != null)
        {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning("No music clip found with this name.");
        }
    }
    public void PlaySFX(string sfxName)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == sfxName);
        if (s != null)
        {
            sfxSource.PlayOneShot(s.clip);
        }
        else
        {
            Debug.LogWarning("No sfx clip found with this name.");
        }
    }

}
