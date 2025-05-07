using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulatorTrigger : MonoBehaviour
{
    public GameObject canvasSimulator;
    public GameObject canvasQuest;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.name  == "Player")
        {
            Time.timeScale = 0;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            canvasSimulator.SetActive(true);
            canvasQuest.SetActive(false);
        }
    }
}
