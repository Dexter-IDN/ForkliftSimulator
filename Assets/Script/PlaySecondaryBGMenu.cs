using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySecondaryBGMenu : MonoBehaviour
{
    public AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        audioManager.GetComponent<AudioManager>().PlaySecondaryMenu();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
