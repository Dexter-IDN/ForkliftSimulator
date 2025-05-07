using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayGameFinish : MonoBehaviour
{
    public AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        audioManager.GetComponent<AudioManager>().PlayGameFinish();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
