using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayEngineStart : MonoBehaviour
{
    AudioManager audioManager;

    private void Awake() {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        audioManager.playSFX(audioManager.engineStart);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
