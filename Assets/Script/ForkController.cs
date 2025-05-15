﻿using UnityEngine;
using System.Collections;

public class ForkController : MonoBehaviour {
    AudioManager audioManager; // Referensi ke AudioManager untuk mengontrol suara

    public Transform fork; // Transform dari garpu (platform) forklift
    public Transform mast; // Transform dari tiang (mast) forklift
    public float speedTranslate; // Kecepatan pergerakan platform
    public Vector3 maxY; // Posisi Y maksimum platform
    public Vector3 minY; // Posisi Y minimum platform
    public Vector3 maxYmast; // Posisi Y maksimum tiang
    public Vector3 minYmast; // Posisi Y minimum tiang

    private bool mastMoveTrue = false; // Flag untuk mengaktifkan gerakan tiang
    private bool isPlayingLiftSound = false; // Flag untuk mengecek apakah suara lift sedang diputar

    private void Awake() {
        // Mencari dan menyimpan referensi ke AudioManager
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    void Start() {
        // Memainkan suara mesin saat forklift mulai
        audioManager.playSFX(audioManager.engineStart);
    }

    // Update dipanggil setiap frame
    void FixedUpdate () {
        // Mengecek posisi platform untuk menentukan apakah tiang harus bergerak
        if(fork.transform.localPosition.y >= maxYmast.y && fork.transform.localPosition.y < maxY.y)
        {
            mastMoveTrue = true; // Aktifkan gerakan tiang jika platform di atas batas tertentu
        }
        else
        {
            mastMoveTrue = false; // Nonaktifkan gerakan tiang
        }

        if (fork.transform.localPosition.y <= maxYmast.y)
        {
            mastMoveTrue = false; // Pastikan tiang tidak bergerak jika platform di bawah batas
        }
      
        // Gerakan platform naik saat tombol J ditekan
        if (Input.GetKey(KeyCode.J))
        {
            // Menggerakkan platform ke posisi maksimum
            fork.transform.localPosition = Vector3.MoveTowards(fork.transform.localPosition, maxY, speedTranslate * Time.deltaTime);
            
            // Memainkan suara lift jika belum diputar
            if (!isPlayingLiftSound)
            {
                audioManager.PlayLiftSound(true);
                isPlayingLiftSound = true;
            }

            // Menggerakkan tiang jika flag aktif
            if(mastMoveTrue)
            {
                mast.transform.localPosition = Vector3.MoveTowards(mast.transform.localPosition, maxYmast, speedTranslate * Time.deltaTime);
            }
        }
        // Gerakan platform turun saat tombol K ditekan
        else if (Input.GetKey(KeyCode.K))
        {
            // Menggerakkan platform ke posisi minimum
            fork.transform.localPosition = Vector3.MoveTowards(fork.transform.localPosition, minY, speedTranslate * Time.deltaTime);

            // Memainkan suara lift jika belum diputar
            if (!isPlayingLiftSound)
            {
                audioManager.PlayLiftSound(true);
                isPlayingLiftSound = true;
            }

            // Menggerakkan tiang jika flag aktif
            if (mastMoveTrue)
            {
                mast.transform.localPosition = Vector3.MoveTowards(mast.transform.localPosition, minYmast, speedTranslate * Time.deltaTime);
            }
        }
        else
        {
            // Menghentikan suara lift jika tidak ada tombol yang ditekan
            if (isPlayingLiftSound)
            {
                audioManager.PlayLiftSound(false);
                isPlayingLiftSound = false;
            }
        }
    }
}