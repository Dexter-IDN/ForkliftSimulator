using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudio : MonoBehaviour
{
    public AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        audioManager.GetComponent<AudioManager>().PlayBackgroundMenu();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
