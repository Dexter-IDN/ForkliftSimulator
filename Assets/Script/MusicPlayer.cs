using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public GameObject audioManager;
    public enum AudioClip
    {
        MainMenu,
        SecondaryMenu,
        GamePlay,
        GameFinish
    }

    public AudioClip music;

    // Start is called before the first frame update
    void Start()
    {
        switch (music)
        {
            case AudioClip.MainMenu:
                audioManager.GetComponent<AudioManager>().PlayMainMenu();
                break;
            case AudioClip.SecondaryMenu:
                audioManager.GetComponent<AudioManager>().PlaySecondaryMenu();
                break;
            case AudioClip.GamePlay:
                audioManager.GetComponent<AudioManager>().PlayGamePlay();
                break;
            case AudioClip.GameFinish:
                audioManager.GetComponent<AudioManager>().PlayGameFinish();
                break;
            default:
                Debug.Log("No music selected");
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
