using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;

    [Header("Audio Clip")]
    public AudioClip backgroundMenu;
    public AudioClip secondaryMenu;
    public AudioClip backgroundGame;
    public AudioClip gameFinish;
    public AudioClip engineStart;
    public AudioClip beepingLoop;
    public AudioClip raiseDownFork;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayBackgroundMenu() {
        musicSource.clip = backgroundMenu;
        musicSource.Play();
    }

    public void PlayBackgroundGame() {
        musicSource.clip = backgroundGame;
        musicSource.Play();
    }

    public void PlaySecondaryMenu() {
        musicSource.clip = secondaryMenu;
        musicSource.Play();
    }

    public void PlayGameFinish() {
        musicSource.clip = gameFinish;
        musicSource.Play();
    }

    public void playSFX(AudioClip clip) {
        sfxSource.PlayOneShot(clip);
    }
}
