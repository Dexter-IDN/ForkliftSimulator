using UnityEngine;
using System.Collections;

public class ForkController : MonoBehaviour {
    AudioManager audioManager;

    public Transform fork; 
    public Transform mast;
    public float speedTranslate; //Platform travel speed
    public Vector3 maxY; //The maximum height of the platform
    public Vector3 minY; //The minimum height of the platform
    public Vector3 maxYmast; //The maximum height of the mast
    public Vector3 minYmast; //The minimum height of the mast

    private bool mastMoveTrue = false; //Activate or deactivate the movement of the mast
    private bool isPlayingLiftSound = false; // Track if lift sound is currently playing

    private void Awake() {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    // Update is called once per frame
    void FixedUpdate () {
        // Debug.Log(mastMoveTrue);
        if(fork.transform.localPosition.y >= maxYmast.y && fork.transform.localPosition.y < maxY.y)
        {
            mastMoveTrue = true;
        }
        else
        {
            mastMoveTrue = false;
        }

        if (fork.transform.localPosition.y <= maxYmast.y)
        {
            mastMoveTrue = false;
        }
      
        if (Input.GetKey(KeyCode.J))
        {
            //fork.Translate(Vector3.up * speedTranslate * Time.deltaTime);
            fork.transform.localPosition = Vector3.MoveTowards(fork.transform.localPosition, maxY, speedTranslate * Time.deltaTime);
            
            // Play sound if not already playing
            if (!isPlayingLiftSound)
            {
                audioManager.PlayLiftSound(true);
                isPlayingLiftSound = true;
            }

            if(mastMoveTrue)
            {
                mast.transform.localPosition = Vector3.MoveTowards(mast.transform.localPosition, maxYmast, speedTranslate * Time.deltaTime);
            }
        }
        else if (Input.GetKey(KeyCode.K))
        {
            fork.transform.localPosition = Vector3.MoveTowards(fork.transform.localPosition, minY, speedTranslate * Time.deltaTime);

            // Play sound if not already playing
            if (!isPlayingLiftSound)
            {
                audioManager.PlayLiftSound(true);
                isPlayingLiftSound = true;
            }

            if (mastMoveTrue)
            {
                mast.transform.localPosition = Vector3.MoveTowards(mast.transform.localPosition, minYmast, speedTranslate * Time.deltaTime);
            }
        }
        else
        {
            // Stop sound if no keys are pressed
            if (isPlayingLiftSound)
            {
                audioManager.PlayLiftSound(false);
                isPlayingLiftSound = false;
            }
        }
    }
}
