﻿using UnityEngine;

public class ForkController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform fork;
    [SerializeField] private Transform mast;
    private AudioManager audioManager;

    [Header("Settings")]
    [SerializeField] private float speedTranslate = 1f;
    [SerializeField] private Vector3 maxY;
    [SerializeField] private Vector3 minY;
    [SerializeField] private Vector3 maxYmast;
    [SerializeField] private Vector3 minYmast;

    private bool isPlayingLiftSound = false;

    private void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio")?.GetComponent<AudioManager>();
    }

    private void FixedUpdate()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        bool moveUp = Input.GetKey(KeyCode.J);
        bool moveDown = Input.GetKey(KeyCode.K);

        if (moveUp)
        {
            MoveFork(maxY);
            MoveMastIfNeeded(maxYmast);
            PlayLiftSound(true);
        }
        else if (moveDown)
        {
            MoveFork(minY);
            MoveMastIfNeeded(minYmast);
            PlayLiftSound(true);
        }
        else
        {
            PlayLiftSound(false);
        }
    }

    private void MoveFork(Vector3 target)
    {
        fork.localPosition = Vector3.MoveTowards(fork.localPosition, target, speedTranslate * Time.deltaTime);
    }

    private void MoveMastIfNeeded(Vector3 target)
    {
        if (ShouldMoveMast())
        {
            mast.localPosition = Vector3.MoveTowards(mast.localPosition, target, speedTranslate * Time.deltaTime);
        }
    }

    private bool ShouldMoveMast()
    {
        float forkY = fork.localPosition.y;
        return forkY > maxYmast.y && forkY < maxY.y;
    }

    private void PlayLiftSound(bool play)
    {
        if (audioManager == null) return;

        if (play && !isPlayingLiftSound)
        {
            audioManager.PlayLiftSound(true);
            isPlayingLiftSound = true;
        }
        else if (!play && isPlayingLiftSound)
        {
            audioManager.PlayLiftSound(false);
            isPlayingLiftSound = false;
        }
    }
}