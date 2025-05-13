using System;
using UnityEngine;

    [RequireComponent(typeof (NewCarController))]
public class NewCarUserControl : MonoBehaviour
    {
        AudioManager audioManager;
        private NewCarController m_Car; // the car controller we want to use
        private bool isPlayingBeepSound = false;

        private void Awake()
        {
            audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
            // get the car controller
            m_Car = GetComponent<NewCarController>();
        }


        private void FixedUpdate()
        {
            // pass the input to the car!
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            if(v < 0) {
                if(!isPlayingBeepSound) {
                    audioManager.PlayBeepSound(true);
                    isPlayingBeepSound = true;
                }
            } else {
                if(isPlayingBeepSound) {
                    audioManager.PlayBeepSound(false);
                    isPlayingBeepSound = false;
                }
            }

            #if !MOBILE_INPUT
                        float handbrake = Input.GetAxis("Jump");
                        m_Car.Move(h, v, v, handbrake);
                        
            #else
                        m_Car.Move(h, v, v, 0f);
            #endif
        }
    }

