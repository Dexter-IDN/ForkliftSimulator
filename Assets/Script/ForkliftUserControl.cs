using System;
using UnityEngine;

    [RequireComponent(typeof (ForkliftController))]
public class ForkliftUserControl : MonoBehaviour
    {
        AudioManager audioManager;
        private ForkliftController m_Car; // the car controller we want to use
        private bool isPlayingBeepSound = false;
        private bool isPlayingEngineForwardSound = false;

        private void Awake()
        {
            audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
            // get the car controller
            m_Car = GetComponent<ForkliftController>();
        }


        private void FixedUpdate()
        {
            // pass the input to the car!
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            
            // Deteksi input mundur langsung dari input mentah
            // bool isReversing = Input.GetAxis("Vertical") < 0;
            
            // Atau alternatif: deteksi berdasarkan input dan kecepatan mobil
            bool isReversing = (v < 0) || (v < 0.1f && m_Car.CurrentSpeed > 0.1f && Input.GetKey(KeyCode.DownArrow));
            
            // Kontrol suara beep saat mundur
            if(isReversing) {
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

            // Kontrol suara mesin saat maju
            if(v > 0) {
                if(!isPlayingEngineForwardSound) {
                    audioManager.PlayEngineForward(true);
                    isPlayingEngineForwardSound = true;
                }
            } else {
                if(isPlayingEngineForwardSound) {
                    audioManager.PlayEngineForward(false);
                    isPlayingEngineForwardSound = false;
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

